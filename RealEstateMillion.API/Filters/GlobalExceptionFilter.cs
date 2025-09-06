using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using RealEstateMillion.Application.Common;
using System.ComponentModel.DataAnnotations;
using RealEstateMillion.Application.Validators;

namespace RealEstateMillion.API.Filters
{
    public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var requestId = context.HttpContext.Items["RequestId"]?.ToString() ?? "Unknown";

            logger.LogError(context.Exception, "Exception in action filter - RequestId: {RequestId}", requestId);

            ApiResponse<object> response;
            int statusCode;

            switch (context.Exception)
            {
                case BusinessException businessEx:
                    statusCode = businessEx.StatusCode;
                    response = ApiResponse<object>.ErrorResponse(businessEx.Message, businessEx.StatusCode, businessEx.Errors);
                    break;
                default:
                    statusCode = 500;
                    response = ApiResponse<object>.ErrorResponse("An internal server error occurred", 500);
                    logger.LogError(context.Exception, response.Message);
                    break;
            }

            context.Result = new ObjectResult(response) { StatusCode = statusCode };
            context.ExceptionHandled = true;
        }
    }

}
