

using Blazui.ClientRender.PWA.Demo.Table;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Element;

namespace Blazui.ClientRender.PWA.Demo.Loading
{
    public class BasicLoadingBase : ComponentBase
    {
        protected List<TestData> Datas = new List<TestData>();

        [Inject]
        private LoadingService LoadingService { get; set; }
        protected IContainerComponent table;
        protected override void OnInitialized()
        {
            Datas.Add(new TestData()
            {
                Address = "地址1",
                Name = "张三",
                Time = DateTime.Now
            });
            Datas.Add(new TestData()
            {
                Address = "地址2",
                Name = "张三1",
                Time = DateTime.Now
            });
            Datas.Add(new TestData()
            {
                Address = "地址3",
                Name = "张三3",
                Time = DateTime.Now
            });
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            await table.WithLoadingAsync(async () =>
            {
                await Task.Delay(2000);
            });
        }

        protected Task RenderCompleted(object arg)
        {
            //table.Loading(LoadingService);
            return Task.CompletedTask;
        }
        protected Task CustomRenderCompleted(object arg)
        {
            //table.Loading(LoadingService, "拼命加载中", "el-icon-loading", "rgba(0, 0, 0, 0.8)");
            return Task.CompletedTask;
        }

        protected void ShowLoading(object arg)
        {
            //table.Loading(LoadingService);
        }
    }
}
