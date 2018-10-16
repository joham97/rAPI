

using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.DTO;
using System;
using System.IO;
using System.Net.Http.Headers;

namespace rAPI.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {

        [HttpGet]
		public ActionResult Get()
        {
            return Forbid("Forbidden Method");
        }

        [HttpPost, DisableRequestSizeLimit]
        public ActionResult Post()
        {
            try
            {
                var file = Request.Form.Files[0];
                string folderName = @"C:\xampp\htdocs\assets\";
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                if (file.Length > 0)
                {
                    string[] parts = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"').Split(".");
                    string extension = parts[parts.Length - 1];
                    string fileName = DateTime.Now.ToString("dd.MM.yy-hh.mm.ss.fff") + "." + extension;
                    string fullPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new ComplexAnswer(true, "successful", 200, new Upload("/assets/" + fileName)));
                }
                return Conflict("File length: " + file.Length);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
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
