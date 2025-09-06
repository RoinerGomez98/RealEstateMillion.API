namespace RealEstateMillion.API.Middleware
{
    public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = Guid.NewGuid().ToString();
            context.Items["RequestId"] = requestId;

            logger.LogInformation("Request {RequestId}: {Method} {Path} started",
                requestId, context.Request.Method, context.Request.Path);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await next(context);
            }
            finally
            {
                stopwatch.Stop();
                logger.LogInformation("Request {RequestId}: {Method} {Path} completed in {ElapsedMs}ms with status {StatusCode}",
                    requestId, context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode);
            }
        }
    }
}
