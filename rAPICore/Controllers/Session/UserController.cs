using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.Services;
using System;

namespace rAPI.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        [HttpGet]
		public ActionResult<NormalAnswer> Get([FromQuery] int userId)
        {
            var result = DatabaseService.Instance.GetUser(userId);

            if (result.success)
                return Ok(result);
            else if (result.code == 404)
                return NotFound(result);
            else
                return Conflict(result);
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

        [HttpDelete]
        public ActionResult Delete()
        {
            return Forbid("Forbidden Method");
        }
    }
}
