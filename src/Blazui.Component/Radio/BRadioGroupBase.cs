using Blazui.Component.EventArgs;
using Blazui.Component.Form;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Radio
{
    public class BRadioGroupBase<TValue> : BFieldComponentBase<TValue>
    {
        [Parameter]
        public EventCallback<TValue> SelectedValueChanged { get; set; }

        [Parameter]
        public RadioSize Size { get; set; }
        [Parameter]
        public EventCallback<BChangeEventArgs<TValue>> SelectedValueChanging { get; set; }
        [Parameter]
        public TValue SelectedValue { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SetFieldValue(SelectedValue);
        }

        protected override void FormItem_OnReset()
        {
            SelectedValue = default;
        }

        internal async Task<bool> TrySetValueAsync(TValue value, bool requireRefresh)
        {
            var arg = new BChangeEventArgs<TValue>()
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
            SelectedValue = value;
            SetFieldValue(SelectedValue);
            if (SelectedValueChanged.HasDelegate)
            {
                await SelectedValueChanged.InvokeAsync(value);
            }
            if (requireRefresh)
            {
                StateHasChanged();
            }
            return true;
        }
    }
}
