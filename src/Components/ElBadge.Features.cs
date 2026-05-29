using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;

namespace Element
{
    public partial class ElBadge
    {
        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public int? Max { get; set; }

        [Parameter]
        public bool IsDot { get; set; }

        [Parameter]
        public bool Dot
        {
            get => IsDot;
            set => IsDot = value;
        }

        [Parameter]
        public bool Hidden { get; set; }

        [Parameter]
        public int? OffsetX { get; set; }

        [Parameter]
        public int? OffsetY { get; set; }

        [Parameter]
        public string Offset { get; set; }

        protected string DisplayValue => !string.IsNullOrWhiteSpace(Text)
            ? Text
            : Max.HasValue && Value > Max.Value
                ? $"{Max.Value}+"
                : Value.ToString(CultureInfo.InvariantCulture);

        protected string BadgeClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-badge", Cls)
            .ToString();

        protected string ContentClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-badge__content", $"el-badge__content--{TypeName}", "is-fixed")
            .AddIf(IsDot, "is-dot")
            .ToString();

        protected string ContentStyle
        {
            get
            {
                var builder = HtmlPropertyBuilder.CreateCssStyleBuilder();
                var (x, y) = ResolveOffset();
                if (x.HasValue)
                {
                    builder.Add($"right:{-x.Value}px");
                }

                if (y.HasValue)
                {
                    builder.Add($"margin-top:{y.Value}px");
                }

                return builder.ToString();
            }
        }

        private string TypeName => Convert.ToInt32(Type, CultureInfo.InvariantCulture) == 3
            ? "danger"
            : Type.ToString().ToLowerInvariant();

        private (int? x, int? y) ResolveOffset()
        {
            if (OffsetX.HasValue || OffsetY.HasValue)
            {
                return (OffsetX, OffsetY);
            }

            if (string.IsNullOrWhiteSpace(Offset))
            {
                return (null, null);
            }

            var parts = Offset.Split(',', ';', ' ');
            int? x = null;
            int? y = null;
            if (parts.Length > 0 && int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedX))
            {
                x = parsedX;
            }

            if (parts.Length > 1 && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedY))
            {
                y = parsedY;
            }

            return (x, y);
        }
    }
}
