namespace RealEstateMillion.API.Middleware
{
    public class PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            await next(context);

            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > 2000)
            {
                var requestId = context.Items["RequestId"]?.ToString() ?? "Unknown";
                logger.LogWarning("Slow request detected - RequestId: {RequestId}, Method: {Method}, Path: {Path}, Duration: {Duration}ms",
                    requestId, context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
