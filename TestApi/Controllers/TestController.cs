using Microsoft.AspNetCore.Mvc;
using TestApi.Models;

namespace TestApi.Controllers
{
    /// <summary>
    /// Test controller
    /// </summary>
    [ApiController]
    [Route("v1/[controller]")]
    public class TestController : Controller
    {
        /// <summary>
        /// Return time
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult<TimeDto> GetTime()
        {
            return Ok(new TimeDto { Time = TimeOnly.FromDateTime(DateTime.Now)});
        }
    }
}
