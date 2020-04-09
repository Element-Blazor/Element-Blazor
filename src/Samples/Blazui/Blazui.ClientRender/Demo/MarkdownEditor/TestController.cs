using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ClientRender
{
    [Route("api/test")]
    public class Test1Controller : ControllerBase
    {
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
                    //0表示成功
                    code = 0,
                    //id为文件唯一标识符
                    id = Guid.NewGuid().ToString(),
                    //url为文件访问地址，如果上传的是图片，此url必须返回，否则不能预览，演示为了方便就直接返回了base64字符串
                    url = $"data:image;base64,{Convert.ToBase64String(ms.ToArray())}"
                }), "application/json");
            }
        }
    }
}
