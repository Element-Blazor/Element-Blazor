
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElSwitch<TValue> : ElementFieldComponentBase<TValue>
    {
        [Parameter]
        public TValue ActiveValue { get; set; }
        [Parameter]
        public TValue InactiveValue { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }
        [Parameter]
        public bool Disabled
        {
            get => IsDisabled;
            set => IsDisabled = value;
        }
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
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<TValue> ModelValueChanged { get; set; }

        [Parameter]
        public TValue ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public TValue Model
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<MouseEventArgs> OnChanged { get; set; }

        public event Func<MouseEventArgs, Task> OnChangedAsync;

        [Parameter]
        public bool Loading { get; set; }

        [Parameter]
        public string LoadingIcon { get; set; } = "el-icon-loading";

        [Parameter]
        public Func<TValue, TValue, Task<bool>> BeforeChange { get; set; }

        [Parameter]
        public EventCallback<ElementChangeEventArgs<TValue>> ValueChanging { get; set; }

        protected bool EffectiveDisabled => IsDisabled || Loading || (FormItem?.Form?.Disabled ?? false);

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SetFieldValue(Value, false);
        }
        protected async Task OnInternalSwitchChangedAsync(MouseEventArgs e)
        {
            if (EffectiveDisabled)
            {
                return;
            }
            var oldValue = Value;
            var nextValue = TypeHelper.Equal(Value, InactiveValue) ? ActiveValue : InactiveValue;
            if (BeforeChange != null && !await BeforeChange(oldValue, nextValue))
            {
                return;
            }
            if (ValueChanging.HasDelegate)
            {
                var args = new ElementChangeEventArgs<TValue>
                {
                    OldValue = oldValue,
                    NewValue = nextValue
                };
                await ValueChanging.InvokeAsync(args);
                if (args.DisallowChange)
                {
                    return;
                }
            }

            Value = nextValue;
            SetFieldValue(Value, true);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
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
            if (TypeHelper.Equal(FormItem.OriginValue, default))
            {
                Value = InactiveValue;
            }
            else
            {
                var boolValue = Convert.ToBoolean(FormItem.OriginValue);
                if (boolValue)
                {
                    Value = ActiveValue;
                }
                else
                {
                    Value = InactiveValue;
                }
            }
            SetFieldValue(Value, false);
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            if (value == null)
            {
                Value = InactiveValue;
            }
            else
            {
                var boolValue = Convert.ToBoolean(value);
                if (boolValue)
                {
                    Value = ActiveValue;
                }
                else
                {
                    Value = InactiveValue;
                }
            }
        }
        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
