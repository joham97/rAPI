using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.DTO;
using rAPI.Services;
using System;
using System.Collections.Generic;

namespace rAPI.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        [HttpGet]
		public ActionResult Get()
        {
            return Forbid("Forbidden Method");
        }

        [HttpPost]
		public ActionResult<NormalAnswer> Post([FromBody] Login login)
        {
            var result = DatabaseService.Instance.Login(login);
            if (result.success)
            {
                var loginAnswer = result as ComplexAnswer;
                var loginData = loginAnswer.data as LoginData;
                // Remove Session
                List<string> removeSessions = new List<string>();
                foreach (var a in SessionService.Instance)
                {
                    if (a.Value.userid == loginData.userId)
                    {
                        removeSessions.Add(a.Key);
                    }
                }
                foreach (var a in removeSessions)
                {
                    SessionService.Instance.Remove(a);
                }
                // Generate new Sessionkey
                string key = SessionService.GenerateSessionKey();
                while (SessionService.Instance.ContainsKey(key))
                {
                    key = SessionService.GenerateSessionKey();
                }
                SessionService.Instance.Add(key, new Session(loginData.userId, DateTime.Now.Ticks));
                loginData.sessionkey = key;
                return Ok(loginAnswer);
            }
            else
            {
                return Unauthorized();
            }
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
