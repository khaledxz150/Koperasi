
using Models.ViewModels.User.Base;

namespace Models.ViewModels.User.Response
{
    public class VerificationResponse : BaseResponse
    {
        public bool IsVerified { get; set; }
        public string? NextStep { get; set; } = null;
        public DateTime? ExpiresAt { get; set; }

        public VerificationResponse()
        {
            Success = true;
            Message = string.Empty;
            StatusCode = 0;
            Data = new object();
        }
        public VerificationResponse(BaseResponse baseResponse)
        {
            Success = baseResponse.Success;
            Message = baseResponse.Message;
            StatusCode = baseResponse.StatusCode;
            FieldErrors = baseResponse.FieldErrors;
            PopUpErrors = baseResponse.PopUpErrors;
        }
    }

    public class VerificationResponse<T> : BaseResponse<T> 
    {
        public bool IsVerified { get; set; }
        public string? NextStep { get; set; } = null;
        public DateTime? ExpiresAt { get; set; }

        public VerificationResponse()
        {
            Success = true;
            Message = string.Empty;
            StatusCode = 0;
            Data = default;
        }
        public VerificationResponse(BaseResponse baseResponse)
        {
            Success = baseResponse.Success;
            Message = baseResponse.Message;
            StatusCode = baseResponse.StatusCode;
            FieldErrors = baseResponse.FieldErrors;
            PopUpErrors = baseResponse.PopUpErrors;
            Data = default;
        }
    }
}
