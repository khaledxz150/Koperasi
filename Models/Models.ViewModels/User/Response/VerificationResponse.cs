
using Models.ViewModels.User.Response.Base;

namespace Models.ViewModels.User.Response
{
    public class VerificationResponse : BaseResponse
    {
        public bool IsVerified { get; set; }
        public string NextStep { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
