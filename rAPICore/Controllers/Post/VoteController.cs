﻿using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.DTO;
using rAPI.Services;
using System.Net;

namespace rAPI.Controllers
{
    [Route("api/[controller]")]
    public class VoteController : Controller
    {
        [HttpGet]
		public ActionResult Get()
        {
            return Forbid("Forbidden Method");
        }

        [HttpPost]
        public ActionResult<NormalAnswer> Post([FromQuery] string sessionkey,  [FromBody] Vote vote)
        {
            if (!SessionService.Instance.ContainsKey(sessionkey))
                return Unauthorized();

            vote.userId = SessionService.Instance[sessionkey].userid;
            var result = DatabaseService.Instance.VotePost(vote);

            if (result.success)
                return Ok(result);
            else if (result.code == 404)
                return NotFound(result);
            else
                return Conflict(result);
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
