using Microsoft.AspNetCore.Mvc.Filters;

namespace UpdateApi.Attributes;

/// <summary>
/// https://stackoverflow.com/a/51853606
/// </summary>
public class ReadableBodyStreamAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // For ASP.NET 2.1
        // context.HttpContext.Request.EnableRewind();
        // For ASP.NET 3.1
        context.HttpContext.Request.EnableBuffering();
    }
}