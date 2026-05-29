using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElEmpty : ElementComponentBase
    {
        private readonly string gradientId = $"el-empty-{System.Guid.NewGuid():N}";

        [Parameter]
        public string Image { get; set; }

        [Parameter]
        public string ImageSize { get; set; }

        [Parameter]
        public string Description { get; set; } = "No Data";

        [Parameter]
        public RenderFragment ImageContent { get; set; }

        [Parameter]
        public RenderFragment DescriptionContent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string EmptyClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-empty", Cls)
            .ToString();

        protected string ImageStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(ImageSize), $"width:{ElementCssUtility.NormalizeCssSize(ImageSize)}")
            .ToString();

        protected string GradientId => gradientId;
    }
}
