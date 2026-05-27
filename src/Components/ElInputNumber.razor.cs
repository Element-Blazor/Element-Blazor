using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElInputNumber : ElementFieldComponentBase<decimal?>
    {
        private static long inputIdSeed;
        private readonly string generatedInputId = $"el-input-number-{Interlocked.Increment(ref inputIdSeed)}";
        private HtmlPropertyBuilder wrapperClsBuilder;
        private InputSize effectiveSize = InputSize.Normal;
        private bool effectiveDisabled;
        private string inputText;
        private bool hasInvalidInput;

        [Parameter]
        public decimal? Value { get; set; }

        [Parameter]
        public decimal? ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<decimal?> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<decimal?> ModelValueChanged { get; set; }

        [Parameter]
        public decimal? Min { get; set; }

        [Parameter]
        public decimal? Max { get; set; }

        [Parameter]
        public decimal Step { get; set; } = 1;

        [Parameter]
        public bool StepStrictly { get; set; }

        [Parameter]
        public int? Precision { get; set; }

        [Parameter]
        public bool Controls { get; set; } = true;

        [Parameter]
        public string ControlsPosition { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        [Parameter]
        public bool Readonly { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public string Autocomplete { get; set; } = "off";

        [Parameter]
        public string Inputmode { get; set; } = "decimal";

        [Parameter]
        public object Tabindex { get; set; } = 0;

        [Parameter]
        public string AriaLabel { get; set; }

        [Parameter]
        public string AriaLabelledby { get; set; }

        [Parameter]
        public string AriaDescribedby { get; set; }

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public EventCallback<decimal?> OnChange { get; set; }

        [Parameter]
        public EventCallback<decimal?> OnInput { get; set; }

        [Parameter]
        public EventCallback<FocusEventArgs> OnFocus { get; set; }

        [Parameter]
        public EventCallback<FocusEventArgs> OnBlur { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            effectiveDisabled = Disabled || (FormItem?.Form?.Disabled ?? false);
            effectiveSize = Size == InputSize.Normal
                ? FormItem?.Size ?? FormItem?.Form?.EffectiveSize ?? ResolveInputSize(InputSize.Normal)
                : Size;
            Id = string.IsNullOrWhiteSpace(Id) ? ResolveAttributeId() ?? generatedInputId : Id;
            if (FormItem?.Form != null)
            {
                FormItem.Form.RegisterInput(FormItem.Name, Id, this, FormItem);
            }
            var sizeCssValue = GetSizeCssValue(effectiveSize);
            wrapperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-input-number", Cls)
                .AddIf(effectiveDisabled, "is-disabled")
                .AddIf(IsWithoutControls, "is-without-controls")
                .AddIf(ControlsPosition == "right", "is-controls-right")
                .AddIf(sizeCssValue != null, $"el-input-number--{sizeCssValue}");

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    Value = ConvertToDecimal(FormItem.OriginValue);
                }
                SetFieldValue(Value, false);
            }

            if (!hasInvalidInput)
            {
                inputText = FormatValue(Value);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = ConvertToDecimal(value);
            inputText = FormatValue(Value);
            hasInvalidInput = false;
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

        private async Task OnInputAsync(ChangeEventArgs e)
        {
            inputText = Convert.ToString(e.Value);
            if (!TryParseInput(inputText, out var parsed))
            {
                hasInvalidInput = true;
                return;
            }

            hasInvalidInput = false;
            Value = parsed;
            SetFieldValue(Value, false);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
            if (OnInput.HasDelegate)
            {
                await OnInput.InvokeAsync(Value);
            }
        }

        private async Task OnChangeAsync(ChangeEventArgs e)
        {
            await CommitTextAsync(Convert.ToString(e.Value), ValidateEvent, notifyChange: true);
        }

        private async Task OnFocusAsync(FocusEventArgs e)
        {
            if (OnFocus.HasDelegate)
            {
                await OnFocus.InvokeAsync(e);
            }
        }

        private async Task OnBlurAsync(FocusEventArgs e)
        {
            await CommitTextAsync(inputText, ValidateEvent, notifyChange: false);
            if (OnBlur.HasDelegate)
            {
                await OnBlur.InvokeAsync(e);
            }
        }

        private async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (effectiveDisabled || Readonly)
            {
                return;
            }

            if (HasModifierKey(e))
            {
                return;
            }

            if (e.Key == "ArrowUp")
            {
                await IncreaseAsync();
            }
            else if (e.Key == "ArrowDown")
            {
                await DecreaseAsync();
            }
            else if (e.Key == "PageUp")
            {
                await StepValueAsync(GetPageStep());
            }
            else if (e.Key == "PageDown")
            {
                await StepValueAsync(-GetPageStep());
            }
            else if (e.Key == "Home" && Min.HasValue)
            {
                await CommitValueAsync(Min.Value, ValidateEvent, notifyChange: true);
            }
            else if (e.Key == "End" && Max.HasValue)
            {
                await CommitValueAsync(Max.Value, ValidateEvent, notifyChange: true);
            }
            else if (e.Key == "Enter")
            {
                await CommitTextAsync(inputText, ValidateEvent, notifyChange: true);
            }
        }

        private static bool HasModifierKey(KeyboardEventArgs e)
        {
            return e.AltKey || e.CtrlKey || e.MetaKey || e.ShiftKey;
        }

        private async Task OnControlKeyDownAsync(KeyboardEventArgs e, Func<Task> action)
        {
            if (e.Key != "Enter" && e.Key != " ")
            {
                return;
            }

            await action();
        }

        private Task IncreaseAsync()
        {
            return StepValueAsync(Step);
        }

        private Task DecreaseAsync()
        {
            return StepValueAsync(-Step);
        }

        private async Task StepValueAsync(decimal step)
        {
            if (effectiveDisabled || Readonly || (step < 0 && IsDecreaseDisabled) || (step > 0 && IsIncreaseDisabled))
            {
                return;
            }

            var source = Value ?? ResolveStepStartValue(step);
            await CommitValueAsync(source + step, ValidateEvent, notifyChange: true);
        }

        private async Task CommitValueAsync(decimal? value, bool validate, bool notifyChange)
        {
            var normalized = Normalize(value);
            if (TypeHelper.Equal(Value, normalized) && inputText == FormatValue(normalized))
            {
                hasInvalidInput = false;
                SetFieldValue(normalized, validate);
                return;
            }

            Value = normalized;
            inputText = FormatValue(Value);
            hasInvalidInput = false;
            SetFieldValue(Value, validate);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
            if (notifyChange && OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
        }

        private decimal? Normalize(decimal? value)
        {
            if (!value.HasValue)
            {
                return null;
            }

            var result = value.Value;
            if (StepStrictly && Step > 0)
            {
                result = Math.Round(result / Step, 0, MidpointRounding.AwayFromZero) * Step;
            }
            if (Precision.HasValue)
            {
                result = Math.Round(result, Precision.Value, MidpointRounding.AwayFromZero);
            }
            if (Min.HasValue && result < Min.Value)
            {
                result = Min.Value;
            }
            if (Max.HasValue && result > Max.Value)
            {
                result = Max.Value;
            }
            return result;
        }

        private async Task CommitTextAsync(string text, bool validate, bool notifyChange)
        {
            if (!TryParseInput(text, out var parsed))
            {
                inputText = FormatValue(Value);
                hasInvalidInput = false;
                SetFieldValue(Value, validate);
                return;
            }

            await CommitValueAsync(parsed, validate, notifyChange);
        }

        private bool TryParseInput(string value, out decimal? result)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                result = null;
                return true;
            }
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var invariantValue))
            {
                result = invariantValue;
                return true;
            }
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out var currentValue))
            {
                result = currentValue;
                return true;
            }
            result = null;
            return false;
        }

        private decimal ResolveStepStartValue(decimal step)
        {
            if (step > 0 && Min.HasValue)
            {
                return Min.Value;
            }

            if (step < 0 && Max.HasValue)
            {
                return Max.Value;
            }

            return 0;
        }

        private decimal GetPageStep()
        {
            var normalizedStep = Step == 0 ? 1 : Math.Abs(Step);
            return normalizedStep * 10;
        }

        private static decimal? ConvertToDecimal(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is decimal decimalValue)
            {
                return decimalValue;
            }
            return Convert.ToDecimal(value, CultureInfo.CurrentCulture);
        }

        private string FormatValue(decimal? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            if (Precision.HasValue)
            {
                return value.Value.ToString($"F{Precision.Value}", CultureInfo.InvariantCulture);
            }
            return value.Value.ToString(CultureInfo.InvariantCulture);
        }

        private string FormattedValue => inputText ?? FormatValue(Value);

        private bool IsInputNumberDisabled => effectiveDisabled;

        private bool IsWithoutControls => !Controls;

        private bool IsDecreaseDisabled => effectiveDisabled || Readonly || (Min.HasValue && (Value ?? 0) <= Min.Value);

        private bool IsIncreaseDisabled => effectiveDisabled || Readonly || (Max.HasValue && (Value ?? 0) >= Max.Value);

        private string DecreaseButtonClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-input-number__decrease")
            .AddIf(IsDecreaseDisabled, "is-disabled")
            .ToString();

        private string IncreaseButtonClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-input-number__increase")
            .AddIf(IsIncreaseDisabled, "is-disabled")
            .ToString();

        private string AriaDisabled => GetAriaBoolean(IsInputNumberDisabled);

        private string AriaReadonly => Readonly ? "true" : null;

        private string AriaInvalid => IsAriaInvalid ? "true" : "false";

        private string AriaValueMin => Min?.ToString(CultureInfo.InvariantCulture);

        private string AriaValueMax => Max?.ToString(CultureInfo.InvariantCulture);

        private string AriaValueNow => hasInvalidInput ? null : Value?.ToString(CultureInfo.InvariantCulture);

        private string AriaValueText => string.IsNullOrWhiteSpace(FormattedValue) || hasInvalidInput ? null : FormattedValue;

        private string InputAriaDescribedBy => JoinAriaIds(AriaDescribedby, AriaDescribedBy);

        private string ResolveAttributeId()
        {
            if (Attributes == null)
            {
                return null;
            }

            return Attributes.TryGetValue("id", out var id) ? Convert.ToString(id, CultureInfo.InvariantCulture) : null;
        }

        private int GetControlTabIndex(bool disabled)
        {
            return disabled ? -1 : 0;
        }

        private static string JoinAriaIds(params string[] ids)
        {
            var value = string.Join(" ", ids.Where(id => !string.IsNullOrWhiteSpace(id)));
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static string GetAriaBoolean(bool value) => value ? "true" : "false";

        private static string GetSizeCssValue(InputSize size) => size switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };
    }
}
