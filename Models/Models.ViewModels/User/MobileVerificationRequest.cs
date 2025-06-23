

using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.User
{
    public class MobileVerificationRequest 
    {
        [Required]
        public long UserID { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string OTP { get; set; }
    }
}
