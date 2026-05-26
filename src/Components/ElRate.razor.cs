using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElRate : ElementFieldComponentBase<double?>
    {
        private HtmlPropertyBuilder wrapperClsBuilder;
        private bool effectiveDisabled;
        private double? hoverValue;

        [Parameter]
        public double? Value { get; set; }

        [Parameter]
        public double? ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<double?> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<double?> ModelValueChanged { get; set; }

        [Parameter]
        public int Max { get; set; } = 5;

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        [Parameter]
        public bool AllowHalf { get; set; }

        [Parameter]
        public bool Clearable { get; set; } = true;

        [Parameter]
        public bool ShowText { get; set; }

        [Parameter]
        public bool ShowScore { get; set; }

        [Parameter]
        public string ScoreTemplate { get; set; } = "{0}";

        [Parameter]
        public string[] Texts { get; set; } = new[] { "极差", "失望", "一般", "满意", "惊喜" };

        [Parameter]
        public string[] Colors { get; set; } = new[] { "#f7ba2a", "#f7ba2a", "#f7ba2a" };

        [Parameter]
        public string VoidColor { get; set; } = "#c6d1de";

        [Parameter]
        public string DisabledVoidColor { get; set; } = "#eff2f7";

        [Parameter]
        public string Icon { get; set; } = "el-icon-star-on";

        [Parameter]
        public string VoidIcon { get; set; } = "el-icon-star-off";

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public EventCallback<double?> OnChange { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            effectiveDisabled = Disabled || (FormItem?.Form?.Disabled ?? false);
            Max = Math.Max(1, Max);
            wrapperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-rate", Cls)
                .AddIf(effectiveDisabled, "is-disabled");

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    Value = ConvertToDouble(FormItem.OriginValue);
                }
                SetFieldValue(Value, false);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = ConvertToDouble(value);
            hoverValue = null;
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

        private async Task SelectRateAsync(int index)
        {
            if (effectiveDisabled)
            {
                return;
            }

            var next = hoverValue ?? index;
            if (Clearable && Value.HasValue && Math.Abs(Value.Value - next) < 0.001)
            {
                next = 0;
            }
            await CommitValueAsync(next <= 0 ? null : next);
        }

        private void HoverRate(int index, MouseEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            hoverValue = AllowHalf && e.OffsetX < 12 ? index - 0.5 : index;
        }

        private void ClearHover()
        {
            hoverValue = null;
        }

        private async Task CommitValueAsync(double? value)
        {
            Value = value;
            SetFieldValue(Value, ValidateEvent);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
        }

        private string GetIconClass(int index)
        {
            return IsActive(index) ? $"el-rate__icon {Icon}" : $"el-rate__icon {VoidIcon}";
        }

        private string GetIconStyle(int index)
        {
            var color = IsActive(index) ? ResolveColor(CurrentValue) : (effectiveDisabled ? DisabledVoidColor : VoidColor);
            return $"color:{color}";
        }

        private bool IsActive(int index)
        {
            return CurrentValue >= index || (AllowHalf && CurrentValue > index - 1 && CurrentValue < index);
        }

        private string ResolveColor(double? value)
        {
            var colors = Colors == null || Colors.Length == 0 ? new[] { "#f7ba2a" } : Colors;
            if (!value.HasValue)
            {
                return colors[0];
            }
            if (colors.Length == 1)
            {
                return colors[0];
            }
            var ratio = value.Value / Max;
            if (colors.Length >= 3)
            {
                if (ratio <= 0.4)
                {
                    return colors[0];
                }
                if (ratio <= 0.8)
                {
                    return colors[1];
                }
                return colors[2];
            }
            return ratio <= 0.5 ? colors[0] : colors[1];
        }

        private string DisplayText
        {
            get
            {
                if (ShowScore)
                {
                    return string.Format(CultureInfo.CurrentCulture, ScoreTemplate, CurrentValue?.ToString("0.0", CultureInfo.CurrentCulture) ?? "0");
                }
                var index = Math.Max(0, Math.Min((int)Math.Ceiling(CurrentValue ?? 0) - 1, (Texts?.Length ?? 0) - 1));
                return Texts != null && Texts.Length > 0 ? Texts[index] : string.Empty;
            }
        }

        private double? CurrentValue => hoverValue ?? Value;

        private bool IsRateDisabled => effectiveDisabled;

        private static double? ConvertToDouble(object value)
        {
            if (value == null)
            {
                return null;
            }
            return Convert.ToDouble(value, CultureInfo.CurrentCulture);
        }
    }
}
