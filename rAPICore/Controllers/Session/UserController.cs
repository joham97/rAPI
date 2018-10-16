using Microsoft.AspNetCore.Mvc;
using rAPI.Services;
using System;

namespace rAPI.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        [HttpGet]
		public ActionResult Get([FromQuery] string userId)
        {
            var result = DatabaseService.Instance.GetUser(Convert.ToInt16(userId));

            if (result.success)
                return Ok(result);
            else
                return NotFound();
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
