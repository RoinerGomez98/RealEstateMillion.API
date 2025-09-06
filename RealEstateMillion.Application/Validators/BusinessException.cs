namespace RealEstateMillion.Application.Validators
{
    public class BusinessException(string message, int statusCode = 400, List<string>? errors = null) : Exception(message)
    {
        public int StatusCode { get; } = statusCode;
        public List<string>? Errors { get; } = errors;
    }
    public class NotFoundException(string message) : BusinessException(message, 404)
    {
    }

    public class ValidationException(string message, List<string>? errors = null) : BusinessException(message, 400, errors)
    {
    }

    public class ConflictException(string message) : BusinessException(message, 409)
    {
    }

}
