

using System.ComponentModel.DataAnnotations;

using Infrastructure.Helpers.Attributes;

namespace Models.ViewModels.User.Request
{
    public class MobileVerificationRequest 
    {
        [CustomRequired(true, 125)]
        public long UserID { get; set; }

        [CustomRequired(true, 145, min: 0, max: 0, maxLength: 4, minLength: 4, replacee: new[] { "4", "4" })]
        public string OTP { get; set; }
    }
}
