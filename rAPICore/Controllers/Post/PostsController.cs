using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.Services;

namespace rAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class PostsController : Controller
    {

        [HttpGet]
        public ActionResult Get([FromQuery] string type, [FromQuery] string sessionkey)
        {

            if (!type.Equals("hot") && !type.Equals("new"))
                return BadRequest(new NormalAnswer(false, "type should be 'hot' or 'new'", 401));
            var isHot = type.Equals("hot");

            NormalAnswer result;
            if (sessionkey != null)
            {
                if (!SessionService.Instance.ContainsKey(sessionkey))
                    return Unauthorized();

                var userId = SessionService.Instance[sessionkey].userid;
                result = DatabaseService.Instance.GetPostAndVote(isHot, userId);
            }
            else
            {
                result = DatabaseService.Instance.GetPost(isHot);
            }

            if (result.success)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult Posts()
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
