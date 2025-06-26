using Models.Enums.System;

namespace Models.ViewModels.User.Base
{
    public class BaseResponse<T> 
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, string> FieldErrors { get; set; } = new Dictionary<string, string>();
        public List<PopUpErrorsModel> PopUpErrors { get; set; }  = new List<PopUpErrorsModel>();
        public T Data { get; set; } = default;
    }

    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, string> FieldErrors { get; set; } = new Dictionary<string, string>();
        public List<PopUpErrorsModel> PopUpErrors { get; set; } = new List<PopUpErrorsModel>();
        public object Data { get; set; } = new object();
    }
}
