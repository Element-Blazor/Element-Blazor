using Blazui.Component;
using Blazui.ServerRender.Demo.Table;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.Dialog
{
    public class BasicDialogBase : ComponentBase
    {
        [Inject]
        DialogService DialogService { get; set; }

        [Inject]
        MessageService MessageService { get; set; }
        public async Task ShowDialog(MouseEventArgs eventArgs)
        {
            var result = await DialogService.ShowDialogAsync<BasicTable>("测试窗口");
            MessageService.Show(result.ToString());
        }
    }
}
