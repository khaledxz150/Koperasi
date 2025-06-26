using Microsoft.AspNetCore.Http;

namespace Infrastructure.Helpers.HttpContext
{
    public static class HTTPContextHelper
    {
        public  static int GetCurrentLanguageFromHeader()
        {
           new HttpContextAccessor().HttpContext.Request.Headers.TryGetValue("LanguageID", out var languageId);
           return int.TryParse(languageId, out var langId) ? langId : 1;
        }
    }
}
