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
    public partial class ElSlider : ElementFieldComponentBase<double>
    {
        private static long inputIdSeed;
        private readonly string generatedInputId = $"el-slider-{Interlocked.Increment(ref inputIdSeed)}";
        private HtmlPropertyBuilder wrapperClsBuilder;
        private bool effectiveDisabled;
        private int activeThumbIndex = -1;

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
        public bool ShowTooltip { get; set; } = true;

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public bool Range { get; set; }

        [Parameter]
        public IList<double> RangeValue { get; set; } = new List<double>();

        [Parameter]
        public EventCallback<IList<double>> RangeValueChanged { get; set; }

        [Parameter]
        public Func<double, string> FormatTooltip { get; set; }

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
            if (Range)
            {
                NormalizeRange();
            }
            else
            {
                Value = Normalize(Value);
            }
            Id = string.IsNullOrWhiteSpace(Id) ? ResolveAttributeId() ?? generatedInputId : Id;
            if (FormItem?.Form != null)
            {
                FormItem.Form.RegisterInput(FormItem.Name, InputId, this, FormItem);
            }
            wrapperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-slider", Cls)
                .AddIf(effectiveDisabled, "is-disabled")
                .AddIf(Range, "is-range")
                .AddIf(ShowInput, "el-slider--with-input");

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    if (Range)
                    {
                        RangeValue = NormalizeRangeValue(FormItem.OriginValue);
                    }
                    else
                    {
                        Value = Convert.ToDouble(FormItem.OriginValue, CultureInfo.CurrentCulture);
                    }
                }
                SetFieldValue(Value, false);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            if (Range)
            {
                RangeValue = NormalizeRangeValue(value);
                Value = RangeValue.LastOrDefault();
                if (RangeValueChanged.HasDelegate)
                {
                    _ = RangeValueChanged.InvokeAsync(RangeValue);
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
                return;
            }

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
            if (IsSliderDisabled)
            {
                return;
            }

            var next = ParseValue(e.Value);
            await CommitValueAsync(next, validate: false, notifyChange: false);
            if (OnInput.HasDelegate)
            {
                await OnInput.InvokeAsync(Value);
            }
        }

        private async Task OnChangeAsync(ChangeEventArgs e)
        {
            if (IsSliderDisabled)
            {
                return;
            }

            await CommitValueAsync(ParseValue(e.Value), ValidateEvent, notifyChange: true);
        }

        private async Task OnRangeInputAsync(int index, ChangeEventArgs e)
        {
            if (IsSliderDisabled)
            {
                return;
            }

            activeThumbIndex = index;
            await CommitRangeValueAsync(index, ParseValue(e.Value), validate: false, notifyChange: false);
            if (OnInput.HasDelegate)
            {
                await OnInput.InvokeAsync(Value);
            }
        }

        private async Task OnRangeChangeAsync(int index, ChangeEventArgs e)
        {
            if (IsSliderDisabled)
            {
                return;
            }

            activeThumbIndex = index;
            await CommitRangeValueAsync(index, ParseValue(e.Value), ValidateEvent, notifyChange: true);
        }

        private async Task OnInputNumberChangedAsync(decimal? value)
        {
            if (IsSliderDisabled)
            {
                return;
            }

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

        private async Task CommitRangeValueAsync(int index, double next, bool validate, bool notifyChange)
        {
            var range = SetRangeThumbValue(index, next);
            RangeValue = range;
            Value = range.LastOrDefault();
            SetFieldValue(Value, validate);
            if (RangeValueChanged.HasDelegate)
            {
                await RangeValueChanged.InvokeAsync(RangeValue);
            }
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

        private IList<double> SetRangeThumbValue(int index, double next)
        {
            var range = CurrentRange.ToList();
            if (index < 0 || index >= range.Count)
            {
                index = 0;
            }
            range[index] = Normalize(next);
            if (range[0] > range[1])
            {
                range.Sort();
                activeThumbIndex = index == 0 ? 1 : 0;
            }
            else
            {
                activeThumbIndex = index;
            }
            return range;
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

        private IReadOnlyList<double> CurrentRange
        {
            get
            {
                var source = RangeValue ?? new List<double>();
                var start = source.Count > 0 ? source[0] : Min;
                var end = source.Count > 1 ? source[1] : Max;
                return new[] { Normalize(start), Normalize(end) }.OrderBy(x => x).ToList();
            }
        }

        private string RangeBarStyle => $"left:{GetPercent(CurrentRange[0])}%;width:{GetPercent(CurrentRange[1]) - GetPercent(CurrentRange[0])}%;";

        private string GetRangeButtonStyle(int index) => $"left:{GetPercent(CurrentRange[index])}%;";

        private string GetTooltipText(double value)
        {
            return FormatTooltip != null
                ? FormatTooltip(value)
                : value.ToString(CultureInfo.CurrentCulture);
        }

        private string ResolveAttributeId()
        {
            if (Attributes == null)
            {
                return null;
            }

            return Attributes.TryGetValue("id", out var id) ? Convert.ToString(id, CultureInfo.InvariantCulture) : null;
        }

        private string GetInputId(int index) => index == 0 ? InputId : $"{InputId}-{index + 1}";

        private string InputId => Id;

        private string AriaDisabled => IsSliderDisabled ? "true" : "false";

        private string AriaInvalid => IsAriaInvalid ? "true" : "false";

        private static string FormatInvariant(double value) => value.ToString(CultureInfo.InvariantCulture);

        private decimal? DecimalValue => (decimal)Value;

        private bool IsSliderDisabled => effectiveDisabled;

        private void NormalizeRange()
        {
            RangeValue = CurrentRange.ToList();
            Value = RangeValue.LastOrDefault();
        }

        private IList<double> NormalizeRangeValue(object value)
        {
            if (value is IEnumerable<double> doubleValues)
            {
                return doubleValues.Select(Normalize).OrderBy(x => x).Take(2).ToList();
            }
            if (value is System.Collections.IEnumerable enumerable && value is not string)
            {
                return enumerable.Cast<object>()
                    .Select(x => Convert.ToDouble(x, CultureInfo.CurrentCulture))
                    .Select(Normalize)
                    .OrderBy(x => x)
                    .Take(2)
                    .ToList();
            }
            return new List<double> { Min, Max };
        }
    }
}
