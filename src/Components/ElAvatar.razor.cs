using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElAvatar : ElementComponentBase
    {
        [Parameter]
        public string Src { get; set; }

        [Parameter]
        public string Alt { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public RenderFragment IconContent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public ElementSize Size { get; set; } = ElementSize.Default;

        [Parameter]
        public string CustomSize { get; set; }

        [Parameter]
        public AvatarShape Shape { get; set; } = AvatarShape.Circle;

        [Parameter]
        public string Fit { get; set; } = "cover";

        protected string AvatarClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-avatar", $"el-avatar--{Shape.ToString().ToLower()}", Cls)
            .AddIf(Size == ElementSize.Large, "el-avatar--large")
            .AddIf(Size == ElementSize.Small, "el-avatar--small")
            .AddIf(!string.IsNullOrWhiteSpace(Icon) || IconContent != null, "el-avatar--icon")
            .ToString();

        protected string AvatarStyle
        {
            get
            {
                var builder = HtmlPropertyBuilder.CreateCssStyleBuilder()
                    .Add(Style);

                if (!string.IsNullOrWhiteSpace(CustomSize))
                {
                    var size = ElementCssUtility.NormalizeCssSize(CustomSize);
                    builder.Add($"width:{size}", $"height:{size}", $"line-height:{size}");
                }

                return builder.ToString();
            }
        }

        protected string ImageStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add("width:100%")
            .AddIf(!string.IsNullOrWhiteSpace(Fit), $"object-fit:{Fit}")
            .ToString();

        protected string IconClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add(NormalizeIcon(Icon))
            .ToString();

        private static string NormalizeIcon(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var trimmed = value.Trim();
            return trimmed.StartsWith("el-icon-") ? trimmed : $"el-icon-{trimmed}";
        }
    }
}
