using Core.Services.User;

using Infrastructure.Helpers.Security;

using Koperasi.API.Controllers.Bases;

using Microsoft.AspNetCore.Mvc;

using Models.ViewModels.User.Request;

namespace Koperasi.API.Controllers
{
    public class RegistrationController : BaseController
    {
        private readonly IRegistrationService _registrationService;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(
            IRegistrationService registrationService,
            ILogger<RegistrationController> logger)
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




        [HttpPost("personal-info")]
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
        [HttpPost("mobile-verification")]
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

        [HttpPost("email-verification")]
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

        [HttpPost("policy-approval")]
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

        [HttpPost("pin-setup")]
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

        [HttpPost("biometric-setup")]
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
