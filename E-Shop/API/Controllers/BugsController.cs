using API.Errors;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BugsController : BaseApiController
    {
        public BugsController()
        {

        }

        [HttpGet("not-found")]
        public IActionResult GetNotFoundRequest()
        {
            return Ok();
        }

        [HttpGet("server-error")]
        public IActionResult GetServerError()
        {
            Product d = null;
            var f = d.Price.ToString();
            return BadRequest();
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
