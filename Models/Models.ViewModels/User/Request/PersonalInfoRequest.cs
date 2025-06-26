using System.ComponentModel.DataAnnotations;

using Infrastructure.Helpers.Attributes;

namespace Models.ViewModels.User.Request
{
    public class PersonalInfoRequest
    {
        [CustomRequired(true, 145, min: 0, max: 0, maxLength: 9, minLength: 10, replacee: new [] {"9", "10"})]
        public string ICNumber { get; set; }

        [CustomRequired(true, 125)]
        public string FullName { get; set; }

        [CustomRequired(true, 125)]
        [CustomEmailAddress(146)]
        public string Email { get; set; }

        [CustomRequired(true, 125)]
        [CustomPhoneNumber(147)]
        public string PhoneNumber { get; set; }
    }
}
