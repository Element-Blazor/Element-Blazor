using Blazui.Component;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazAdmin.Docs.Demo.Dialog
{
    public class ExmapleDialogBase : ComponentBase
    {
        [Inject]
        DialogService DialogService { get; set; }

        [Inject]
        Blazui.Component.MessageBox MessageService { get; set; }
        public async Task ShowDialog(MouseEventArgs eventArgs)
        {
            var result = await DialogService.ShowDialogAsync<ExampleDialog>("测试窗口");
        }

        public async Task ShowMessage(MouseEventArgs eventArgs)
        {
            await MessageService.AlertAsync("消息");
        }
    }
}
