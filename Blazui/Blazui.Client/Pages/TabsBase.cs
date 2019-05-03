using Blazui.Client.Demo;
using Blazui.Client.Model;
using Blazui.Component.Container;
using Blazui.Component.EventArgs;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blazui.Client.Pages
{
    public class TabsBase : PageBase
    {
        protected IList<DemoModel> demos;

        [Inject]
        private HttpClient httpClient { get; set; }

        [Inject]
        private IUriHelper UriHelper { get; set; }

        protected override async Task OnInitAsync()
        {
            var router = UriHelper.GetAbsoluteUri().Split('/').LastOrDefault();
            var uri = $"api/sample/{router}";
            demos = await httpClient.GetJsonAsync<IList<DemoModel>>(uri);
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
