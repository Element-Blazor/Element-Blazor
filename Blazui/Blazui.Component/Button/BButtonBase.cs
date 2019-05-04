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
        public ButtonSize Size { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }
    }
}
