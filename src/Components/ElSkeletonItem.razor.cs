using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElSkeletonItem : ElementComponentBase
    {
        [Parameter]
        public SkeletonItemVariant Variant { get; set; } = SkeletonItemVariant.Text;

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public string Height { get; set; }

        protected string ItemClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-skeleton__item", $"el-skeleton__{Variant.ToString().ToLower()}", Cls)
            .ToString();

        protected string ItemStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Width), $"width:{ElementCssUtility.NormalizeCssSize(Width)}")
            .AddIf(!string.IsNullOrWhiteSpace(Height), $"height:{ElementCssUtility.NormalizeCssSize(Height)}")
            .Add(Style)
            .ToString();
    }
}
