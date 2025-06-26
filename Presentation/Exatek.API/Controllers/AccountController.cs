using Core.Services.User;

using Infrastructure.Helpers.Security;

using Koperasi.API.Controllers.Bases;

using Microsoft.AspNetCore.Mvc;

using Models.ViewModels.User.Base;
using Models.ViewModels.User.Request;
using Models.ViewModels.User.Response;

namespace Koperasi.API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUserService _registrationService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IUserService registrationService,
            ILogger<AccountController> logger)
        {
            _registrationService = registrationService;
            _logger = logger;
        }

#if DEBUG
        [HttpGet("Decrypt")]
        public IActionResult Decrypt(string Code)
        { return Ok(Code.Decrypt<int>()); }
        [HttpGet("Encrypt")]
        public IActionResult Encrypt(long ID)
        { return Ok(ID.Encrypt()); }
#endif


        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _registrationService.LoginAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }


        [HttpPost("setup-personal-info")]
        [ProducesResponseType(typeof(PersonalInfoResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateUser([FromBody] PersonalInfoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _registrationService.CreateUserAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        [HttpPost("setup-mobile-verification")]
        [ProducesResponseType(typeof(VerificationResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyMobile([FromBody] MobileVerificationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _registrationService.VerifyMobileOTPAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("setup-email-verification")]
        [ProducesResponseType(typeof(VerificationResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyEmail([FromBody] EmailVerificationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _registrationService.VerifyEmailOTPAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("setup-policy-approval")]
        [ProducesResponseType(typeof(VerificationResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApprovePolicy([FromBody] PolicyApprovalRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _registrationService.ApprovePolicyAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("setup-pin-setup")]
        [ProducesResponseType(typeof(VerificationResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SetupPIN([FromBody] PinSetupRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _registrationService.SetupPINAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("setup-biometric-setup")]
        [ProducesResponseType(typeof(VerificationResponse<UserDataResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SetupBiometric([FromBody] BiometricSetupRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _registrationService.SetupBiometricAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
