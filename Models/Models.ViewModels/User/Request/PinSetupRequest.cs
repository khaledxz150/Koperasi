using Infrastructure.Helpers.Attributes;

namespace Models.ViewModels.User.Request
{
    public class PinSetupRequest 
    {
        [CustomRequired(true, 125)]
        public long UserID { get; set; }

        [CustomRequired(true, 145, min: 0, max: 0, maxLength: 6, minLength:6, replacee: new[] { "6", "6" })]
        public string PIN { get; set; }
    }
}
