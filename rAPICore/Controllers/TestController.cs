using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.DTO;
using System.Net;

namespace rAPI.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
		[HttpGet]
		public ActionResult<NormalAnswer> Get()
        {
            return Ok(new NormalAnswer(true, "successful", 200));
        }
        
        [HttpPost]
		public ActionResult<NormalAnswer> Post()
        {
            return Ok(new NormalAnswer(true, "successful", 200));
        }
        
        [HttpPut]
		public ActionResult<NormalAnswer> Put()
        {
            return Ok(new NormalAnswer(true, "successful", 200));
        }
        
        [HttpDelete]
		public ActionResult<NormalAnswer> Delete()
        {
            return Ok(new NormalAnswer(true, "successful", 200));
        }
    }
}
