using RealEstateMillion.Application.Common;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using RealEstateMillion.Application.Validators;

namespace RealEstateMillion.API.Middleware
{
    public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var requestId = context.Items["RequestId"]?.ToString() ?? "Unknown";

            logger.LogError(exception, "An error occurred processing request {RequestId}: {Message}",
                requestId, exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            ApiResponse<object> apiResponse;

            switch (exception)
            {
                case BusinessException businessEx:
                    response.StatusCode = businessEx.StatusCode;
                    apiResponse = ApiResponse<object>.ErrorResponse(businessEx.Message, businessEx.StatusCode, businessEx.Errors);
                    break;
                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    apiResponse = ApiResponse<object>.ErrorResponse("Unauthorized access", 401);
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var message = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                        ? exception.Message
                        : "An internal server error occurred";
                    apiResponse = ApiResponse<object>.ErrorResponse(message, 500);
                    break;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var result = JsonSerializer.Serialize(apiResponse, jsonOptions);
            await response.WriteAsync(result);
        }
    }
}
