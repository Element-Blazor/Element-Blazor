using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;

namespace Element
{
    public partial class BInput<TValue> : BFieldComponentBase<TValue>, IDisposable
    {
        internal HtmlPropertyBuilder wrapperClsBuilder;
        private static long inputIdSeed;
        private readonly string generatedInputId = $"el-input-{Interlocked.Increment(ref inputIdSeed)}";
        private bool passwordVisible;
        private bool isComposing;
        private InputSize effectiveSize = InputSize.Normal;
        private bool effectiveDisabled;
        /// <summary>
        /// 输入框类型
        /// </summary>
        [Parameter]
        public InputType Type { get; set; } = InputType.Text;

        /// <summary>
        /// 当输入值错误时是否抛出异常
        /// </summary>
        protected virtual bool ThrowOnInvalidValue { get; } = false;

        /// <summary>
        /// 在前方加入元素
        /// </summary>
        [Parameter]
        public RenderFragment Prepend { get; set; }

        [Parameter]
        public RenderFragment Append { get; set; }

        /// <summary>
        /// 是否启用清空按钮
        /// </summary>
        [Parameter]
        public bool IsClearButtonClick { get; set; }

        [Parameter]
        public EventCallback<bool> EnableClearButtonChanged { get; set; }

        /// <summary>
        /// 按钮背景图片
        /// </summary>
        [Parameter]
        public string Image { get; set; }

