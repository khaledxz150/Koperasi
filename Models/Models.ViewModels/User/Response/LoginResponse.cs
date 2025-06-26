using Models.Enums.User;
using Models.ViewModels.User.Base;

using static Models.Enums.User.RegistrationStatusEnum;

namespace Models.ViewModels.User.Response
{
    public class LoginResponse : BaseResponse
    {
        public RegistrationStatusEnum NextStep { get; set; }
        public object Data { get; set; } = new object();

        public LoginResponse()
        {
            Success = false;
            Message = string.Empty;
            StatusCode = 0;

        }
        public LoginResponse(BaseResponse baseResponse)
        {
            Success = baseResponse.Success;
            Message = baseResponse.Message;
            StatusCode = baseResponse.StatusCode;
            FieldErrors = baseResponse.FieldErrors;
            PopUpErrors = baseResponse.PopUpErrors;
        }
    }
}
