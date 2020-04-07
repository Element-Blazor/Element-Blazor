

using Blazui.ServerRender.Demo.Table;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component;

namespace Blazui.ServerRender.Demo.Loading
{
    public class CustomLoadingBase : ComponentBase
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
            }, "拼命加载中", "el-icon-loading", "rgba(0, 0, 0, 0.8)");
        }
    }
}