        /// <summary>
        /// 输入框尺寸
        /// </summary>
        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;
        [Parameter]
        public bool EnableClearButton { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        [Parameter]
        public bool Clearable { get; set; }

        [Parameter]
        public bool IsClearable
        {
            get => Clearable;
            set => Clearable = value;
        }

        [Parameter]
        public bool Readonly { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public object Maxlength { get; set; }

        [Parameter]
        public object Minlength { get; set; }

        [Parameter]
        public string Resize { get; set; }

        [Parameter]
        public bool Autosize { get; set; }

        [Parameter]
        public string Autocomplete { get; set; } = "off";

        [Parameter]
        public string Form { get; set; }

        [Parameter]
        public bool ShowPassword { get; set; }

        [Parameter]
        public bool ShowWordLimit { get; set; }

        [Parameter]
        public string WordLimitPosition { get; set; } = "inside";

        [Parameter]
        public string InputStyle { get; set; }

        [Parameter]
        public bool Autofocus { get; set; }

        [Parameter]
        public int Rows { get; set; } = 2;

        [Parameter]
        public string AriaLabel { get; set; }

        [Parameter]
        public string Inputmode { get; set; }

        [Parameter]
        public object Tabindex { get; set; } = 0;

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public EventCallback<FocusEventArgs> OnFocus { get; set; }

        [Parameter]
        public EventCallback<FocusEventArgs> OnBlur { get; set; }

        [Parameter]
        public EventCallback<TValue> OnInput { get; set; }

        [Parameter]
        public EventCallback<TValue> OnChange { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClear { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        [Parameter]
        public EventCallback OnCompositionStart { get; set; }

        [Parameter]
        public EventCallback OnCompositionUpdate { get; set; }

        [Parameter]
        public EventCallback<TValue> OnCompositionEnd { get; set; }

        /// <summary>
        /// 如何格式化
        /// </summary>
        [Parameter]
        public virtual Func<TValue, string> Formatter { get; set; } = v => Convert.ToString(v);

        [Parameter]
        public virtual Func<string, string> Parser { get; set; }

        /// <summary>
        /// 输入框的值
        /// </summary>
        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public TValue ModelValue
        {
            get => Value;
            set => Value = value;
        }

        /// <summary>
        /// 值改变时触发
        /// </summary>
        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<TValue> ModelValueChanged { get; set; }

        /// <summary>
        /// Placeholder
        /// </summary>
        [Parameter]
        public virtual string Placeholder { get; set; } = "请输入内容";

        internal bool IsFocus { get; set; }

        internal bool IsMouseOver { get; set; }

        [Parameter]
        public virtual string PrefixIcon { get; set; }

        [Parameter]
        public virtual EventCallback<MouseEventArgs> OnSuffixIconClick { get; set; }
        [Parameter]
        public virtual EventCallback<MouseEventArgs> OnPrefixIconClick { get; set; }
        [Parameter]
        public virtual EventCallback<MouseEventArgs> OnInputClick { get; set; }
        [Parameter]
        public virtual EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter]
        public virtual string SuffixIcon { get; set; }
        [Parameter]
        public virtual string AdditionalClearIcon { get; set; }

        internal ElementReference Content { get; set; }

        /// <summary>
        /// HTML 元素引用
        /// </summary>
        public ElementReference InputElement { get; set; }

        protected async Task ClearOnClick(MouseEventArgs e)
        {
            EnableClearButton = true;
            if (EnableClearButtonChanged.HasDelegate)
            {
                await EnableClearButtonChanged.InvokeAsync(EnableClearButton);
            }
            Value = TypeHelper.ChangeType<TValue>(string.Empty);
            if (FormItem != null)
            {
                FormItem.Value = Value;
            }
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
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
            if (OnClear.HasDelegate)
            {
                await OnClear.InvokeAsync(e);
            }
            SetFieldValue(Value, ValidateEvent);
        }

        protected virtual async Task OnFocusAsync()
        {
            if (Value == null)
            {
                Value = default;
            }
            IsFocus = true;
        }

        protected async Task OnFocusEventAsync(FocusEventArgs e)
        {
            await OnFocusAsync();
            if (OnFocus.HasDelegate)
            {
                await OnFocus.InvokeAsync(e);
            }
        }

        protected async Task OnBlurEventAsync(FocusEventArgs e)
        {
            IsFocus = false;
            if (ValidateEvent)
            {
                SetFieldValue(Value, true);
            }
            if (OnBlur.HasDelegate)
            {
                await OnBlur.InvokeAsync(e);
            }
        }

        protected virtual async Task OnChangeEventArgs(ChangeEventArgs input)
        {
            if (isComposing)
            {
                return;
            }

            try
            {
                var value = input.Value;
                if (Formatter != null && Parser != null && value != null)
                {
                    value = Parser(Convert.ToString(value));
                }
                Value = (TValue)TypeHelper.ChangeType(value, typeof(TValue));
            }
            catch (FormatException)
            {
                if (ThrowOnInvalidValue)
                {
                    throw;
                }
                Value = default;
            }
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
            SetFieldValue(Value, false);
        }

        protected async Task OnNativeChangeAsync(ChangeEventArgs input)
        {
            if (ValidateEvent)
            {
                SetFieldValue(Value, false);
                FormItem?.Validate();
                FormItem?.ShowErrorMessage();
            }
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
        }

        protected void TogglePasswordVisibility()
        {
            passwordVisible = !passwordVisible;
        }

        protected async Task OnCompositionStartAsync()
        {
            isComposing = true;
            if (OnCompositionStart.HasDelegate)
            {
                await OnCompositionStart.InvokeAsync();
            }
        }

        protected async Task OnCompositionUpdateAsync()
        {
            if (OnCompositionUpdate.HasDelegate)
            {
                await OnCompositionUpdate.InvokeAsync();
            }
        }

        protected async Task OnCompositionEndAsync(ChangeEventArgs input)
        {
            isComposing = false;
            await OnChangeEventArgs(input);
            if (OnCompositionEnd.HasDelegate)
            {
                await OnCompositionEnd.InvokeAsync(Value);
            }
        }

        public ValueTask FocusAsync()
        {
            return InputElement.Dom(JSRuntime).FocusAsync();
        }

        public ValueTask BlurAsync()
        {
            return InputElement.Dom(JSRuntime).BlurAsync();
        }

        public ValueTask SelectAsync()
        {
            return InputElement.Dom(JSRuntime).SelectAsync();
        }

        public async Task ClearAsync()
        {
            await ClearOnClick(null);
        }

        public override void Dispose()
        {
            FormItem?.Form?.UnregisterInput(this);
            base.Dispose();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (effectiveDisabled)
            {
                await InputElement.Dom(JSRuntime).SetDisabledAsync(effectiveDisabled);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            if (value == null)
            {
                Value = default;
            }
            else
            {
                Value = (TValue)TypeHelper.ChangeType(value, typeof(TValue));
            }

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

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            effectiveDisabled = Disabled || (FormItem?.Form?.Disabled ?? false);
            effectiveSize = Size == InputSize.Normal
                ? FormItem?.Size ?? FormItem?.Form?.Size ?? InputSize.Normal
                : Size;
            Id = string.IsNullOrWhiteSpace(Id) && FormItem != null ? generatedInputId : Id;
            if (FormItem?.Form != null)
            {
                FormItem.Form.RegisterInput(FormItem.Name, Id, this);
            }
            var inputSizeCssValue = GetSizeCssValue(effectiveSize);
            wrapperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add(Type == InputType.Textarea ? "el-textarea" : "el-input", Cls)
                .AddIf(inputSizeCssValue != null, Type == InputType.Textarea ? $"el-textarea--{inputSizeCssValue}" : $"el-input--{inputSizeCssValue}")
                .AddIf(HasSuffix, "el-input--suffix")
                .AddIf(!string.IsNullOrWhiteSpace(PrefixIcon), "el-input--prefix")
                .AddIf(effectiveDisabled, "is-disabled")
                .AddIf(IsFocus, "is-focus")
                .AddIf(IsExceed, "is-exceed")
                .AddIf(Prepend != null || Append != null, "el-input-group")
                .AddIf(Prepend != null, "el-input-group--prepend")
                .AddIf(Append != null, "el-input-group--append");
            if (FormItem == null)
            {
                return;
            }
            if (FormItem.OriginValueHasRendered)
            {
                SetFieldValue(Value ?? FormItem.Value, false);
                Value = Value ?? FormItem.OriginValue;
                return;
            }
            FormItem.OriginValueHasRendered = true;
            if (FormItem.Form.Values.Any())
            {
                Value = FormItem.OriginValue;
            }
            SetFieldValue(Value, false);
        }

        protected string NativeInputType => (ShowPassword && Type == InputType.Password && passwordVisible) ? "text" : Type.ToString().ToLower();

        protected bool IsInputDisabled => effectiveDisabled;

        protected bool IsWordLimitVisible => ShowWordLimit
            && Maxlength != null
            && (Type == InputType.Text || Type == InputType.Textarea)
            && !effectiveDisabled
            && !Readonly
            && !ShowPassword;

        protected bool ShowPasswordToggle => ShowPassword
            && Type == InputType.Password
            && !effectiveDisabled
            && !string.IsNullOrEmpty(Formatter(Value));

        protected bool HasSuffix => Clearable || !string.IsNullOrWhiteSpace(SuffixIcon) || ShowPasswordToggle || IsWordLimitVisible || HasValidationStatusIcon;

        protected string InputElementClass => Type == InputType.Textarea
            ? HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-textarea__inner")
                .AddIf(IsFocus, "is-focus")
                .AddIf(Clearable, "is-clearable")
                .ToString()
            : "el-input__inner";

        protected int TextLength => Formatter(Value)?.Length ?? 0;

        protected bool IsExceed => IsWordLimitVisible && int.TryParse(Convert.ToString(Maxlength), out var max) && TextLength > max;

        protected bool HasValidationStatusIcon => FormItem != null && FormItem.Form.StatusIcon && FormItem.ValidationResult != null;

        protected string ValidationStatusIconClass
        {
            get
            {
                var state = FormItem?.ValidateStatus;
                var icon = state == "success" ? "el-icon-success" : state == "validating" ? "el-icon-loading is-loading" : "el-icon-error";
                return $"el-input__icon el-input__validateIcon {icon}";
            }
        }

        protected override bool ShouldRender()
        {
            return true;
        }

        private static string GetSizeCssValue(InputSize size) => size switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };
    }
}
