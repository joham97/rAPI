using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.Services;

namespace rAPI.Controllers
{
    [Route("api/[controller]")]
    public class LogoutController : Controller
    {
        [HttpGet]
		public ActionResult Get([FromQuery] string sessionkey)
        {
            SessionService.Instance.Remove(sessionkey);
            return Ok(new NormalAnswer(true, "successful", 200));
        }
        
        [HttpPost]
		public ActionResult Post()
        {
            return Forbid("Forbidden Method");
        }

        [HttpPut]
		public ActionResult Put()
        {
            return Forbid("Forbidden Method");
        }

        [HttpPut]
		public ActionResult Delete()
        {
            return Forbid("Forbidden Method");
        }
    }
}
