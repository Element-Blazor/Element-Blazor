using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            await Task.Delay(1000);
            return Content("ok");
        }
    }
}
