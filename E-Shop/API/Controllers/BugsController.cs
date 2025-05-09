using API.Errors;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BugsController : BaseApiController
    {
        public BugsController()
        {

        }

        [HttpGet("test-auth")]
        [Authorize]
        public ActionResult<string> GetSecretText()
        {
            return "secret data";
        }

        [HttpGet("not-found")]
        public IActionResult GetNotFoundRequest()
        {
            return NotFound(new ApiResponse(404));
        }

        [HttpGet("server-error")]
        public IActionResult GetServerError()
        {
            Product d = null;
            var f = d.Price.ToString();
            return BadRequest(new ApiResponse(500));
        }

        [HttpGet("bad-request")]
        public IActionResult GeBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("bad-request/{id}")]
        public IActionResult GetModelBadRequest(int id)
        {
            return BadRequest();
        }
    }
}
