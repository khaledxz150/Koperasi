using Infrastructure.Helpers.Attributes;

namespace Models.ViewModels.User.Request
{
    public class BiometricSetupRequest 
    {
        [CustomRequired(true, 125)]
        public string UserCode { get; set; }

        public bool EnableBiometric { get; set; } = false;

    }
}
