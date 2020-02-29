using Blazui.Component.EventArgs;
using Blazui.Component;
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
            SetFieldValue(SelectedValue, false);
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            if (value == null)
            {
                SelectedValue = default;
            }
            else
            {
                SelectedValue = (TValue)TypeHelper.ChangeType(value, typeof(TValue));
            }
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
            SetFieldValue(SelectedValue, true);
            RequireRender = true;
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

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
