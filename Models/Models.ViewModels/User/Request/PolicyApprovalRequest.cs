using Infrastructure.Helpers.Attributes;

namespace Models.ViewModels.User.Request
{
    public class PolicyApprovalRequest
    {
        [CustomRequired(true, 125)]
        public string UserCode { get; set; }

        [CustomRequired(true, 125)]
        public bool AcceptTerms { get; set; }

        [CustomRequired(true, 125)]
        public bool AcceptPrivacy { get; set; }
    }
}
