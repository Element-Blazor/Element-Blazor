using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Globalization;

namespace Element
{
    public partial class ElCol : ElementComponentBase
    {
        [CascadingParameter]
        internal ElRow Row { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public int Span { get; set; } = 24;

        [Parameter]
        public int Offset { get; set; }

        [Parameter]
        public int Push { get; set; }

        [Parameter]
        public int Pull { get; set; }

        [Parameter]
        public int? Xs { get; set; }

        [Parameter]
        public int? Sm { get; set; }

        [Parameter]
        public int? Md { get; set; }

        [Parameter]
        public int? Lg { get; set; }

        [Parameter]
        public int? Xl { get; set; }

        [Parameter]
        public string Tag { get; set; } = "div";

        protected IDictionary<string, object> ColParameters => new Dictionary<string, object>
        {
            [nameof(ElementTagRenderer.Tag)] = string.IsNullOrWhiteSpace(Tag) ? "div" : Tag,
            [nameof(ElementTagRenderer.ChildContent)] = ChildContent,
            [nameof(ElementTagRenderer.Attributes)] = Attributes,
            [nameof(ElementTagRenderer.Class)] = ColClass,
            [nameof(ElementTagRenderer.Style)] = ColStyle
        };

        protected string ColClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-col", $"el-col-{NormalizeSpan(Span)}", Cls)
            .AddIf(Offset > 0, $"el-col-offset-{NormalizeSpan(Offset)}")
            .AddIf(Push > 0, $"el-col-push-{NormalizeSpan(Push)}")
            .AddIf(Pull > 0, $"el-col-pull-{NormalizeSpan(Pull)}")
            .AddIf(Xs.HasValue, $"el-col-xs-{NormalizeSpan(Xs)}")
            .AddIf(Sm.HasValue, $"el-col-sm-{NormalizeSpan(Sm)}")
            .AddIf(Md.HasValue, $"el-col-md-{NormalizeSpan(Md)}")
            .AddIf(Lg.HasValue, $"el-col-lg-{NormalizeSpan(Lg)}")
            .AddIf(Xl.HasValue, $"el-col-xl-{NormalizeSpan(Xl)}")
            .ToString();

        protected string ColStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(Row?.Gutter > 0, $"padding-left:{(Row.Gutter / 2m).ToString("0.###", CultureInfo.InvariantCulture)}px", $"padding-right:{(Row.Gutter / 2m).ToString("0.###", CultureInfo.InvariantCulture)}px")
            .Add(Style)
            .ToString();

        private static int NormalizeSpan(int? value)
        {
            if (!value.HasValue)
            {
                return 0;
            }

            if (value.Value < 0)
            {
                return 0;
            }

            if (value.Value > 24)
            {
                return 24;
            }

            return value.Value;
        }
    }
}
