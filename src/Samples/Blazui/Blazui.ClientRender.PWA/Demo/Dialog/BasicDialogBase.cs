
using Blazui.ClientRender.PWA.Demo.Table;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component;

namespace Blazui.ClientRender.PWA.Demo.Dialog
{
    public class BasicDialogBase : BDialogBase
    {

        [Inject]
        MessageService MessageService { get; set; }
        public async Task ShowDialog(MouseEventArgs eventArgs)
        {
            var result = await DialogService.ShowDialogAsync<BasicTable>("测试窗口");
            MessageService.Show(result.ToString());
        }
        public async Task ShowPassParameterDialog(MouseEventArgs eventArgs)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("Name", "我是传过来的参数");
            var result = await DialogService.ShowDialogAsync<TestContent>("测试窗口", parameters);
            MessageService.Show(result.ToString());
        }
    }
}
