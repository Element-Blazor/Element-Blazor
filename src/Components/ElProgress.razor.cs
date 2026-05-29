using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;

namespace Element
{
    public partial class ElProgress : ElementComponentBase
    {
        [Parameter]
        public double Percentage { get; set; }

        [Parameter]
        public ProgressType Type { get; set; } = ProgressType.Line;

        [Parameter]
        public ProgressStatus Status { get; set; } = ProgressStatus.None;

        [Parameter]
        public double StrokeWidth { get; set; } = 6;

        [Parameter]
        public bool TextInside { get; set; }

        [Parameter]
        public bool ShowText { get; set; } = true;

        [Parameter]
        public int Width { get; set; } = 126;

        [Parameter]
        public string Color { get; set; }

        [Parameter]
        public string TrackColor { get; set; } = "#e5e9f2";

        [Parameter]
        public string Format { get; set; }

        [Parameter]
        public string StrokeLinecap { get; set; } = "round";

        [Parameter]
        public RenderFragment<double> Content { get; set; }

        protected double SafePercentage => Math.Max(0, Math.Min(100, Percentage));

        protected string ProgressClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-progress", $"el-progress--{Type.ToString().ToLower()}", Cls)
            .AddIf(Status != ProgressStatus.None, $"is-{Status.ToString().ToLower()}")
            .AddIf(!ShowText, "el-progress--without-text")
            .AddIf(TextInside, "el-progress--text-inside")
            .ToString();

        protected string OuterStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add($"height:{StrokeWidth.ToString("0.###", CultureInfo.InvariantCulture)}px")
            .AddIf(!string.IsNullOrWhiteSpace(TrackColor), $"background-color:{TrackColor}")
            .ToString();

        protected string InnerStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add($"width:{SafePercentage.ToString("0.###", CultureInfo.InvariantCulture)}%")
            .AddIf(!string.IsNullOrWhiteSpace(Color), $"background-color:{Color}")
            .ToString();

        protected string CircleStyle => $"height:{Width}px;width:{Width}px";

        protected string TextStyle => Type == ProgressType.Line
            ? string.Empty
            : $"font-size:{Math.Max(12, Width * 0.111111).ToString("0.###", CultureInfo.InvariantCulture)}px";

        protected string StrokeColorValue => !string.IsNullOrWhiteSpace(Color)
            ? Color
            : Status switch
            {
                ProgressStatus.Success => "#67c23a",
                ProgressStatus.Exception => "#f56c6c",
                ProgressStatus.Warning => "#e6a23c",
                _ => "#409eff"
            };

        protected string TrackPath
        {
            get
            {
                if (Type == ProgressType.Dashboard)
                {
                    return "M 50 50 m 0 47 a 47 47 0 1 1 0 -94 a 47 47 0 1 1 0 94";
                }

                return "M 50 50 m 0 -47 a 47 47 0 1 1 0 94 a 47 47 0 1 1 0 -94";
            }
        }

        protected string CirclePathStyle
        {
            get
            {
                var perimeter = Math.PI * 2 * 47;
                var rate = Type == ProgressType.Dashboard ? 0.75 : 1;
                var dashArray = $"{(perimeter * rate).ToString("0.###", CultureInfo.InvariantCulture)}px, {perimeter.ToString("0.###", CultureInfo.InvariantCulture)}px";
                var dashOffset = ((1 - SafePercentage / 100) * perimeter * rate).ToString("0.###", CultureInfo.InvariantCulture);
                return $"stroke-dasharray:{dashArray};stroke-dashoffset:{dashOffset}px";
            }
        }

        protected RenderFragment TextContent
        {
            get
            {
                if (Content != null)
                {
                    return builder => builder.AddContent(0, Content(SafePercentage));
                }

                return builder => builder.AddContent(0, string.IsNullOrWhiteSpace(Format)
                    ? $"{SafePercentage.ToString("0.###", CultureInfo.InvariantCulture)}%"
                    : string.Format(CultureInfo.CurrentCulture, Format, SafePercentage));
            }
        }
    }
}
