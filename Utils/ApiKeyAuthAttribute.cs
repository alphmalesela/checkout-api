using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var dbContext = httpContext.RequestServices.GetService<CheckoutContext>();

        var path = httpContext.Request.Path.Value?.ToLower();

        // Skip authentication for public endpoints
        if (path!.StartsWith("/api/users"))
        {
            await next();
            return;
        }

        // Require API key for protected endpoints
        if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "API Key is missing" });
            return;
        }

        var apiKey = authHeader.ToString().Replace("Bearer ", "").Trim();
        var user = await dbContext!.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);

        if (user == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        // Store the user object in HttpContext.Items
        httpContext.Items["User"] = user;
        await next();
    }
}
