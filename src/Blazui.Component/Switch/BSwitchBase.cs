using Blazui.Component.Form;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Switch
{
    public class BSwitchBase<TValue> : BFieldComponentBase<TValue>
    {
        [Parameter]
        public TValue ActiveValue { get; set; }
        [Parameter]
        public TValue InactiveValue { get; set; }

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
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnChanged { get; set; }

        public event Func<MouseEventArgs, Task> OnChangedAsync;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SetFieldValue(Value);
        }
        protected async Task OnInternalSwitchChangedAsync(MouseEventArgs e)
        {
            if (IsDisabled)
            {
                return;
            }
            if (TypeHelper.Equal(Value, InactiveValue))
            {
                Value = ActiveValue;
            }
            else
            {
                Value = InactiveValue;
            }
            SetFieldValue(Value);
            if (OnChanged.HasDelegate)
            {
                await OnChanged.InvokeAsync(e);
            }
            if (OnChangedAsync != null)
            {
                await OnChangedAsync(e);
            }
        }

        protected override void FormItem_OnReset()
        {
            Value = InactiveValue;
        }
    }
}
