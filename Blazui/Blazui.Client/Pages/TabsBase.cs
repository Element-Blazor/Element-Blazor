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
        protected List<DemoModel> demos { get; set; } = new List<DemoModel>();

        [Inject]
        private HttpClient httpClient { get; set; }

        protected override Task OnInitAsync()
        {
            demos.Add(new DemoModel()
            {
                Code = @"<BTab>
    <BTabPanel Title=""用户管理"">用户管理1</BTabPanel>
    <BTabPanel Title=""角色管理"">角色管理1</BTabPanel>
    <BTabPanel Title=""部门管理"">部门管理1</BTabPanel>
    <BTabPanel Title=""人员管理"">人员管理1</BTabPanel>
</BTab>",
                Title = "基础的、简洁的标签页",
                Demo = typeof(BasicTab)
            });
            demos.Add(new DemoModel()
            {
                Code = @"<BTab Type=""@TabType.Card"">
    <BTabPanel Title=""用户管理"">用户管理1</BTabPanel>
    <BTabPanel Title=""角色管理"">角色管理1</BTabPanel>
    <BTabPanel Title=""部门管理"">部门管理1</BTabPanel>
    <BTabPanel Title=""人员管理"">人员管理1</BTabPanel>
</BTab>",
                Title = "选项卡样式的标签页",
                Demo = typeof(CardTab)
            });
            demos.Add(new DemoModel()
            {
                Code = @"<BTab Type=""@TabType.BorderCard"">
    <BTabPanel Title=""用户管理"">用户管理1</BTabPanel>
    <BTabPanel Title=""角色管理"">角色管理1</BTabPanel>
    <BTabPanel Title=""部门管理"">部门管理1</BTabPanel>
    <BTabPanel Title=""人员管理"">人员管理1</BTabPanel>
</BTab>",
                Title = "卡片化的标签页",
                Demo = typeof(BorderCardTab)
            });
            demos.Add(new DemoModel()
            {
                Code = @"<BTab Type=""@TabType.BorderCard""  TabPosition=""@TabPosition.Left"">
    <BTabPanel Title=""用户管理"">用户管理1</BTabPanel>
    <BTabPanel Title=""角色管理"">角色管理1</BTabPanel>
    <BTabPanel Title=""部门管理"">部门管理1</BTabPanel>
    <BTabPanel Title=""人员管理"">人员管理1</BTabPanel>
</BTab>",
                Title = "在左边的标签页",
                Demo = typeof(LeftTab)
            });
            demos.Add(new DemoModel()
            {
                Code = @"<BTab Type=""@TabType.BorderCard""  TabPosition=""@TabPosition.Left"">
    <BTabPanel Title=""用户管理"">用户管理1</BTabPanel>
    <BTabPanel Title=""角色管理"">角色管理1</BTabPanel>
    <BTabPanel Title=""部门管理"">部门管理1</BTabPanel>
    <BTabPanel Title=""人员管理"">人员管理1</BTabPanel>
</BTab>",
                Title = "可编辑的标签页",
                Demo = typeof(EditableTab)
            });
            return base.OnInitAsync();
        }

        //protected RenderFragment RenderComponent(DemoModel context)
        //{

        //}

        protected async Task ActiveTabChanged(UIChangeEventArgs e)
        {
            var eventArgs = (ChangeEventArgs<ITab>)e;
            eventArgs.NewValue.OnRenderCompletedAsync -= TabCode_OnRenderCompleteAsync;
            eventArgs.NewValue.OnRenderCompletedAsync += TabCode_OnRenderCompleteAsync;
        }
        protected async Task TabCode_OnRenderCompleteAsync(ITab tab)
        {
            await jSRuntime.InvokeAsync<object>("renderHightlight", tab.TabContainer.Content);
        }
    }
}
