using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElHeader : ElementComponentBase
    {
        [CascadingParameter]
        internal ElContainer Container { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Height { get; set; } = "60px";

        protected string HeaderClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-header", Cls)
            .ToString();

        protected string HeaderStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Height), $"--el-header-height:{ElementCssUtility.NormalizeCssSize(Height)}")
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
