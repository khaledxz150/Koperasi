using System.Net;
using System.Net.Mail;

using Application.UnitOfWork.Repos;

using Core.Services.User;
using Core.UnitOfWork;

using Infrastructure.Data;
using Infrastructure.Extensions;
using Infrastructure.Helpers.HttpContext;
using Infrastructure.Helpers.Security;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Models.Entities.User;
using Models.Enums.System;
using Models.ViewModels.Options;
using Models.ViewModels.User.Request;
using Models.ViewModels.User.Response;

using static Models.Enums.User.RegistrationStatusEnum;

namespace Application.Services.User
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Users> _userManager;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<RegistrationService> _logger;
        private readonly int _LanguageID;
        private readonly IOptions<TwilioOptions> _twilioOptions;
        private readonly IOptions<SmtpOptions> _smtpOptions;

        public RegistrationService(
            UserManager<Users> userManager,
            IMemoryCache memoryCache,
            ILogger<RegistrationService> logger,
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
        public async Task<PersonalInfoResponse> CreateUserAsync(PersonalInfoRequest request)
        {
            try
            {
                // Check if IC number already exists
                if (await IsICNumberExistsAsync(request.ICNumber))
                {
                    return new PersonalInfoResponse
                    {
                        Success = false,
                        Message = _memoryCache.GetWord(_LanguageID,127),
                        PopUpErrors = new Dictionary<string, string> {
                            { _memoryCache.GetWord(_LanguageID, 127), _memoryCache.GetWord(_LanguageID, 184) }
                        },
                        ValidationPopupType = ValidationPopupTypesEnum.SuggestLoginAndRetry
                    };
                }

                // Create new user
                var user = new Users
                {
                    UserName = request.ICNumber,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    ICNumber = request.ICNumber,
                    FullName = request.FullName,
                    LanguageID = _LanguageID,
                    Status = RegistrationStatus.PersonalInfo,
                    CreatedAt = DateTime.UtcNow,
                    Salt = Infrastructure.Helpers.Security.HashingHelpers.GenerateSalt()
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    var DictionaryErrors = new Dictionary<string, string>();

                    foreach (var item in result.Errors)
                    {
                        AssignErrorsToDictionary(item.Code, DictionaryErrors);
                    }
                    return new PersonalInfoResponse
                    {
                        Success = false,
                        Message = _memoryCache.GetWord(_LanguageID,124),
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        FieldErrors = DictionaryErrors
                    };
                }

                // Move to next step
                user.Status = RegistrationStatus.MobileVerification;
                user.MobileOTPHash = SendOtp(user.PhoneNumber).SaltHash(user.Salt);
                await _userManager.UpdateAsync(user);

                // Send mobile OTP
                //await SendMobileOTPAsync(user, request.Culture);


                return new PersonalInfoResponse
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    UserCode = user.Id.Encrypt(),
                    NextStep = RegistrationStatus.MobileVerification.ToString(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return new PersonalInfoResponse
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = _memoryCache.GetWord(_LanguageID, 124),
                };
            }
        }




        public async Task<VerificationResponse> VerifyMobileOTPAsync(MobileVerificationRequest request)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserCode.Decrypt<long>());
                if (user == null)
                {
                    return new VerificationResponse
                    {
                        Success = false,
                        IsVerified = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = _memoryCache.GetWord(_LanguageID, 124), // "User not found" or general error
                    };
                }

                // Hash the provided OTP with user's salt and compare
                var providedOtpHash = request.OTP.SaltHash(user.Salt);
                if (user.MobileOTPHash != providedOtpHash)
                {
                    return new VerificationResponse
                    {
                        Success = false,
                        IsVerified = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = _memoryCache.GetWord(_LanguageID, 178),
                        PopUpErrors = new Dictionary<string, string> {
                            { _memoryCache.GetWord(_LanguageID, 178), _memoryCache.GetWord(_LanguageID, 183)}
                        }
                    };
                }
               var sentOTP =  SendEmailOtp(user.Email);

                _logger.Log(LogLevel.Information,$"SENT OTP TO EMAIL IS: {sentOTP}");

                user.Status = RegistrationStatus.EmailVerification;
                user.EmailOTPHash = sentOTP.SaltHash(user.Salt);
                user.EmailOTPSentAt = DateTime.Now;
                //user.MobileOTPHash = null;
                //user.MobileOTPSentAt = null;
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                var message = _memoryCache.GetWord(_LanguageID, 179);

