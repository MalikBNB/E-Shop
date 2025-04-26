using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("errors/{code}")]
    [ApiExplorerSettings(IgnoreApi = true)] /* Solution for the exception: 
                                             * Ambiguous HTTP method for action - 
                                             * API.Controllers.ErrorsController.Error (API). 
                                             * Actions require an explicit HttpMethod binding for 
                                             * Swagger/OpenAPI 3.0
                                             */
    public class ErrorsController : BaseApiController
    {
        public IActionResult Error(int code)
        {
            return new ObjectResult(new ApiResponse(code));
        }
    }
}
