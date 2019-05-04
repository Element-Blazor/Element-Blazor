using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Button
{
    public class BButtonBase : ComponentBase
    {
        protected async Task OnButtonClickedAsync(UIMouseEventArgs e)
        {
            if (!OnClick.HasDelegate)
            {
                return;
            }
            await OnClick.InvokeAsync(e);
        }
        [Parameter]
        public EventCallback<UIMouseEventArgs> OnClick { get; set; }

        [Parameter]
        public ButtonType Type { get; set; } = ButtonType.Default;

        [Parameter]
        public bool IsPlain { get; set; }

        [Parameter]
        public bool IsRound { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool IsCircle { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        private ButtonSize Size { get; set; }

        [Parameter]
        public string Icon { get; set; }

        protected override async Task OnInitAsync()
        {
            //if (Theme == ButtonTheme.Disabled)
            //{
            //    disabled = true;
            //}
            //if (IsFluid)
            //{
            //    fluid = "layui-btn-fluid";
            //}
            //switch (Size)
            //{
            //    case ButtonSize.Default:
            //        size = string.Empty;
            //        break;
            //    case ButtonSize.Large:
            //        size = "layui-btn-lg";
            //        break;
            //    case ButtonSize.Small:
            //        size = "layui-btn-sm";
            //        break;
            //    case ButtonSize.XSmall:
            //        size = "layui-btn-xs";
            //        break;
            //}
            //if (IsRadius)
            //{
            //    radius = "layui-btn-radius";
            //}
            await Task.CompletedTask;
        }
    }
}
