using Blazui.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Server.Controllers
{
    [Route("api/[controller]")]
    public class SampleController : Controller
    {
        [HttpGet("[action]/{name}")]
        public string Code(string name)
        {
            var location = Path.GetDirectoryName(typeof(Startup).Assembly.Location);
            var razorPath = Path.Combine(location, "Demo", name +".razor");

            var code = System.IO.File.Exists(razorPath)
                       ?System.IO.File.ReadAllText(razorPath)
                       :string.Empty;
            return code;
        }
    }
}
