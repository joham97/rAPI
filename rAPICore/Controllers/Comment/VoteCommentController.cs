using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.DTO;
using rAPI.Services;

namespace rAPI.Controllers
{
    [Route("api/[controller]")]
    public class VoteCommentController : Controller
    {
        [HttpGet]
		public ActionResult Get()
        {
            return Forbid("Forbidden Method");
        }

        [HttpPost]
        public ActionResult<NormalAnswer> Post([FromQuery] string sessionkey, [FromBody] CommentVote vote)
        {
            if (!SessionService.Instance.ContainsKey(sessionkey))
                return Unauthorized();

            var userId = SessionService.Instance[sessionkey].userid;

            var result = DatabaseService.Instance.VoteComment(vote, userId);

            if (result.success)
                return Ok(result);
            else
                return NotFound();
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
