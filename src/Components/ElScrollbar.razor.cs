using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElScrollbar : ElementComponentBase
    {
        private const decimal DefaultThumbSize = 20m;

        private decimal scrollTopPercentage;
        private decimal scrollLeftPercentage;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Height { get; set; }

        [Parameter]
        public string MaxHeight { get; set; }

        [Parameter]
        public bool Native { get; set; }

        [Parameter]
        public bool Noresize { get; set; }

        [Parameter]
        public string WrapStyle { get; set; }

        [Parameter]
        public string ViewStyle { get; set; }

        [Parameter]
        public string WrapClassName { get; set; }

        [Parameter]
        public string ViewClassName { get; set; }

        [Parameter]
        public bool Always { get; set; }

        [Parameter]
        public int MinSize { get; set; } = 20;

        [Parameter]
        public EventCallback OnScroll { get; set; }

        public ElementReference WrapElement { get; set; }

        protected string ScrollbarClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-scrollbar", Cls)
            .AddIf(Always, "is-always")
            .AddIf(Native, "is-native")
            .ToString();

        protected string ScrollbarStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Height), $"height:{ElementCssUtility.NormalizeCssSize(Height)}")
            .AddIf(!string.IsNullOrWhiteSpace(MaxHeight), $"max-height:{ElementCssUtility.NormalizeCssSize(MaxHeight)}")
            .Add(Style)
            .ToString();

        protected string WrapClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-scrollbar__wrap", WrapClassName)
            .AddIf(!Native, "el-scrollbar__wrap--hidden-default")
            .ToString();

        protected string ViewClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-scrollbar__view", ViewClassName)
            .ToString();

        protected string BarVisibilityStyle => Always ? "opacity:1" : null;

        protected string HorizontalThumbStyle => $"width:{ThumbSize.ToString("0.###", CultureInfo.InvariantCulture)}%;transform:translateX({scrollLeftPercentage.ToString("0.###", CultureInfo.InvariantCulture)}%)";

        protected string VerticalThumbStyle => $"height:{ThumbSize.ToString("0.###", CultureInfo.InvariantCulture)}%;transform:translateY({scrollTopPercentage.ToString("0.###", CultureInfo.InvariantCulture)}%)";

        private decimal ThumbSize => MinSize <= 0 ? DefaultThumbSize : MinSize;

        private async Task OnScrollAsync(EventArgs args)
        {
            var state = await JSRuntime.InvokeAsync<ScrollbarState>("elementScrollbarGetState", WrapElement);
            scrollTopPercentage = CalculateOffset(state.ScrollTop, state.ScrollHeight, state.ClientHeight);
            scrollLeftPercentage = CalculateOffset(state.ScrollLeft, state.ScrollWidth, state.ClientWidth);

            if (OnScroll.HasDelegate)
            {
                await OnScroll.InvokeAsync(null);
            }
        }

        public async Task ScrollToAsync(int top, int left = 0)
        {
            await JSRuntime.InvokeVoidAsync("elementScrollbarScrollTo", WrapElement, top, left);
        }

        public async Task SetScrollTopAsync(int top)
        {
            await JSRuntime.InvokeVoidAsync("elementScrollbarSetScrollTop", WrapElement, top);
        }

        public async Task SetScrollLeftAsync(int left)
        {
            await JSRuntime.InvokeVoidAsync("elementScrollbarSetScrollLeft", WrapElement, left);
        }

        private static decimal CalculateOffset(decimal current, decimal scrollSize, decimal clientSize)
        {
            var max = scrollSize - clientSize;
            return max <= 0 ? 0 : current / max * 100;
        }

        private sealed class ScrollbarState
        {
            public decimal ScrollTop { get; set; }
            public decimal ScrollLeft { get; set; }
            public decimal ScrollHeight { get; set; }
            public decimal ScrollWidth { get; set; }
            public decimal ClientHeight { get; set; }
            public decimal ClientWidth { get; set; }
        }
    }
}
