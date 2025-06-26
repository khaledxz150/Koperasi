

namespace Models.Enums.User
{
    public class RegistrationStatusEnum
    {
        public enum RegistrationStatus
        {
            PersonalInfo = 1,
            MobileVerification = 2,
            EmailVerification = 4,
            PolicyApproval = 5,
            PINSetup = 6,
            BiometricSetup = 5,
            Completed = 6
        }
    }
}
