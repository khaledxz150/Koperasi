using Infrastructure.Helpers.Attributes;

namespace Models.ViewModels.User.Request
{
    public class LoginRequest
    {

        [CustomRequired(true, 145, min: 0, max: 0, maxLength: 10, minLength: 9, replacee: new[] { "9", "10" })]
        public string ICNumber { get; set; }
    }
}
