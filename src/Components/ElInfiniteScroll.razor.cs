using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElInfiniteScroll : ElementComponentBase
    {
        private DateTime lastLoadTime = DateTime.MinValue;
        private int loadCount;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback<InfiniteScrollEventArgs> OnLoad { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool Loading { get; set; }

        [Parameter]
        public int Delay { get; set; } = 200;

        [Parameter]
        public string Height { get; set; }

        [Parameter]
        public string MaxHeight { get; set; }

        [Parameter]
        public string LoadingText { get; set; } = "加载中";

        [Parameter]
        public string DisabledText { get; set; } = "没有更多了";

        [Parameter]
        public RenderFragment LoadingContent { get; set; }

        [Parameter]
        public RenderFragment DisabledContent { get; set; }

        protected string ScrollClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-infinite-scroll", Cls)
            .AddIf(Disabled, "is-disabled")
            .AddIf(Loading, "is-loading")
            .ToString();

        protected string ScrollStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Height), $"height:{ElementCssUtility.NormalizeCssSize(Height)}")
            .AddIf(!string.IsNullOrWhiteSpace(MaxHeight), $"max-height:{ElementCssUtility.NormalizeCssSize(MaxHeight)}")
            .AddIf(!string.IsNullOrWhiteSpace(Height) || !string.IsNullOrWhiteSpace(MaxHeight), "overflow:auto")
            .Add(Style)
            .ToString();

        protected async Task OnScrollAsync(EventArgs e)
        {
            if (Disabled || Loading || !OnLoad.HasDelegate)
            {
                return;
            }

            var now = DateTime.UtcNow;
            if ((now - lastLoadTime).TotalMilliseconds < Delay)
            {
                return;
            }

            lastLoadTime = now;
            loadCount++;
            await OnLoad.InvokeAsync(new InfiniteScrollEventArgs
            {
                ScrollEventArgs = e,
                LoadCount = loadCount
            });
        }
    }
}
