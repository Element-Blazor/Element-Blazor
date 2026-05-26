using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElFooter : ElementComponentBase
    {
        [CascadingParameter]
        internal ElContainer Container { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Height { get; set; } = "60px";

        protected string FooterClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-footer", Cls)
            .ToString();

        protected string FooterStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Height), $"--el-footer-height:{ElementCssUtility.NormalizeCssSize(Height)}")
            .Add(Style)
            .ToString();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Container?.RegisterHeaderOrFooter();
        }

        public override void Dispose()
        {
            Container?.UnregisterHeaderOrFooter();
            base.Dispose();
        }
    }
}
