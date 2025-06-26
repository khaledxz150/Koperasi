using Infrastructure.Helpers.Attributes;

namespace Models.ViewModels.User.Request
{
    public class LoginRequest
    {

        [CustomRequired(true, 145, min: 0, max: 0, maxLength: 10, minLength: 10, replacee: new[] { "10", "10" })]
        public string ICNumber { get; set; }
        public string PIN { get; set; }
    }
}
