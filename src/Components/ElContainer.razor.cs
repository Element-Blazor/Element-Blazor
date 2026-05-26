using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElContainer : ElementComponentBase
    {
        private int headerFooterCount;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public ContainerDirection Direction { get; set; } = ContainerDirection.Auto;

        protected string ContainerClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-container", Cls)
            .AddIf(IsVertical, "is-vertical")
            .ToString();

        protected string ContainerStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add(Style)
            .ToString();

        private bool IsVertical => Direction == ContainerDirection.Vertical ||
            (Direction == ContainerDirection.Auto && headerFooterCount > 0);

        internal void RegisterHeaderOrFooter()
        {
            headerFooterCount++;
            _ = InvokeAsync(StateHasChanged);
        }

        internal void UnregisterHeaderOrFooter()
        {
            if (headerFooterCount > 0)
            {
                headerFooterCount--;
                _ = InvokeAsync(StateHasChanged);
            }
        }
    }
}
