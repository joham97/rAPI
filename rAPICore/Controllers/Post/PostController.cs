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
        public ActionResult Get([FromQuery] string postId, [FromQuery] string sessionkey)
        {
            var intPostId = Convert.ToInt32(postId);

            NormalAnswer result;
            if (sessionkey != null)
            {
                if (!SessionService.Instance.ContainsKey(sessionkey))
                    return Unauthorized();

                var userId = SessionService.Instance[sessionkey].userid;
                result = DatabaseService.Instance.GetSinglePost(intPostId, userId);
            }
            else
            {
                result = DatabaseService.Instance.GetSinglePost(intPostId);
            }


            if (result.success)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult Post([FromQuery] string sessionkey, [FromBody] CreatePost createPost)
        {
            if (!SessionService.Instance.ContainsKey(sessionkey))
                return Unauthorized();

            int userId = SessionService.Instance[sessionkey].userid;
            var result = DatabaseService.Instance.CreatePost(createPost, userId);

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
        public ActionResult Delete([FromQuery] int postId, [FromQuery] string sessionkey)
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
                return NotFound();
        }
    }
}
