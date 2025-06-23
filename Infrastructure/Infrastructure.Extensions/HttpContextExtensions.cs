using System.Diagnostics.CodeAnalysis;

using Qimmah.Extensions;

namespace Infrastructure.Extensions;
public static class HttpContextExtensions
{
    public static bool TryGetBearerToken([NotNullWhen(true)] this HttpContext context, out string Token)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var claim = context.User.Claims.FirstOrDefault(x => x.Type == "BearerToken");

        Token = "";
        if (claim.IsNotNullOrEmpty())
        {
            Token = claim.Value;
            return true;
        }
        return false;
    }
}
