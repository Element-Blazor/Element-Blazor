using Blazui.ClientRender.PWA.Model;

using Blazui.Component;
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
using System.Text;

namespace Blazui.ClientRender.PWA.Pages
{
    internal class RouteService
    {
        private static Dictionary<string, Type> routeMap = new Dictionary<string, Type>();
        static RouteService()
        {
            var pageTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.DefinedTypes)
                .Where(x => typeof(ComponentBase).IsAssignableFrom(x))
                .ToList();
            var pageTemplates = pageTypes.Select(x => new
            {
                Routes = x.GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>().Select(y => y.Template).ToArray(),
                Component = x
            }).ToArray();
            foreach (var pageTemplate in pageTemplates)
            {
                foreach (var route in pageTemplate.Routes)
                {
                    routeMap.Add(route, pageTemplate.Component);
                }
            }
        }
        internal static Type GetComponent(string path)
        {
            routeMap.TryGetValue(path, out var component);
            return component;
        }
    }
    public class PageBase : ComponentBase
    {
        private IList<DemoModel> Code(string router)
        {
            System.Reflection.Assembly Asmb = System.Reflection.Assembly.GetExecutingAssembly();
            string strName = Asmb.GetName().Name + ".Blazui.ClientRender.PWA.xml";
            System.IO.Stream ManifestStream = Asmb.GetManifestResourceStream("Blazui.ClientRender.PWA.Demo.demos.json");

            byte[] streamData = new byte[ManifestStream.Length];
            ManifestStream.Read(streamData, 0, (int)ManifestStream.Length);
            ManifestStream.Dispose();
            var demoFile = Encoding.UTF8.GetString(streamData);
            Console.WriteLine("file:" + demoFile);
            var demoInfos = JsonConvert.DeserializeObject<IEnumerable<DemoPageModel>>(demoFile);
//            var demoInfos = JsonConvert.DeserializeObject<IEnumerable<DemoPageModel>>(@"[
//  {
//                ""name"": ""markdowneditor"",
//    ""demos"": [
//      {
//        ""title"": ""基础用法"",
//        ""name"": ""MarkdownEditor"",
//        ""files"": [
//          ""BasicEditor.razor"",
//          ""TestController.cs""
//         ]
//      },
//  {
//    ""title"": ""表单提交"",
//    ""name"":""MarkdownEditor"",
// ""files"":[
//      ""FormMarkdown.razor"",
//      ""FormMarkdownBase.cs"",
//      ""ArticleModel.cs""
//    ]
//       },
//  {
//    ""title"": ""Markdown 显示"",
//    ""name"": ""MarkdownEditor"",
//    ""files"": [
//      ""Markdown.razor""
//    ]
//   }
//     ]
//  },
//  {
//    ""name"": ""transfer"",
//    ""demos"": [
//      {
//        ""title"": ""基础用法"",
//        ""name"": ""Transfer"",
//        ""files"": [
//          ""BasicTransfer.razor""
//        ]
//      },
//      {
//        ""title"": ""表单提交"",
//        ""name"": ""Transfer"",
//        ""files"": [
//          ""FormTransfer.razor"",
//          ""FormTransferBase.cs"",
//          ""TransferModel.cs""
//        ]
//      }
//    ]
//  }
//]");
            throw new Exception(demoInfos.ToString());
            var demoInfo = new object();// demoInfos.SingleOrDefault(x => x.Name == "");
            if (demoInfo == null)
            {
                return new List<DemoModel>();
            }
            var demos = new List<DemoModel>();
            //foreach (var item in demoInfo.Demos)
            //{
            //    var demoModel = new DemoModel()
            //    {
            //        Type = "Blazui.ClientRender.PWA.Demo." + item.Name,
            //        Title = item.Title
            //    };
            //    var location = Path.GetDirectoryName(Asmb.Location);
            //    var razorPath = Path.Combine(location, "Blazui.ClientRender.PWA.Demo", item.Name + ".razor");
            //    throw new Exception(razorPath);
            //    if (System.IO.File.Exists(razorPath))
            //    {
            //        var code = System.IO.File.ReadAllText(razorPath);
            //        demoModel.Options.Add(new TabOption()
            //        {
            //            Content = GetCode(WebUtility.HtmlEncode(code), "razor"),
            //            Name = item.Name,
            //            Title = item.Name + ".razor",
            //            OnRenderCompletedAsync = TabCode_OnRenderCompleteAsync
            //        });
            //        demos.Add(demoModel);
            //        continue;
            //    }
            //    var codeFiles = Directory.EnumerateFiles(Path.Combine(location, item.Name))
            //        .Where(x => item.Files.Contains(Path.GetFileName(x)))
            //        .OrderBy(x => item.Files.IndexOf(Path.GetFileName(x)));
            //    demoModel.Type += "." + Path.GetFileNameWithoutExtension(codeFiles.FirstOrDefault());
            //    foreach (var codeFile in codeFiles)
            //    {
            //        var extension = codeFile.Split('.').LastOrDefault().ToLower();
            //        var language = extension;
            //        var code = System.IO.File.ReadAllText(codeFile);
            //        switch (extension)
            //        {
            //            case "razor":
            //                break;
            //            case "css":
            //                break;
            //            case "cs":
            //                language = "csharp";
            //                break;
            //        }
            //        demoModel.Options.Add(new TabOption()
            //        {
            //            Content = GetCode(WebUtility.HtmlEncode(code), language),
            //            Title = Path.GetFileName(codeFile),
            //            Name = Path.GetFileName(codeFile),
            //            OnRenderCompletedAsync = TabCode_OnRenderCompleteAsync
            //        });
            //    }
            //    demos.Add(demoModel);
            //}
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
            var comopnentType = RouteService.GetComponent(router);
            demos = Code(router);
            foreach (var item in demos)
            {
                item.Demo = RouteService.GetComponent(router);
            }
        }

        protected async Task TabCode_OnRenderCompleteAsync(object tab)
        {
            await jSRuntime.InvokeVoidAsync("renderHightlight", ((BTabPanelBase)tab).TabContainer.Content);
        }
    }
}
