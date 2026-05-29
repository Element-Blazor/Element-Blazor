using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;

namespace Element
{
    public partial class ElStatistic : ElementComponentBase
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment TitleContent { get; set; }

        [Parameter]
        public double Value { get; set; }

        [Parameter]
        public string ValueStyle { get; set; }

        [Parameter]
        public int Precision { get; set; }

        [Parameter]
        public string Format { get; set; }

        [Parameter]
        public string PrefixText { get; set; }

        [Parameter]
        public RenderFragment Prefix { get; set; }

        [Parameter]
        public string SuffixText { get; set; }

        [Parameter]
        public RenderFragment Suffix { get; set; }

        [Parameter]
        public CultureInfo Culture { get; set; }

        protected string StatisticClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-statistic", Cls)
            .ToString();

        protected string DisplayValue
        {
            get
            {
                var culture = Culture ?? CultureInfo.CurrentCulture;
                if (!string.IsNullOrWhiteSpace(Format))
                {
                    return string.Format(culture, Format, Value);
                }

                return Value.ToString($"N{Math.Max(0, Precision)}", culture);
            }
        }
    }
}
