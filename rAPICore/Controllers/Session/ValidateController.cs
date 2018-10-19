using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.Services;

namespace rAPI.Controllers
{
    [Route("api/[controller]")]
    public class ValidateController : Controller
    {
        [HttpGet]
		public ActionResult<NormalAnswer> Get([FromQuery] string sessionkey)
        {
            if (SessionService.Instance.ContainsKey(sessionkey))
                return Ok(new NormalAnswer(true, "successful", 200));
            else
                return Unauthorized();
        }
        
        [HttpPost]
		public ActionResult Post(int id)
        {
            return Forbid("Forbidden Method");
        }

        [HttpPut]
        public ActionResult Put()
        {
            return Forbid("Forbidden Method");
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            return Forbid("Forbidden Method");
        }
    }
}
