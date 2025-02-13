using Microsoft.EntityFrameworkCore;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context, CheckoutContext dbContext)
    {
        var path = context.Request.Path.Value?.ToLower();

        // Skip authentication for public endpoints
        if (path!.StartsWith("/api/users") && context.Request.Method == "POST")
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("API Key is missing.");
            return;
        }

        // Extract API key from "Bearer YOUR_API_KEY"
        var apiKey = authHeader.ToString().Replace("Bearer ", "").Trim();

        // Check if the API key exists in the database
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);

        if (user == null)
        {
            context.Response.StatusCode = 403; // Forbidden
            await context.Response.WriteAsync("Invalid API Key.");
            return;
        }

        // API Key is valid, proceed with request
        await _next(context);
    }
}
