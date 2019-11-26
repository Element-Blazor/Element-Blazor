using Blazui.ServerRender.Model;
using Blazui.Component.Container;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace Blazui.ServerRender.Pages
{
    public class PageBase : ComponentBase
    {
        private IList<DemoModel> Code(string name)
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
                    Type = "Blazui.ServerRender.Demo." + item.Name,
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
                var codeFiles = Directory.EnumerateFiles(Path.Combine(location, item.Name))
                    .Where(x => item.Files.Contains(Path.GetFileName(x)))
                    .OrderBy(x => item.Files.IndexOf(Path.GetFileName(x)));
                demoModel.Type += "." + Path.GetFileNameWithoutExtension(codeFiles.FirstOrDefault());
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
        [Inject]
        private IHttpClientFactory httpClientFactory { get; set; }
        [Inject]
        protected IJSRuntime jSRuntime { get; set; }

        protected IList<DemoModel> demos;

        protected string GetCode(string code, string language)
        {
            return $"<pre lang=\"{language}\">{code}</pre>";
        }

        protected string GetName(string fileName)
        {
            return fileName.Replace(".", string.Empty);
        }

        [Inject]
        private NavigationManager NavigationManager { get; set; }
        protected override void OnInitialized()
        {
            var router = NavigationManager.Uri.Split('/').LastOrDefault();
            demos = Code(router);
            foreach (var item in demos)
            {
                item.Demo = Type.GetType(item.Type);
            }
        }

        protected async Task<bool> ActiveTabChangingAsync(ITab tab)
        {
            tab.OnRenderCompletedAsync += TabCode_OnRenderCompleteAsync;
            return await Task.FromResult(true);
        }
        protected async Task TabCode_OnRenderCompleteAsync(ITab tab)
        {
            tab.OnRenderCompletedAsync -= TabCode_OnRenderCompleteAsync;
            await jSRuntime.InvokeAsync<object>("renderHightlight", tab.TabContainer.Content);
        }
    }
}
