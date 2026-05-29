using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;

namespace Element
{
    public partial class ElWatermark : ElementComponentBase
    {
        [Parameter]
        public int Width { get; set; } = 120;

        [Parameter]
        public int Height { get; set; } = 64;

        [Parameter]
        public int Rotate { get; set; } = -22;

        [Parameter]
        public int ZIndex { get; set; } = 9;

        [Parameter]
        public string Image { get; set; }

        [Parameter]
        public string Content { get; set; } = "Element Plus";

        [Parameter]
        public IEnumerable<string> ContentList { get; set; }

        [Parameter]
        public WatermarkFont Font { get; set; } = new WatermarkFont();

        [Parameter]
        public int[] Gap { get; set; } = new[] { 100, 100 };

        [Parameter]
        public int[] Offset { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private WatermarkFont ResolvedFont => Font ?? new WatermarkFont();

        private int ResolvedWidth => Math.Max(1, Width);

        private int ResolvedHeight => Math.Max(1, Height);

        private int GapX => GetPairValue(Gap, 0, 100);

        private int GapY => GetPairValue(Gap, 1, 100);

        private int OffsetX => GetPairValue(Offset, 0, GapX / 2);

        private int OffsetY => GetPairValue(Offset, 1, GapY / 2);

        private int TileWidth => ResolvedWidth + Math.Max(0, GapX);

        private int TileHeight => ResolvedHeight + Math.Max(0, GapY);

        protected string WatermarkClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-watermark", Cls)
            .ToString();

        protected string WatermarkStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add(Style)
            .ToString();

        protected string MaskStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add($"z-index:{ZIndex}")
            .Add($"background-size:{TileWidth}px {TileHeight}px")
            .Add($"background-position:{OffsetX}px {OffsetY}px")
            .Add($"background-image:{BuildBackgroundImage()}")
            .ToString();

        private string BuildBackgroundImage()
        {
            var svg = new StringBuilder();
            svg.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" ");
            svg.Append(CultureInfo.InvariantCulture, $"width=\"{TileWidth}\" height=\"{TileHeight}\" viewBox=\"0 0 {TileWidth} {TileHeight}\">");
            svg.Append(CultureInfo.InvariantCulture, $"<g transform=\"translate({ResolvedWidth / 2.0} {ResolvedHeight / 2.0}) rotate({Rotate})\">");

            if (!string.IsNullOrWhiteSpace(Image))
            {
                AppendImage(svg);
            }
            else
            {
                AppendText(svg);
            }

            svg.Append("</g></svg>");
            return $"url(\"data:image/svg+xml,{Uri.EscapeDataString(svg.ToString())}\")";
        }

        private void AppendImage(StringBuilder svg)
        {
            svg.Append(CultureInfo.InvariantCulture, $"<image href=\"{EscapeXml(Image)}\" ");
            svg.Append(CultureInfo.InvariantCulture, $"x=\"{-ResolvedWidth / 2.0}\" y=\"{-ResolvedHeight / 2.0}\" ");
            svg.Append(CultureInfo.InvariantCulture, $"width=\"{ResolvedWidth}\" height=\"{ResolvedHeight}\" preserveAspectRatio=\"xMidYMid meet\" />");
        }

        private void AppendText(StringBuilder svg)
        {
            var lines = ResolveTextLines();
            if (lines.Count == 0)
            {
                return;
            }

            var font = ResolvedFont;
            var fontSize = NormalizeFontSize(font.FontSize);
            var fontSizeValue = ResolveFontSizeNumber(font.FontSize);
            var lineHeight = fontSizeValue + Math.Max(0, font.FontGap);
            var startY = -((lines.Count - 1) * lineHeight) / 2;
            var textAnchor = ResolveTextAnchor(font.TextAlign);
            var x = ResolveTextX(textAnchor);

            for (var i = 0; i < lines.Count; i++)
            {
                svg.Append(CultureInfo.InvariantCulture, $"<text x=\"{x}\" y=\"{startY + (i * lineHeight)}\" ");
                svg.Append(CultureInfo.InvariantCulture, $"fill=\"{EscapeXml(font.Color)}\" font-size=\"{EscapeXml(fontSize)}\" ");
                svg.Append(CultureInfo.InvariantCulture, $"font-weight=\"{EscapeXml(font.FontWeight)}\" font-family=\"{EscapeXml(font.FontFamily)}\" ");
                svg.Append(CultureInfo.InvariantCulture, $"font-style=\"{EscapeXml(font.FontStyle)}\" text-anchor=\"{textAnchor}\" ");
                svg.Append(CultureInfo.InvariantCulture, $"dominant-baseline=\"{EscapeXml(font.TextBaseline)}\">");
                svg.Append(EscapeXml(lines[i]));
                svg.Append("</text>");
            }
        }

        private IReadOnlyList<string> ResolveTextLines()
        {
            if (ContentList != null)
            {
                return ContentList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            }

            return string.IsNullOrEmpty(Content)
                ? Array.Empty<string>()
                : new[] { Content };
        }

        private static int GetPairValue(IReadOnlyList<int> values, int index, int defaultValue)
        {
            return values != null && values.Count > index ? values[index] : defaultValue;
        }

        private static string NormalizeFontSize(object value)
        {
            if (value == null)
            {
                return "16px";
            }

            if (value is string text)
            {
                return ElementCssUtility.NormalizeCssSize(text);
            }

            if (IsNumeric(value))
            {
                return $"{Convert.ToString(value, CultureInfo.InvariantCulture)}px";
            }

            return value.ToString();
        }

        private static double ResolveFontSizeNumber(object value)
        {
            if (value == null)
            {
                return 16;
            }

            if (IsNumeric(value))
            {
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }

            var text = value.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return 16;
            }

            if (text.EndsWith("px", StringComparison.OrdinalIgnoreCase))
            {
                text = text.Substring(0, text.Length - 2);
            }

            return double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var result)
                ? result
                : 16;
        }

        private static bool IsNumeric(object value)
        {
            return value is byte
                || value is sbyte
                || value is short
                || value is ushort
                || value is int
                || value is uint
                || value is long
                || value is ulong
                || value is float
                || value is double
                || value is decimal;
        }

        private double ResolveTextX(string textAnchor)
        {
            if (textAnchor == "start")
            {
                return -ResolvedWidth / 2.0;
            }

            return textAnchor == "end" ? ResolvedWidth / 2.0 : 0;
        }

        private static string ResolveTextAnchor(string textAlign)
        {
            switch (textAlign?.ToLowerInvariant())
            {
                case "left":
                case "start":
                    return "start";
                case "right":
                case "end":
                    return "end";
                default:
                    return "middle";
            }
        }

        private static string EscapeXml(string value)
        {
            return SecurityElement.Escape(value) ?? string.Empty;
        }
    }
}
