
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Element;

namespace Blazui.ServerRender.Demo.Dialog
{
    public class NestDialogBase : BDialogBase
    {
        public async Task ShowDialog(MouseEventArgs eventArgs)
        {
            var result = await DialogService.ShowDialogAsync<ExampleDialog>("测试窗口");
        }
    }
}
