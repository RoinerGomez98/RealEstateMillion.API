namespace RealEstateMillion.API.Middleware
{
    public class JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var requestId = context.Items["RequestId"]?.ToString() ?? "Unknown";
                logger.LogDebug("JWT token present for request {RequestId}", requestId);
            }

            await next(context);
        }
    }
}
