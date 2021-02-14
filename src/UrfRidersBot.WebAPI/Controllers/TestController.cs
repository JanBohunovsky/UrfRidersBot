using Microsoft.AspNetCore.Mvc;

namespace UrfRidersBot.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Test()
        {
            return Ok();
        }
    }
}