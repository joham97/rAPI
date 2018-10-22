using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.DTO;
using rAPI.Services;
using System;
using System.Net;

namespace rAPI.Controllers
{
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        [HttpGet]
        public ActionResult<NormalAnswer> Get([FromQuery] string sessionkey, [FromQuery] int postId)
        {
            NormalAnswer result;
            if (sessionkey != null)
            {
                if (!SessionService.Instance.ContainsKey(sessionkey))
                    return Unauthorized();

                var userId = SessionService.Instance[sessionkey].userid;
                result = DatabaseService.Instance.GetSinglePost(postId, userId);
            }
            else
            {
                result = DatabaseService.Instance.GetSinglePost(postId);
            }


            if (result.success)
                return Ok(result);
            else if (result.code == 404)
                return NotFound(result);
            else 
                return Conflict(result);
        }

        [HttpPost]
        public ActionResult<NormalAnswer> Post([FromQuery] string sessionkey, [FromBody] CreatePost createPost)
        {
            if (!SessionService.Instance.ContainsKey(sessionkey))
                return Unauthorized();

            int userId = SessionService.Instance[sessionkey].userid;
            var result = DatabaseService.Instance.CreatePost(createPost, userId);

            if (result.success)
                return Ok(result);
            else
                return Conflict(result);
        }

        [HttpPut]
        public ActionResult Put()
        {
            return Forbid("Forbidden Method");
        }

        [HttpDelete]
        public ActionResult<NormalAnswer> Delete([FromQuery] string sessionkey, [FromQuery] int postId)
        {
            if (!SessionService.Instance.ContainsKey(sessionkey))
                return Unauthorized();

            var userId = SessionService.Instance[sessionkey].userid;
            if (!DatabaseService.Instance.IsUserOwnerOfPost(postId, userId))
                return Unauthorized();

            var result = DatabaseService.Instance.DeletePost(postId);

            if (result.success)
                return Ok(result);
            else
                return Conflict(result);
        }
    }
}
