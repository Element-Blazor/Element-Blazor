using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElSegmented<TValue> : ElementComponentBase
    {
        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<TValue> OnChange { get; set; }

        [Parameter]
        public IEnumerable<SegmentedOption> Options { get; set; }

        [Parameter]
        public IEnumerable<TValue> Items { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool Block { get; set; }

        [Parameter]
        public ElementSize Size { get; set; } = ElementSize.Default;

        protected IReadOnlyList<SegmentedOption> ResolvedOptions => Options?.ToList()
            ?? Items?.Select(x => new SegmentedOption
            {
                Label = Convert.ToString(x, CultureInfo.CurrentCulture),
                Value = Convert.ToString(x, CultureInfo.InvariantCulture)
            }).ToList()
            ?? new List<SegmentedOption>();

        protected string SegmentedClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-segmented", Cls)
            .AddIf(Block, "is-block")
            .AddIf(Disabled, "is-disabled")
            .AddIf(Size == ElementSize.Large, "el-segmented--large")
            .AddIf(Size == ElementSize.Small, "el-segmented--small")
            .ToString();

        protected string GetItemClass(SegmentedOption option, bool selected)
        {
            return HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-segmented__item")
                .AddIf(selected, "is-selected")
                .AddIf(Disabled || option.Disabled, "is-disabled")
                .ToString();
        }

        protected bool IsSelected(SegmentedOption option)
        {
            return EqualityComparer<TValue>.Default.Equals(Value, ConvertValue(option.Value));
        }

        protected async Task SelectAsync(SegmentedOption option)
        {
            if (Disabled || option.Disabled)
            {
                return;
            }

            var next = ConvertValue(option.Value);
            if (EqualityComparer<TValue>.Default.Equals(Value, next))
            {
                return;
            }

            Value = next;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(next);
            }

            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(next);
            }
        }

        private static TValue ConvertValue(string value)
        {
            if (typeof(TValue) == typeof(string))
            {
                return (TValue)(object)value;
            }

            if (typeof(TValue).IsEnum)
            {
                return (TValue)Enum.Parse(typeof(TValue), value);
            }

            var targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
            return (TValue)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
    }
}
