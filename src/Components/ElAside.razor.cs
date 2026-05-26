using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElAside : ElementComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Width { get; set; } = "300px";

        protected string AsideClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-aside", Cls)
            .ToString();

        protected string AsideStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Width), $"--el-aside-width:{ElementCssUtility.NormalizeCssSize(Width)}")
            .Add(Style)
            .ToString();
    }
}
