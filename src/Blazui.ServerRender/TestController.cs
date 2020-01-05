using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender
{
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [Route("test")]
        [HttpGet]
        public IActionResult Test()
        {
            return Content("ok");
        }
        [Route("upload")]
        [HttpPost]
        public async Task<IActionResult> UploadAsync([FromForm]IFormFile fileContent)
        {
            await Task.Delay(new Random().Next(1000));
            var ms = new MemoryStream();
            using (ms)
            {
                fileContent.CopyTo(ms);
                return Content(JsonConvert.SerializeObject(new
                {
                    code = 0,
                    id = Guid.NewGuid().ToString(),
                    url = $"data:image;base64,{Convert.ToBase64String(ms.ToArray())}"
                }), "application/json");
            }
        }
    }
}
