using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElBreadcrumb : ElementComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Separator { get; set; } = "/";

        [Parameter]
        public string SeparatorIcon { get; set; }

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
