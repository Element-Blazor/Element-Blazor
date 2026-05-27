using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElColorPickerPanel : ElementFieldComponentBase<string>
    {
        private ElementColor currentColor = ElementColor.Parse("#409eff");
        private string inputValue;
        private bool effectiveDisabled;
        private IReadOnlyList<PredefinedColor> predefinedColors = Array.Empty<PredefinedColor>();

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
        public bool Border { get; set; } = true;

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        [Parameter]
        public bool ShowAlpha { get; set; }

        [Parameter]
        public string ColorFormat { get; set; }

        [Parameter]
        public IEnumerable<string> Predefine { get; set; }

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public string HueSliderClass { get; set; }

        [Parameter]
        public string HueSliderStyle { get; set; }

        [Parameter]
        public RenderFragment FooterContent { get; set; }

        [Parameter]
        public RenderFragment Footer { get; set; }

        [Parameter]
        public EventCallback<string> OnChange { get; set; }

        [Parameter]
        public EventCallback<string> OnActiveChange { get; set; }

        public ElementColor Color => currentColor?.Clone();

        public ElementReference InputRef { get; private set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            effectiveDisabled = Disabled || (FormItem?.Form?.Disabled ?? false);

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    Value = Convert.ToString(FormItem.OriginValue, CultureInfo.CurrentCulture);
                }
                SetFieldValue(Value, false);
            }

            if (ElementColor.TryParse(Value, out var parsed))
            {
                currentColor = parsed;
            }
            inputValue = FormatColor(currentColor);
            predefinedColors = (Predefine ?? Enumerable.Empty<string>())
                .Select(x => new PredefinedColor(x, ElementColor.TryParse(x, out var color) ? color : null))
                .Where(x => x.Color != null)
                .ToArray();
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = Convert.ToString(value, CultureInfo.CurrentCulture);
            if (ElementColor.TryParse(Value, out var parsed))
            {
                currentColor = parsed;
            }
            inputValue = FormatColor(currentColor);

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

        public Task Update()
        {
            inputValue = FormatColor(currentColor);
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task OnHueInputAsync(ChangeEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            var next = currentColor.Clone();
            next.Hue = ParseDouble(e.Value, next.Hue);
            await CommitColorAsync(next, ValidateEvent, notifyChange: true);
        }

        private async Task OnSaturationInputAsync(ChangeEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            var next = currentColor.Clone();
            next.Saturation = Clamp(ParseDouble(e.Value, next.Saturation), 0, 100);
            await CommitColorAsync(next, ValidateEvent, notifyChange: true);
        }

        private async Task OnValueInputAsync(ChangeEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            var next = currentColor.Clone();
            next.Value = Clamp(ParseDouble(e.Value, next.Value), 0, 100);
            await CommitColorAsync(next, ValidateEvent, notifyChange: true);
        }

        private async Task OnAlphaInputAsync(ChangeEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            var next = currentColor.Clone();
            next.Alpha = Clamp(ParseDouble(e.Value, next.Alpha * 100), 0, 100) / 100;
            await CommitColorAsync(next, ValidateEvent, notifyChange: true);
        }

        private async Task OnSvPanelClickAsync(MouseEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            var next = currentColor.Clone();
            next.Saturation = Clamp(e.OffsetX / 280d * 100, 0, 100);
            next.Value = Clamp(100 - e.OffsetY / 180d * 100, 0, 100);
            await CommitColorAsync(next, ValidateEvent, notifyChange: true);
        }

        private async Task OnHueSliderClickAsync(MouseEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            var next = currentColor.Clone();
            next.Hue = Clamp(e.OffsetX / 280d * 360, 0, 360);
            await CommitColorAsync(next, ValidateEvent, notifyChange: true);
        }

        private async Task OnAlphaSliderClickAsync(MouseEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            var next = currentColor.Clone();
            next.Alpha = Clamp(e.OffsetX / 280d, 0, 1);
            await CommitColorAsync(next, ValidateEvent, notifyChange: true);
        }

        private Task OnColorTextInputAsync(ChangeEventArgs e)
        {
            inputValue = Convert.ToString(e.Value, CultureInfo.CurrentCulture);
            return Task.CompletedTask;
        }

        private async Task OnColorTextChangeAsync(ChangeEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            var text = Convert.ToString(e.Value, CultureInfo.CurrentCulture);
            inputValue = text;
            if (!ElementColor.TryParse(text, out var parsed))
            {
                return;
            }

            await CommitColorAsync(parsed, ValidateEvent, notifyChange: true);
        }

        private Task SelectPredefineAsync(PredefinedColor item)
        {
            if (effectiveDisabled || item?.Color == null)
            {
                return Task.CompletedTask;
            }

            return CommitColorAsync(item.Color.Clone(), ValidateEvent, notifyChange: true);
        }

        private async Task CommitColorAsync(ElementColor color, bool validate, bool notifyChange)
        {
            currentColor = color;
            Value = FormatColor(currentColor);
            inputValue = Value;
            SetFieldValue(Value, validate);

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
            if (OnActiveChange.HasDelegate)
            {
                await OnActiveChange.InvokeAsync(Value);
            }
            if (notifyChange && OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
        }

        private string FormatColor(ElementColor color)
        {
            return color?.ToCssString(ElementColor.ParseFormat(ColorFormat), ShowAlpha) ?? string.Empty;
        }

        private string PanelClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-color-picker__panel", "el-color-picker-panel", Cls)
            .AddIf(!Border, "is-borderless")
            .AddIf(effectiveDisabled, "is-disabled")
            .ToString();

        private string PanelStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add("position:relative")
            .Add(Style)
            .AddIf(!Border, "border-color:transparent", "box-shadow:none")
            .AddIf(effectiveDisabled, "pointer-events:none", "opacity:.6")
            .ToString();

        private string SvPanelStyle => $"background-color:{HueColor}";

        private string SvCursorStyle => string.Format(CultureInfo.InvariantCulture, "left:{0}%;top:{1}%;", currentColor.Saturation, 100 - currentColor.Value);

        private string HueSliderMergedStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add(HueSliderStyle)
            .ToString();

        private string HueThumbStyle => string.Format(CultureInfo.InvariantCulture, "left:{0}%;", currentColor.Hue / 360 * 100);

        private string AlphaThumbStyle => string.Format(CultureInfo.InvariantCulture, "left:{0}%;", currentColor.Alpha * 100);

        private string AlphaBarStyle => $"background:linear-gradient(to right, rgba({currentColor.Red}, {currentColor.Green}, {currentColor.Blue}, 0) 0%, {currentColor.ToAlphaGradientColor()} 100%)";

        private string SvRangeInputStyle => "position:absolute;left:0;top:0;right:0;width:100%;height:100%;opacity:0;pointer-events:auto";

        private string SliderRangeInputStyle => "position:absolute;left:0;top:0;width:100%;height:100%;opacity:0;pointer-events:auto";

        private string HueColor => new ElementColor(currentColor.Hue, 100, 100).ToOpaqueRgbString();

        private string InputValue => inputValue ?? FormatColor(currentColor);

        private int HueValue => (int)Math.Round(currentColor.Hue, MidpointRounding.AwayFromZero);

        private int SaturationValue => (int)Math.Round(currentColor.Saturation, MidpointRounding.AwayFromZero);

        private int BrightnessValue => (int)Math.Round(currentColor.Value, MidpointRounding.AwayFromZero);

        private int AlphaValue => (int)Math.Round(currentColor.Alpha * 100, MidpointRounding.AwayFromZero);

        private bool IsColorPickerPanelDisabled => effectiveDisabled;

        private IReadOnlyList<PredefinedColor> PredefinedColors => predefinedColors;

        private string GetPredefineClass(PredefinedColor item)
        {
            return HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-color-predefine__color-selector")
                .AddIf(item.Color.Alpha < 1, "is-alpha")
                .AddIf(currentColor.ColorEquals(item.Color, ShowAlpha), "selected")
                .ToString();
        }

        private string GetPredefineStyle(PredefinedColor item)
        {
            return $"background-color:{item.Color.ToCssString(ElementColorFormat.Rgb, true)}";
        }

        private static double ParseDouble(object value, double fallback)
        {
            return double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
                ? result
                : fallback;
        }

        private static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        private class PredefinedColor
        {
            public PredefinedColor(string value, ElementColor color)
            {
                Value = value;
                Color = color;
            }

            public string Value { get; }

            public ElementColor Color { get; }
        }
    }
}
