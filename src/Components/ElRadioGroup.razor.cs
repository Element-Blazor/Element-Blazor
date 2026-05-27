using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElRadioGroup<TValue> : ElementFieldComponentBase<TValue>
    {
        [Parameter]
        public EventCallback<TValue> SelectedValueChanged { get; set; }

        [Parameter]
        public RadioSize Size { get; set; }

        internal RadioSize EffectiveSize
        {
            get
            {
                if (Size != RadioSize.Default)
                {
                    return ResolveRadioSize(Size);
                }

                return FormItem?.Form?.EffectiveSize switch
                {
                    InputSize.Large => RadioSize.Medium,
                    InputSize.Small => RadioSize.Small,
                    _ => ResolveRadioSize(Size)
                };
            }
        }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        internal bool EffectiveDisabled => Disabled || (FormItem?.Form?.Disabled ?? false);

        [Parameter]
        public EventCallback<ElementChangeEventArgs<TValue>> SelectedValueChanging { get; set; }
        [Parameter]
        public TValue SelectedValue { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SetFieldValue(SelectedValue, false);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (FormItem == null)
            {
                return;
            }
            if (FormItem.OriginValueHasRendered)
            {
                return;
            }
            FormItem.OriginValueHasRendered = true;
            if (FormItem.Form.Values.Any())
            {
                SelectedValue = FormItem.OriginValue == null
                    ? default
                    : (TValue)TypeHelper.ChangeType(FormItem.OriginValue, typeof(TValue));
            }
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
            var arg = new ElementChangeEventArgs<TValue>()
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
