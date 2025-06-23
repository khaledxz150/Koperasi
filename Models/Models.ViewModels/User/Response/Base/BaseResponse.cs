namespace Models.ViewModels.User.Response.Base
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, string> FieldErrors { get; set; } = new Dictionary<string, string>();
    }
}
