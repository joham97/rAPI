using Microsoft.AspNetCore.Mvc;
using rAPI.DTO;
using rAPI.Services;

namespace rAPI.Controllers
{
    [Route("api/[controller]")]
    public class RegisterController : Controller
    {
        [HttpGet]
		public ActionResult Get()
        {
            return Forbid("Forbidden Method");
        }
        
        [HttpPost]
		public ActionResult Post([FromBody] Registration registration)
        {
            var result = DatabaseService.Instance.Registrieren(registration);

            if (result.success)
                return Ok(result);
            else
                return NotFound(result);
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