#if DEBUG
                message += $" DEBUG ONLY  - OTP SENT  = {sentOTP}";
#endif
                return new VerificationResponse
                {
                    Success = true,
                    IsVerified = true,
                    Message = message, // "Mobile verified successfully"
                    NextStep = RegistrationStatus.EmailVerification.ToString(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying mobile OTP");
                return new VerificationResponse
                {
                    Success = false,
                    IsVerified = false,
                    Message = _memoryCache.GetWord(_LanguageID, 124),
                };
            }
        }

        public async Task<VerificationResponse> VerifyEmailOTPAsync(EmailVerificationRequest request)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserCode.Decrypt<long>());
                if (user == null)
                {
                    return new VerificationResponse
                    {
                        Success = false,
                        IsVerified = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = _memoryCache.GetWord(_LanguageID, 124), 
                    };
                }

                var providedOtpHash = request.OTP.SaltHash(user.Salt);
                if (user.EmailOTPHash != providedOtpHash)
                {
                    return new VerificationResponse
                    {
                        Success = false,
                        IsVerified = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = _memoryCache.GetWord(_LanguageID, 178), 
                        PopUpErrors = new Dictionary<string, string> {
                            { _memoryCache.GetWord(_LanguageID, 178), _memoryCache.GetWord(_LanguageID, 183) }
                        }
                    };
                }

                // Update user registration status and clear OTP
                user.Status = RegistrationStatus.PolicyApproval;
                user.EmailOTPHash = null;
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                return new VerificationResponse
                {
                    Success = true,
                    IsVerified = true,
                    Message = _memoryCache.GetWord(_LanguageID, 186),
                    NextStep = RegistrationStatus.PolicyApproval.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email OTP");
                return new VerificationResponse
                {
                    Success = false,
                    IsVerified = false,
                    Message = _memoryCache.GetWord(_LanguageID, 182), // "Unexpected error"
                };
            }
        }

        public async Task<VerificationResponse?> ApprovePolicyAsync(PolicyApprovalRequest request)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserCode.Decrypt<long>());
                if (user == null)
                {
                    return new VerificationResponse
                    {
                        Success = false,
                        IsVerified = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = _memoryCache.GetWord(_LanguageID, 124) 
                    };
                }

                user.Status = RegistrationStatus.PINSetup;
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                return new VerificationResponse
                {
                    Success = true,
                    IsVerified = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = _memoryCache.GetWord(_LanguageID, 187), 
                    NextStep = RegistrationStatus.PINSetup.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving policy");
                return new VerificationResponse
                {
                    Success = false,
                    IsVerified = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = _memoryCache.GetWord(_LanguageID, 182)
                };
            }
        }


        public async Task<VerificationResponse> SetupPINAsync(PinSetupRequest request)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserCode.Decrypt<long>());
                if (user == null)
                {
                    return new VerificationResponse
                    {
                        Success = false,
                        IsVerified = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = _memoryCache.GetWord(_LanguageID, 124) // User not found
                    };
                }

                user.PINHash = request.PIN.SaltHash(user.Salt);
                user.Status = RegistrationStatus.BiometricSetup;
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                return new VerificationResponse
                {
                    Success = true,
                    IsVerified = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = _memoryCache.GetWord(_LanguageID, 188), // "PIN saved successfully"
                    NextStep = RegistrationStatus.BiometricSetup.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up PIN");
                return new VerificationResponse
                {
                    Success = false,
                    IsVerified = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = _memoryCache.GetWord(_LanguageID, 182) // "Unexpected error"
                };
            }
        }


        public async Task<VerificationResponse> SetupBiometricAsync(BiometricSetupRequest request)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserCode.Decrypt<long>());
                if (user == null)
                {
                    return new VerificationResponse
                    {
                        Success = false,
                        IsVerified = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = _memoryCache.GetWord(_LanguageID, 124)
                    };
                }


                user.EnableBiometric = request.EnableBiometric;
                user.Status = RegistrationStatus.Completed;
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                return new VerificationResponse
                {
                    Success = true,
                    IsVerified = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = _memoryCache.GetWord(_LanguageID, 189),
                    NextStep = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finalizing biometric setup");
                return new VerificationResponse
                {
                    Success = false,
                    IsVerified = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = _memoryCache.GetWord(_LanguageID, 182)
                };
            }
        }



        public async Task<bool> IsICNumberExistsAsync(string icNumber)
        {
            return await _unitOfWork._userRepository.AnyAsync(u => u.ICNumber == icNumber);
        }

        private void AssignErrorsToDictionary(string Code, Dictionary<string, string> DictionaryErrors)
        {
            switch (Code)
            {
                case "DuplicateEmail":
                    DictionaryErrors["Email"] = _memoryCache.GetWord(_LanguageID, 173); // "البريد الإلكتروني مستخدم"
                    break;

                case "InvalidEmail":
                    DictionaryErrors["Email"] = _memoryCache.GetWord(_LanguageID, 174); // "بريد إلكتروني غير صالح"
                    break;

                case "PasswordTooShort":
                    DictionaryErrors["PIN"] = _memoryCache.GetWord(_LanguageID, 175); // "كلمة المرور قصيرة جدًا"
                    break;

                case "PasswordRequiresDigit":
                    DictionaryErrors["PIN"] = _memoryCache.GetWord(_LanguageID, 177); // "يجب أن تحتوي كلمة المرور على رقم"
                    break;

                default:
                    DictionaryErrors[""] = _memoryCache.GetWord(_LanguageID, 182); // fallback (or use GetWord with a general error)
                    break;
            }
        }
        public string SendOtp(string userPhoneNumber)
        {
            string otp = new Random().Next(1000, 9999).ToString();

            Twilio.TwilioClient.Init(_twilioOptions.Value.AccountSid, _twilioOptions.Value.AuthToken);

            var message = Twilio.Rest.Api.V2010.Account.MessageResource.Create(
                to: new Twilio.Types.PhoneNumber(userPhoneNumber),
                from: new Twilio.Types.PhoneNumber(_twilioOptions.Value.PhoneNumber), // your Twilio number
                body: $"Your OTP code is: {otp}");

            return otp;
        }
        public string SendEmailOtp(string userEmail)
        {
            try
            {
                // Generate a 4-digit OTP
                string otp = new Random().Next(1000, 9999).ToString();

                // Create the email message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpOptions.Value.FromEmail, "Koperasi"), // Optional: Add a display name
                    Subject = "Your OTP Code",
                    Body = $"Your OTP code is: {otp}",
                    IsBodyHtml = false
                };
                mailMessage.To.Add(userEmail);

                // Configure the SMTP client
                using (var smtpClient = new SmtpClient(_smtpOptions.Value.Host, _smtpOptions.Value.Port))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpOptions.Value.Username, _smtpOptions.Value.Password);
                    smtpClient.EnableSsl = true; // Use SSL/TLS for secure communication

                    // Send the email - this is not working, I concluded because umniah is blocking it for some reason? 
                    //smtpClient.Send(mailMessage);
                }

                return otp; // Return the OTP for further processing
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP error occurred while sending email OTP");
                throw new Exception("Failed to send OTP via email. Please check your SMTP configuration.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error occurred while sending email OTP");
                throw new Exception("An unexpected error occurred while sending the OTP.");
            }
        }

    }
}
