using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionMiddleware(RequestDelegate next, 
                                   ILogger<ExceptionMiddleware> logger, 
                                   IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // If there is no exception move next
            }
            catch (Exception ex)
            {
                // If there is an exception do the following:

                // 1- Log it
                _logger.LogError(ex, ex.Message);

                // 2- Specify the context response type
                context.Response.ContentType = "application/json";

                // 3- set the context Status Code to InternalServerError
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _environment.IsDevelopment()
                  ? new ApiException((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                  : new ApiException((int)HttpStatusCode.InternalServerError, ex.Message);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
