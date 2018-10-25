using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.Services;
using System;

namespace rAPI.Controllers
{
    [Route("api/[controller]")]
    public class PostsController : Controller
    {

        [HttpGet]
        public ActionResult<NormalAnswer> Get([FromQuery] string sessionkey, [FromQuery] string type, [FromQuery] string userId)
        {
            if (!type.Equals("hot") && !type.Equals("new") && (!type.Equals("user") || userId == null || !int.TryParse(userId, out int n)))
                return BadRequest();
            var isHot = type.Equals("hot");

            NormalAnswer result;
            if (sessionkey != null)
            {
                if (!SessionService.Instance.ContainsKey(sessionkey))
                    return Unauthorized();

                var requestUserId = SessionService.Instance[sessionkey].userid;
                if (type.Equals("user"))
                    result = DatabaseService.Instance.GetPostAndVoteOfUser(Convert.ToInt32(userId), requestUserId);
                else
                    result = DatabaseService.Instance.GetPostAndVote(isHot, requestUserId);
            }
            else
            {
                if (type.Equals("user"))
                    result = DatabaseService.Instance.GetPostOfUser(Convert.ToInt32(userId));
                else
                    result = DatabaseService.Instance.GetPost(isHot);
            }

            if (result.success)
                return Ok(result);
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
