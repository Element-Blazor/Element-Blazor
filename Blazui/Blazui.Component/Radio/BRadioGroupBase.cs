using Blazui.Component.EventArgs;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Radio
{
    public class BRadioGroupBase : ComponentBase
    {
        [Parameter]
        public EventCallback<string> SelectedValueChanged { get; set; }

        [Parameter]
        public EventCallback<ChangeEventArgs<string>> SelectedValueChanging { get; set; }
        [Parameter]
        public string SelectedValue { get; set; }
        [Parameter]
        protected RenderFragment ChildContent { get; set; }

        public event Func<ChangeEventArgs<string>, Task<bool>> OnSelectedValueChangingAsync;

        public event Func<ChangeEventArgs<string>, Task> OnSelectedValueChangedAsync;

        internal async Task<bool> TrySetValueAsync(string value)
        {
            var arg = new ChangeEventArgs<string>()
            {
                NewValue = value,
                OldValue = SelectedValue
            };
            if (SelectedValueChanging.HasDelegate)
            {
                await SelectedValueChanging.InvokeAsync(arg);
                if (arg.DisallowChange)
                {
                    return false;
                }
            }
            if (OnSelectedValueChangingAsync != null)
            {
                var allowChanging = await OnSelectedValueChangingAsync(arg);
                if (!allowChanging)
                {
                    return false;
                }
            }
            SelectedValue = value;
            if (SelectedValueChanged.HasDelegate)
            {
                await SelectedValueChanged.InvokeAsync(value);
            }
            if (OnSelectedValueChangedAsync != null)
            {
                await OnSelectedValueChangedAsync(arg);
            }
            return true;
        }
    }
}
