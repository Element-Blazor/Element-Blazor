using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Switch
{
    public class BSwitchBase : ComponentBase
    {
        [Parameter]
        public string ActiveValue { get; set; } = "true";
        [Parameter]
        public string InactiveValue { get; set; } = "false";

        [Parameter]
        public bool IsDisabled { get; set; }
        [Parameter]
        public string ActiveText { get; set; }

        [Parameter]
        public string InactiveText { get; set; }

        [Parameter]
        public string ActiveColor { get; set; } = "#409EFF";

        [Parameter]
        public string InactiveColor { get; set; } = "#C0CCDA";
        [Parameter]
        public string Value { get; set; } = "true";

        [Parameter]
        public EventCallback<UIMouseEventArgs> OnChanged { get; set; }

        public event Func<UIMouseEventArgs, Task> OnChangedAsync;
        protected async Task OnInternalSwitchChangedAsync(UIMouseEventArgs e)
        {
            if (IsDisabled)
            {
                return;
            }
            if (Value == InactiveValue)
            {
                Value = ActiveValue;
            }
            else
            {
                Value = InactiveValue;
            }
            if (OnChanged.HasDelegate)
            {
                await OnChanged.InvokeAsync(e);
            }
            if (OnChangedAsync != null)
            {
                await OnChangedAsync(e);
            }
        }
    }
}
