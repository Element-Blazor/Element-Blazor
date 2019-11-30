using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Button
{
    public class BButtonBase : BComponentBase
    {
        protected async Task OnButtonClickedAsync(MouseEventArgs e)
        {
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(e);
            }
            var task = Click?.Invoke(e);
            if (task != null)
            {
                await task;
            }
        }

        [Parameter]
        public string Cls { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter]
        public Func<MouseEventArgs, Task> Click { get; set; }

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
