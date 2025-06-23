using System.ComponentModel.DataAnnotations;

using Infrastructure.Helpers.Attributes;

namespace Models.ViewModels.User
{
    public class PersonalInfoRequest
    {
        [CustomRequired(true, 145, min:0,max:0,maxLength:10,minLength:10)]
        public string ICNumber { get; set; }

        [CustomRequired(true, 125)]
        public string FullName { get; set; }

        [CustomRequired(true, 125)]
        [CustomEmailAddress(146)]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Range(1, 2)]
        public int LanguageID { get; set; }
    }
}
