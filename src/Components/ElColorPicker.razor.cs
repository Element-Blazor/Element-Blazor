using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElColorPicker : ElementFieldComponentBase<string>
    {
        private static long dropDownIdSeed;
        private readonly string DropDownId = $"el-color-picker-dropdown-{Interlocked.Increment(ref dropDownIdSeed)}";
        private DropDownOption dropDownOption;
        private ElementReference colorPickerElement;
        private bool effectiveDisabled;
        private InputSize effectiveSize = InputSize.Normal;
        private bool isHovering;

        [Inject]
        internal PopupService PopupService { get; set; }

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
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        [Parameter]
        public bool ShowAlpha { get; set; }

        [Parameter]
        public string ColorFormat { get; set; }

        [Parameter]
        public IEnumerable<string> Predefine { get; set; }

        [Parameter]
        public bool Clearable { get; set; }

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public string PopperClass { get; set; }

        [Parameter]
        public string PopperStyle { get; set; }

        [Parameter]
        public int PopperMaxHeight { get; set; } = 420;

        [Parameter]
        public object Tabindex { get; set; } = 0;

        [Parameter]
        public string AriaLabel { get; set; } = "color picker";

        [Parameter]
        public EventCallback<string> OnChange { get; set; }

        [Parameter]
        public EventCallback<string> OnActiveChange { get; set; }

        [Parameter]
        public EventCallback<bool> OnVisibleChange { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClear { get; set; }

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

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    Value = Convert.ToString(FormItem.OriginValue, CultureInfo.CurrentCulture);
                }
                SetFieldValue(Value, false);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = Convert.ToString(value, CultureInfo.CurrentCulture);
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

        private async Task OnColorPickerClickAsync(MouseEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            if (IsDropDownOpen)
            {
                await HideAsync();
                return;
            }

            await ShowAsync();
        }

        private async Task OnClearIconClickAsync(MouseEventArgs e)
        {
            if (!Clearable || effectiveDisabled || !HasColor)
            {
                await OnColorPickerClickAsync(e);
                return;
            }

            await ClearAsync(e);
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
            if (ValidateEvent)
            {
                SetFieldValue(Value, true);
            }
            if (OnBlur.HasDelegate)
            {
                await OnBlur.InvokeAsync(e);
            }
        }

        private async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            if (e.Key == "Enter" || e.Key == " " || e.Key == "ArrowDown")
            {
                await ShowAsync();
            }
            else if (e.Key == "Escape")
            {
                await HideAsync();
            }
        }

        public Task ShowAsync()
        {
            return OpenDropDownAsync();
        }

        public Task HideAsync()
        {
            return CloseDropDownAsync();
        }

        public ValueTask FocusAsync()
        {
            return colorPickerElement.Dom(JSRuntime).FocusAsync();
        }

        public ValueTask BlurAsync()
        {
            return colorPickerElement.Dom(JSRuntime).BlurAsync();
        }

        public Task ClearAsync()
        {
            return ClearAsync(null);
        }

        private async Task ClearAsync(MouseEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            await CommitValueAsync(string.Empty, ValidateEvent, notifyChange: true, notifyActiveChange: true);
            if (OnClear.HasDelegate)
            {
                await OnClear.InvokeAsync(e);
            }
            await HideAsync();
        }

        private async Task OnPanelValueChangedAsync(string value)
        {
            await CommitValueAsync(value, ValidateEvent, notifyChange: true, notifyActiveChange: false);
        }

        private async Task OnPanelActiveChangeAsync(string value)
        {
            if (OnActiveChange.HasDelegate)
            {
                await OnActiveChange.InvokeAsync(value);
            }
        }

        private async Task CommitValueAsync(string value, bool validate, bool notifyChange, bool notifyActiveChange)
        {
            Value = value;
            SetFieldValue(Value, validate);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
            if (notifyActiveChange && OnActiveChange.HasDelegate)
            {
                await OnActiveChange.InvokeAsync(Value);
            }
            if (notifyChange && OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
        }

        private async Task OpenDropDownAsync()
        {
            if (effectiveDisabled)
            {
                return;
            }

            if (dropDownOption != null)
            {
                RefreshDropDown();
                return;
            }

            dropDownOption = new DropDownOption
            {
                Target = colorPickerElement,
                OptionContent = BuildPanelContent,
                PopperClass = string.Join(" ", new[] { "el-color-picker__dropdown", PopperClass }.Where(x => !string.IsNullOrWhiteSpace(x))),
                PopperStyle = PopperStyle,
                MaxHeight = PopperMaxHeight,
                FitInputWidth = false,
                AutoWidth = false,
                DropDownId = DropDownId,
                Refresh = () => InvokeAsync(StateHasChanged),
                OnClosed = () => NotifyVisibleChangeAsync(false),
                IsShow = true,
                Width = 300
            };
            PopupService.SelectDropDownOptions.Add(dropDownOption);
            await NotifyVisibleChangeAsync(true);
        }

        private async Task CloseDropDownAsync()
        {
            if (dropDownOption?.Instance != null)
            {
                await dropDownOption.Instance.CloseDropDownAsync(dropDownOption);
                return;
            }

            if (dropDownOption != null)
            {
                PopupService.SelectDropDownOptions.Remove(dropDownOption);
                dropDownOption = null;
                await NotifyVisibleChangeAsync(false);
            }
        }

        private Task NotifyVisibleChangeAsync(bool visible)
        {
            if (!visible)
            {
                dropDownOption = null;
            }

            if (OnVisibleChange.HasDelegate)
            {
                return OnVisibleChange.InvokeAsync(visible);
            }

            StateHasChanged();
            return Task.CompletedTask;
        }

        private void BuildPanelContent(RenderTreeBuilder builder)
        {
            var seq = 0;
            builder.OpenElement(seq++, "li");
            builder.AddAttribute(seq++, "class", "el-color-picker__panel-item");
            builder.AddAttribute(seq++, "role", "presentation");
            builder.OpenComponent<ElColorPickerPanel>(seq++);
            builder.AddAttribute(seq++, nameof(ElColorPickerPanel.Value), Value);
            builder.AddAttribute(seq++, nameof(ElColorPickerPanel.ValueChanged), EventCallback.Factory.Create<string>(this, OnPanelValueChangedAsync));
            builder.AddAttribute(seq++, nameof(ElColorPickerPanel.ShowAlpha), ShowAlpha);
            builder.AddAttribute(seq++, nameof(ElColorPickerPanel.ColorFormat), ColorFormat);
            builder.AddAttribute(seq++, nameof(ElColorPickerPanel.Predefine), Predefine);
            builder.AddAttribute(seq++, nameof(ElColorPickerPanel.Disabled), effectiveDisabled);
            builder.AddAttribute(seq++, nameof(ElColorPickerPanel.ValidateEvent), false);
            builder.AddAttribute(seq++, nameof(ElColorPickerPanel.Border), false);
            builder.AddAttribute(seq++, nameof(ElColorPickerPanel.OnActiveChange), EventCallback.Factory.Create<string>(this, OnPanelActiveChangeAsync));
            builder.AddAttribute(seq++, nameof(ElColorPickerPanel.FooterContent), BuildFooterContent());
            builder.CloseComponent();
            builder.CloseElement();
        }

        private RenderFragment BuildFooterContent()
        {
            return builder =>
            {
                var seq = 0;
                if (Clearable)
                {
                    builder.OpenElement(seq++, "button");
                    builder.AddAttribute(seq++, "type", "button");
                    builder.AddAttribute(seq++, "class", "el-color-dropdown__link-btn");
                    builder.AddAttribute(seq++, "disabled", effectiveDisabled || !HasColor);
                    builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, e => ClearAsync(e)));
                    builder.AddContent(seq++, "清空");
                    builder.CloseElement();
                }

                builder.OpenElement(seq++, "button");
                builder.AddAttribute(seq++, "type", "button");
                builder.AddAttribute(seq++, "class", "el-color-dropdown__btn");
                builder.AddAttribute(seq++, "disabled", effectiveDisabled);
                builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, _ => HideAsync()));
                builder.AddContent(seq++, "确定");
                builder.CloseElement();
            };
        }

        public override void Dispose()
        {
            if (dropDownOption != null)
            {
                PopupService.SelectDropDownOptions.Remove(dropDownOption);
            }
            base.Dispose();
        }

        private void RefreshDropDown()
        {
            dropDownOption?.RequestRender?.Invoke();
            StateHasChanged();
        }

        private string WrapperClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-color-picker", Cls)
            .AddIf(effectiveDisabled, "is-disabled")
            .AddIf(IsDropDownOpen, "is-focus")
            .AddIf(SizeCssValue != null, $"el-color-picker--{SizeCssValue}")
            .ToString();

        private string ColorBlockClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-color-picker__color")
            .AddIf(ShowAlpha && HasColor, "is-alpha")
            .ToString();

        private string ColorInnerStyle => $"background-color:{DisplayColor}";

        private string DisplayColor
        {
            get
            {
                if (!ElementColor.TryParse(Value, out var color))
                {
                    return "transparent";
                }

                return color.ToCssString(ElementColorFormat.Rgb, ShowAlpha || color.Alpha < 1);
            }
        }

        private string TriggerIconClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-color-picker__icon")
            .Add(Clearable && isHovering ? "el-icon-close el-color-picker__clear" : "el-icon-arrow-down")
            .ToString();

        private string SizeCssValue => effectiveSize switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };

        private bool HasColor => ElementColor.TryParse(Value, out _);

        private bool IsDropDownOpen => dropDownOption != null && dropDownOption.IsShow;

        private bool IsColorPickerDisabled => effectiveDisabled;
    }
}
