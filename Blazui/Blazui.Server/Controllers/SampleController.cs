using Blazui.Client.Model;
using Blazui.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Blazui.Server.Controllers
{
    [Route("api/[controller]")]
    public class SampleController : Controller
    {
        [HttpGet("{name}")]
        public IList<DemoModel> Code(string name)
        {
            var location = Path.Combine(Path.GetDirectoryName(typeof(Startup).Assembly.Location), "Demo");
            var demoInfos = JsonConvert.DeserializeObject<IEnumerable<DemoPageModel>>(System.IO.File.ReadAllText(Path.Combine(location, "demos.json")));
            var demoInfo = demoInfos.SingleOrDefault(x => x.Name == name);
            if (demoInfo == null)
            {
                return new List<DemoModel>();
            }
            var demos = new List<DemoModel>();
            foreach (var item in demoInfo.Demos)
            {
                var razorPath = Path.Combine(location, item.Name + ".razor");
                var demoModel = new DemoModel()
                {
                    Type = "Blazui.Client.Demo." + item.Name,
                    Title = item.Title
                };
                if (System.IO.File.Exists(razorPath))
                {
                    var code = System.IO.File.ReadAllText(razorPath);
                    demoModel.Codes.Add(new CodeModel()
                    {
                        Code = WebUtility.HtmlEncode(code),
                        FileName = item.Name + ".razor",
                        Language = "razor"
                    });
                    demos.Add(demoModel);
                    continue;
                }
                demoModel.Type += "." + item.Name;
                var codeFiles = Directory.EnumerateFiles(Path.Combine(location, item.Name))
                    .OrderBy(x => item.Files.IndexOf(Path.GetFileName(x)));
                foreach (var codeFile in codeFiles)
                {
                    var extension = codeFile.Split('.').LastOrDefault().ToLower();
                    var language = extension;
                    var code = System.IO.File.ReadAllText(codeFile);
                    switch (extension)
                    {
                        case "razor":
                            break;
                        case "css":
                            break;
                        case "cs":
                            language = "csharp";
                            break;
                    }
                    demoModel.Codes.Add(new CodeModel()
                    {
                        Code = WebUtility.HtmlEncode(code),
                        FileName = Path.GetFileName(codeFile),
                        Language = language
                    });
                }
                demos.Add(demoModel);
            }
            return demos;
        }
    }
}
