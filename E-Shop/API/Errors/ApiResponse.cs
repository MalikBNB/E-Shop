using Microsoft.EntityFrameworkCore.Query.Internal;

namespace API.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message{ get; set; }

        public ApiResponse(int statusCode, string message = null!)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessage(statusCode);
        }

        public ApiResponse()
        {            
        }

        private string GetDefaultMessage(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad request",
                401 => "Unauthorized",
                404 => "Resource not found",
                500 => "An error occured",
                _ => null!
            };
        }
    }
}
