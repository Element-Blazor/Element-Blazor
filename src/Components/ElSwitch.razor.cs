
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElSwitch<TValue> : ElementFieldComponentBase<TValue>
    {
        private static long inputIdSeed;
        private readonly string generatedInputId = $"el-switch-{Interlocked.Increment(ref inputIdSeed)}";
        private HtmlPropertyBuilder wrapperClsBuilder;
        private TValue activeValue;
        private TValue inactiveValue;
        private bool activeValueSet;
        private bool inactiveValueSet;

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
        public string Id { get; set; }

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

        protected TValue EffectiveActiveValue => activeValue;

        protected TValue EffectiveInactiveValue => inactiveValue;

        protected bool IsActive => TypeHelper.Equal(EffectiveActiveValue, Value);

        protected bool IsInactive => TypeHelper.Equal(EffectiveInactiveValue, Value);

        protected string InputId => Id;

        protected string AriaDisabled => EffectiveDisabled ? "true" : "false";

        protected string AriaChecked => IsActive ? "true" : "false";

        protected string AriaBusy => Loading ? "true" : "false";

        protected string AriaInvalid => IsAriaInvalid ? "true" : "false";

        protected string CurrentValueText => Convert.ToString(Value, CultureInfo.CurrentCulture);

        protected async Task OnInternalSwitchChangedAsync(MouseEventArgs e)
        {
            if (EffectiveDisabled)
            {
                return;
            }
            var oldValue = Value;
            var nextValue = IsActive ? EffectiveInactiveValue : EffectiveActiveValue;
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
            BuildWrapperClass();
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

        protected Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (e.Key != "Enter" && e.Key != " ")
            {
                return Task.CompletedTask;
            }

            return OnInternalSwitchChangedAsync(new MouseEventArgs());
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ResolveSwitchValues();
            Id = string.IsNullOrWhiteSpace(Id) ? ResolveAttributeId() ?? generatedInputId : Id;
            if (FormItem?.Form != null)
            {
                FormItem.Form.RegisterInput(FormItem.Name, InputId, this, FormItem);
            }
            if (FormItem == null)
            {
                BuildWrapperClass();
                return;
            }

            if (FormItem.OriginValueHasRendered)
            {
                BuildWrapperClass();
                return;
            }

            FormItem.OriginValueHasRendered = true;
            if (FormItem.Form.Values.Any())
            {
                Value = TypeHelper.Equal(FormItem.OriginValue, default)
                    ? EffectiveInactiveValue
                    : NormalizeIncomingValue(FormItem.OriginValue);
            }
            SetFieldValue(Value, false);
            BuildWrapperClass();
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            if (value == null)
            {
                Value = EffectiveInactiveValue;
            }
            else
            {
                Value = NormalizeIncomingValue(value);
            }
            SetFieldValue(Value, false);
            BuildWrapperClass();
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                _ = ModelValueChanged.InvokeAsync(Value);
            }
            else
            {
                StateHasChanged();
            }
        }

        private void ResolveSwitchValues()
        {
            activeValue = ActiveValue;
            inactiveValue = InactiveValue;
            var valueType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
            if (valueType == typeof(bool))
            {
                if (!activeValueSet)
                {
                    activeValue = (TValue)(object)true;
                }
                if (!inactiveValueSet)
                {
                    inactiveValue = (TValue)(object)false;
                }
            }
        }

        private TValue NormalizeIncomingValue(object value)
        {
            if (ValueEquals(value, EffectiveActiveValue))
            {
                return EffectiveActiveValue;
            }
            if (ValueEquals(value, EffectiveInactiveValue))
            {
                return EffectiveInactiveValue;
            }

            return Convert.ToBoolean(value, CultureInfo.CurrentCulture)
                ? EffectiveActiveValue
                : EffectiveInactiveValue;
        }

        private bool ValueEquals(object candidate, TValue value)
        {
            if (candidate is TValue typedCandidate)
            {
                return TypeHelper.Equal(typedCandidate, value);
            }

            try
            {
                return TypeHelper.Equal(ConvertToValue(candidate), value);
            }
            catch
            {
                return false;
            }
        }

        private static TValue ConvertToValue(object value)
        {
            return value == null ? default : (TValue)TypeHelper.ChangeType(value, typeof(TValue));
        }

        private void BuildWrapperClass()
        {
            wrapperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-switch", Cls)
                .AddIf(IsActive, "is-checked")
                .AddIf(IsDisabled || (FormItem?.Form?.Disabled ?? false), "is-disabled")
                .AddIf(Loading, "is-loading");
        }

        private string ResolveAttributeId()
        {
            if (Attributes == null)
            {
                return null;
            }

            return Attributes.TryGetValue("id", out var id) ? Convert.ToString(id, CultureInfo.InvariantCulture) : null;
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            activeValueSet = parameters.TryGetValue<TValue>(nameof(ActiveValue), out _);
            inactiveValueSet = parameters.TryGetValue<TValue>(nameof(InactiveValue), out _);
            return base.SetParametersAsync(parameters);
        }

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
