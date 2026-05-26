using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElSlider : ElementFieldComponentBase<double>
    {
        private HtmlPropertyBuilder wrapperClsBuilder;
        private bool effectiveDisabled;

        [Parameter]
        public double Value { get; set; }

        [Parameter]
        public double ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<double> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<double> ModelValueChanged { get; set; }

        [Parameter]
        public double Min { get; set; }

        [Parameter]
        public double Max { get; set; } = 100;

        [Parameter]
        public double Step { get; set; } = 1;

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        [Parameter]
        public bool ShowStops { get; set; }

        [Parameter]
        public bool ShowInput { get; set; }

        [Parameter]
        public IEnumerable<SliderMark> Marks { get; set; }

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public EventCallback<double> OnInput { get; set; }

        [Parameter]
        public EventCallback<double> OnChange { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            effectiveDisabled = Disabled || (FormItem?.Form?.Disabled ?? false);
            if (Max <= Min)
            {
                Max = Min + 1;
            }
            Step = Step <= 0 ? 1 : Step;
            Value = Normalize(Value);
            wrapperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-slider", Cls)
                .AddIf(effectiveDisabled, "is-disabled")
                .AddIf(ShowInput, "el-slider--with-input");

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    Value = Convert.ToDouble(FormItem.OriginValue, CultureInfo.CurrentCulture);
                }
                SetFieldValue(Value, false);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = value == null ? Min : Convert.ToDouble(value, CultureInfo.CurrentCulture);
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
            var next = ParseValue(e.Value);
            await CommitValueAsync(next, validate: false, notifyChange: false);
            if (OnInput.HasDelegate)
            {
                await OnInput.InvokeAsync(Value);
            }
        }

        private async Task OnChangeAsync(ChangeEventArgs e)
        {
            await CommitValueAsync(ParseValue(e.Value), ValidateEvent, notifyChange: true);
        }

        private async Task OnInputNumberChangedAsync(decimal? value)
        {
            await CommitValueAsync(value.HasValue ? (double)value.Value : Min, ValidateEvent, notifyChange: true);
        }

        private Task OnRunwayClickAsync(MouseEventArgs e)
        {
            return Task.CompletedTask;
        }

        private async Task CommitValueAsync(double next, bool validate, bool notifyChange)
        {
            Value = Normalize(next);
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

        private double Normalize(double value)
        {
            var clamped = Math.Max(Min, Math.Min(Max, value));
            var steps = Math.Round((clamped - Min) / Step, 0, MidpointRounding.AwayFromZero);
            return Math.Max(Min, Math.Min(Max, Min + steps * Step));
        }

        private double ParseValue(object value)
        {
            return double.TryParse(Convert.ToString(value), NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
                ? result
                : Value;
        }

        private double GetPercent(double value)
        {
            return (Math.Max(Min, Math.Min(Max, value)) - Min) / (Max - Min) * 100;
        }

        private IEnumerable<double> Stops
        {
            get
            {
                var count = (int)Math.Floor((Max - Min) / Step);
                for (var i = 1; i < count; i++)
                {
                    yield return i * Step / (Max - Min) * 100;
                }
            }
        }

        private string BarStyle => $"width:{GetPercent(Value)}%;";

        private string ButtonStyle => $"left:{GetPercent(Value)}%;";

        private decimal? DecimalValue => (decimal)Value;

        private bool IsSliderDisabled => effectiveDisabled;
    }
}
