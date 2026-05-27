using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElInputOtp : ElementFieldComponentBase<string>
    {
        private ElementReference[] inputElements = Array.Empty<ElementReference>();
        private HtmlPropertyBuilder wrapperClsBuilder;
        private bool effectiveDisabled;
        private string internalValue = string.Empty;

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public string ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<string> ModelValueChanged { get; set; }

        [Parameter]
        public int Length { get; set; } = 6;

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
        public string Inputmode { get; set; } = "numeric";

        [Parameter]
        public bool Mask { get; set; }

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public EventCallback<string> OnInput { get; set; }

        [Parameter]
        public EventCallback<string> OnChange { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            effectiveDisabled = Disabled || (FormItem?.Form?.Disabled ?? false);
            Length = Math.Max(1, Length);
            if (inputElements.Length != Length)
            {
                inputElements = new ElementReference[Length];
            }

            wrapperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-input-otp", Cls)
                .AddIf(effectiveDisabled, "is-disabled")
                .AddIf(Readonly, "is-readonly");

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    Value = Convert.ToString(FormItem.OriginValue);
                }
                SetFieldValue(Value, false);
            }

            internalValue = Normalize(Value);
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = Convert.ToString(value);
            internalValue = Normalize(Value);
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

        private async Task OnCellInputAsync(int index, ChangeEventArgs e)
        {
            if (effectiveDisabled || Readonly)
            {
                return;
            }

            var text = Convert.ToString(e.Value) ?? string.Empty;
            if (text.Length > 1)
            {
                await ApplyTextAsync(index, text);
                return;
            }

            var chars = EnsureCells();
            chars[index] = text.FirstOrDefault();
            var nextValue = new string(chars).TrimEnd('\0');
            var shouldNotifyChange = IsValueComplete(nextValue);
            await CommitAsync(nextValue, validate: shouldNotifyChange ? ValidateEvent : false, notifyChange: shouldNotifyChange);

            if (!string.IsNullOrEmpty(text) && index < Length - 1)
            {
                await FocusCellAsync(index + 1);
            }
        }

        private async Task OnCellKeyDownAsync(int index, KeyboardEventArgs e)
        {
            if (effectiveDisabled || Readonly)
            {
                return;
            }

            if (e.Key == "Backspace")
            {
                await HandleBackspaceAsync(index);
            }
            else if (e.Key == "ArrowLeft" && index > 0)
            {
                await FocusCellAsync(index - 1);
            }
            else if (e.Key == "ArrowRight" && index < Length - 1)
            {
                await FocusCellAsync(index + 1);
            }
            else if (e.Key == "Delete")
            {
                await HandleDeleteAsync(index);
            }
            else if (e.Key == "Home")
            {
                await FocusCellAsync(0);
            }
            else if (e.Key == "End")
            {
                await FocusCellAsync(Math.Max(Length - 1, 0));
            }
        }

        private async Task ApplyTextAsync(int index, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var chars = EnsureCells();
            var writeIndex = index;
            foreach (var value in text.Where(x => !char.IsWhiteSpace(x)))
            {
                if (writeIndex >= Length)
                {
                    break;
                }
                chars[writeIndex++] = value;
            }

            var nextValue = new string(chars).TrimEnd('\0');
            await CommitAsync(nextValue, validate: ValidateEvent, notifyChange: true);
            await FocusCellAsync(GetNextFocusIndex(writeIndex));
        }

        private async Task CommitAsync(string value, bool validate, bool notifyChange)
        {
            Value = Normalize(value);
            internalValue = Value;
            SetFieldValue(Value, validate);
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
            if (notifyChange)
            {
                await NotifyChangeAsync();
            }
        }

        private async Task NotifyChangeAsync()
        {
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
        }

        private async Task HandleBackspaceAsync(int index)
        {
            var chars = EnsureCells();
            if (chars[index] != '\0')
            {
                chars[index] = '\0';
                await CommitAsync(new string(chars).TrimEnd('\0'), validate: false, notifyChange: false);
                return;
            }

            if (index <= 0)
            {
                await CommitAsync(string.Empty, validate: false, notifyChange: false);
                return;
            }

            chars[index - 1] = '\0';
            await CommitAsync(new string(chars).TrimEnd('\0'), validate: false, notifyChange: false);
            await FocusCellAsync(index - 1);
        }

        private async Task HandleDeleteAsync(int index)
        {
            var chars = EnsureCells();
            if (chars[index] == '\0')
            {
                return;
            }

            chars[index] = '\0';
            await CommitAsync(new string(chars).TrimEnd('\0'), validate: false, notifyChange: false);
        }

        private async Task FocusCellAsync(int index)
        {
            if (index < 0 || index >= inputElements.Length)
            {
                return;
            }
            await inputElements[index].Dom(JSRuntime).FocusAsync();
        }

        private string GetCellValue(int index)
        {
            if (index < 0 || index >= internalValue.Length)
            {
                return string.Empty;
            }
            var cell = internalValue[index];
            if (cell == '\0')
            {
                return string.Empty;
            }

            var value = cell.ToString();
            return Mask && !string.IsNullOrEmpty(value) ? "*" : value;
        }

        private char[] EnsureCells()
        {
            var chars = new char[Length];
            var value = Normalize(internalValue);
            for (var i = 0; i < Math.Min(value.Length, Length); i++)
            {
                chars[i] = value[i];
            }
            return chars;
        }

        private string Normalize(string value)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : new string(value.Where(x => x != '\0').Take(Length).ToArray());
        }

        private int GetNextFocusIndex(int writeIndex)
        {
            if (writeIndex <= 0)
            {
                return 0;
            }

            return Math.Min(writeIndex >= Length ? Length - 1 : writeIndex, Length - 1);
        }

        private bool IsValueComplete(string value) => !string.IsNullOrEmpty(value) && value.Length >= Length;

        private bool IsInputOtpDisabled => effectiveDisabled;
    }
}
