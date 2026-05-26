using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElSplitter : ElementComponentBase
    {
        private int nextPanelIndex;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public SplitterDirection Direction { get; set; } = SplitterDirection.Horizontal;

        public ElementReference RootElement { get; set; }

        protected string SplitterClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-splitter", Cls)
            .Add($"el-splitter--{Direction.ToString().ToLowerInvariant()}")
            .ToString();

        protected string SplitterStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add(Style)
            .ToString();

        internal int RegisterPanel()
        {
            nextPanelIndex++;
            return nextPanelIndex;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("elementSplitterInit", RootElement);
            }
        }
    }
}
