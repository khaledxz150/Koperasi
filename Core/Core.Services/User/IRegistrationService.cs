using Models.ViewModels.User.Request;
using Models.ViewModels.User.Response;

namespace Core.Services.User
{
    public interface IRegistrationService
    {
        Task<PersonalInfoResponse> CreateUserAsync(PersonalInfoRequest request);

        Task<VerificationResponse?> ApprovePolicyAsync(PolicyApprovalRequest request);
        Task<VerificationResponse> SetupBiometricAsync(BiometricSetupRequest request);
        Task<VerificationResponse> SetupPINAsync(PinSetupRequest request);
        Task<VerificationResponse> VerifyEmailOTPAsync(EmailVerificationRequest request);
        Task<VerificationResponse> VerifyMobileOTPAsync(MobileVerificationRequest request);
    }
}
