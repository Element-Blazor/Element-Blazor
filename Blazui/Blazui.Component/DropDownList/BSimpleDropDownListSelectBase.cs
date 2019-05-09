using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.DropDownList
{
    public class BSimpleDropDownListSelectBase : ComponentBase
    {
        [Parameter]
        internal string Placeholder { get; set; }
        [Parameter]
        internal EventCallback<UIMouseEventArgs> OnClick { get; set; }
        //protected async Task OnSelectClickAsync(UIMouseEventArgs e)
        //{
        //    var zIndex = ComponentManager.GenerateZIndex();
        //    content.ZIndex = zIndex;
        //    content.IsShow = !content.IsShow;
        //    await Task.CompletedTask;
        //}
    }
}
