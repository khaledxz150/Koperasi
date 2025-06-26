using System.Net;

using Core.Services.User;
using Core.UnitOfWork;

using Infrastructure.Extensions;
using Infrastructure.Helpers.HttpContext;
using Infrastructure.Helpers.Security;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Models.Entities.User;
using Models.Enums.System;
using Models.Enums.User;
using Models.ViewModels.Options;
using Models.ViewModels.User.Base;
using Models.ViewModels.User.Request;
using Models.ViewModels.User.Response;

namespace Application.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Users> _userManager;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<UserService> _logger;
        private readonly int _LanguageID;
        private readonly IOptions<TwilioOptions> _twilioOptions;
        private readonly IOptions<SmtpOptions> _smtpOptions;

        public UserService(
            UserManager<Users> userManager,
            IMemoryCache memoryCache,
            ILogger<UserService> logger,
            IOptions<TwilioOptions> twilioOptions,
            IOptions<SmtpOptions> smtpOptions,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _memoryCache = memoryCache;
            _logger = logger;
            _LanguageID = HTTPContextHelper.GetCurrentLanguageFromHeader();
            _twilioOptions = twilioOptions;
            _smtpOptions = smtpOptions;
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.ICNumber == request.ICNumber);
            if (user == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = _memoryCache.GetWord(_LanguageID, 124) // "User not found"
                };
            }

            if (user.Status != RegistrationStatusEnum.Completed)
            {
                string mobileOtp = GenerateOtp();
                user.MobileOTPHash = mobileOtp.SaltHash(user.Salt);
                user.Status = RegistrationStatusEnum.MobileVerification;
                SendOtpViaTwilio(user.PhoneNumber, mobileOtp);

                return new LoginResponse
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    NextStep = RegistrationStatusEnum.MobileVerification
                };
            }
            

            return new LoginResponse
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = _memoryCache.GetWord(_LanguageID, 192),
                Data = new
                {
                    UserCode = user.Id.Encrypt(),
                    FullName = user.FullName
                }
            };
        }


        public async Task<PersonalInfoResponse> CreateUserAsync(PersonalInfoRequest request)
        {
            try
            {
                if (await IsICNumberExistsAsync(request.ICNumber))
                {
                    return FailedPersonalInfoResponse(127, 184, ValidationPopupTypesEnum.SuggestLoginAndRetry);
                }

                var user = BuildNewUser(request);
                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    return FailedIdentityResultResponse(result);
                }

                string mobileOtp = GenerateOtp();
                user.MobileOTPHash = mobileOtp.SaltHash(user.Salt);
                user.Status = RegistrationStatusEnum.MobileVerification;

                await _userManager.UpdateAsync(user);
                SendOtpViaTwilio(user.PhoneNumber, mobileOtp);

                return SuccessPersonalInfoResponse(user, RegistrationStatusEnum.MobileVerification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return (PersonalInfoResponse)GeneralFailResponse(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<VerificationResponse> VerifyMobileOTPAsync(MobileVerificationRequest request)
        {
            var user = await GetUserById(request.UserCode);
            if (user == null) return new VerificationResponse(UserNotFoundResponse());

            if (!IsOtpValid(request.OTP, user.MobileOTPHash, user.Salt))
            {
                return InvalidOtpResponse(178, 183);
            }

            string emailOtp = GenerateOtp();
            user.MobileOTPHash = null;
            user.EmailOTPHash = emailOtp.SaltHash(user.Salt);
            user.Status = RegistrationStatusEnum.EmailVerification;
            user.EmailOTPSentAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation($"SENT OTP TO EMAIL IS: {emailOtp}");
#if !DEBUG
            SendEmailOtp(user.Email, emailOtp); // Skipped for local test
#endif

            return SuccessVerificationResponse(179, RegistrationStatusEnum.EmailVerification, emailOtp);
        }

        public async Task<VerificationResponse> VerifyEmailOTPAsync(EmailVerificationRequest request)
        {
            var user = await GetUserById(request.UserCode);
            if (user == null) return new VerificationResponse(UserNotFoundResponse());

            if (!IsOtpValid(request.OTP, user.EmailOTPHash, user.Salt))
            {
                return InvalidOtpResponse(178, 183);
            }

            user.Status = RegistrationStatusEnum.PolicyApproval;
            user.EmailOTPHash = null;
            await _userManager.UpdateAsync(user);

            return SimpleSuccessResponse(186, RegistrationStatusEnum.PolicyApproval);
        }

        public async Task<VerificationResponse?> ApprovePolicyAsync(PolicyApprovalRequest request)
        {
            var user = await GetUserById(request.UserCode);
            if (user == null) return new VerificationResponse(UserNotFoundResponse());

            user.Status = RegistrationStatusEnum.PINSetup;
            await _userManager.UpdateAsync(user);

            return SimpleSuccessResponse(187, RegistrationStatusEnum.PINSetup);
        }

        public async Task<VerificationResponse> SetupPINAsync(PinSetupRequest request)
        {
            var user = await GetUserById(request.UserCode);
            if (user == null) return new VerificationResponse(UserNotFoundResponse());

            _userManager.AddPasswordAsync(user, request.PIN).Wait();

            user.Status = RegistrationStatusEnum.BiometricSetup;
            await _userManager.UpdateAsync(user);

            return SimpleSuccessResponse(188, RegistrationStatusEnum.BiometricSetup);
        }

        public async Task<VerificationResponse> SetupBiometricAsync(BiometricSetupRequest request)
        {
            var user = await GetUserById(request.UserCode);
            if (user == null) return new VerificationResponse(UserNotFoundResponse());

            user.EnableBiometric = request.EnableBiometric;
            user.Status = RegistrationStatusEnum.Completed;
            await _userManager.UpdateAsync(user);

            return SimpleSuccessResponse(189, null);
        }

        // ===== Helper Functions =====

        private Users BuildNewUser(PersonalInfoRequest request)
        {
            return new Users
            {
                UserName = request.ICNumber,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                ICNumber = request.ICNumber,
                FullName = request.FullName,
                LanguageID = _LanguageID,
                Status = RegistrationStatusEnum.PersonalInfo,
                CreatedAt = DateTime.UtcNow,
                Salt = HashingHelpers.GenerateSalt()
            };
        }

        private string GenerateOtp() => new Random().Next(1000, 9999).ToString();

        private bool IsOtpValid(string inputOtp, string? storedHash, string salt) =>
            storedHash == inputOtp.SaltHash(salt);

        private async Task<Users?> GetUserById(string encryptedUserId)
        {
            try
            {
                var id = encryptedUserId.Decrypt<long>();
                return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception)
            {

                return null;
            }
        }

        private void SendOtpViaTwilio(string phone, string otp)
        {
            //Twilio.TwilioClient.Init(_twilioOptions.Value.AccountSid, _twilioOptions.Value.AuthToken);
            //Twilio.Rest.Api.V2010.Account.MessageResource.Create(
            //    to: new Twilio.Types.PhoneNumber(phone),
            //    from: new Twilio.Types.PhoneNumber(_twilioOptions.Value.PhoneNumber),
            //    body: $"Your OTP code is: {otp}");
        }

        private PersonalInfoResponse FailedPersonalInfoResponse(int mainMsg, int popupMsg, ValidationPopupTypesEnum type) => new()
        {
            Success = false,
            Message = _memoryCache.GetWord(_LanguageID, mainMsg),
            PopUpErrors = new List<PopUpErrorsModel> {
                new PopUpErrorsModel{ Title = _memoryCache.GetWord(_LanguageID, mainMsg),
                    Message=  _memoryCache.GetWord(_LanguageID, popupMsg),
                    ValidationPopupTypesEnum = type
                }
            },
            ValidationPopupType = type
        };

        private PersonalInfoResponse FailedIdentityResultResponse(IdentityResult result)
        {
            var errors = new Dictionary<string, string>();
            foreach (var item in result.Errors)
            {
                AssignErrorsToDictionary(item.Code, errors);
            }
            return new PersonalInfoResponse
            {
                Success = false,
                Message = _memoryCache.GetWord(_LanguageID, 124),
                StatusCode = (int)HttpStatusCode.BadRequest,
                FieldErrors = errors
            };
        }

        private PersonalInfoResponse SuccessPersonalInfoResponse(Users user, RegistrationStatusEnum nextStep) => new()
        {
            Success = true,
            StatusCode = (int)HttpStatusCode.OK,
            UserCode = user.Id.Encrypt(),
            NextStep = nextStep.ToString()
        };

        private BaseResponse GeneralFailResponse(HttpStatusCode StatusCode) => new()
        {
            Success = false,
            StatusCode = (int)StatusCode,
        };

        private VerificationResponse FailedVerificationResponse(int msgId) => new()
        {
            Success = false,
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Message = _memoryCache.GetWord(_LanguageID, msgId)
        };

        private BaseResponse UserNotFoundResponse() => new()
        {
            Success = false,
            StatusCode = (int)HttpStatusCode.BadRequest,
            Message = _memoryCache.GetWord(_LanguageID, 124)
        };

        private VerificationResponse InvalidOtpResponse(int titleIDicD, int messageDicID) => new()
        {
            Success = false,
            IsVerified = false,
            StatusCode = (int)HttpStatusCode.BadRequest,
            Message = _memoryCache.GetWord(_LanguageID, titleIDicD),
            PopUpErrors = new List<PopUpErrorsModel> { 
                new PopUpErrorsModel{ Title = _memoryCache.GetWord(_LanguageID, titleIDicD),
                    Message=  _memoryCache.GetWord(_LanguageID, messageDicID),
                    ValidationPopupTypesEnum = ValidationPopupTypesEnum.SuggestEntryRetry
                }
            }
        };

        private VerificationResponse SuccessVerificationResponse(int messageId, RegistrationStatusEnum nextStep, string debugOtp) => new()
        {
            Success = true,
            IsVerified = true,
            Message = _memoryCache.GetWord(_LanguageID, messageId) + $" DEBUG ONLY  - OTP SENT  = {debugOtp}",
            NextStep = nextStep.ToString()
        };

        private VerificationResponse SimpleSuccessResponse(int messageId, RegistrationStatusEnum? nextStep) => new()
        {
            Success = true,
            IsVerified = true,
            StatusCode = (int)HttpStatusCode.OK,
            Message = _memoryCache.GetWord(_LanguageID, messageId),
            NextStep = nextStep?.ToString()
        };

        private void AssignErrorsToDictionary(string Code, Dictionary<string, string> dict)
        {
            dict["Email"] = Code switch
            {
                "DuplicateEmail" => _memoryCache.GetWord(_LanguageID, 173),
                "InvalidEmail" => _memoryCache.GetWord(_LanguageID, 174),
                "PasswordTooShort" => _memoryCache.GetWord(_LanguageID, 175),
                "PasswordRequiresDigit" => _memoryCache.GetWord(_LanguageID, 177),
                _ => _memoryCache.GetWord(_LanguageID, 182)
            };
        }

        public async Task<bool> IsICNumberExistsAsync(string icNumber)
        {
            return await _unitOfWork._userRepository.AnyAsync(u => u.ICNumber == icNumber);
        }
    }
}
