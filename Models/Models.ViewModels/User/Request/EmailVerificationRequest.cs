using System.ComponentModel.DataAnnotations;

using Infrastructure.Helpers.Attributes;

namespace Models.ViewModels.User.Request
{
    public class EmailVerificationRequest 
    {
        [CustomRequired(true, 125)]
        public string UserCode { get; set; }

        [CustomRequired(true, 145, min: 0, max: 0, maxLength: 4, minLength: 4, replacee: new[] { "4", "4" })]
        public string OTP { get; set; }
    }
}
