using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElSplitterPanel : ElementComponentBase
    {
        private bool resizing;
        private double startPoint;
        private double currentBasis;
        private decimal? liveSize;

        [CascadingParameter]
        internal ElSplitter Splitter { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Size { get; set; }

        [Parameter]
        public string Min { get; set; }

        [Parameter]
        public string Max { get; set; }

        [Parameter]
        public bool Resizable { get; set; } = true;

        [Parameter]
        public bool Collapsible { get; set; }

        [Parameter]
        public EventCallback<string> OnResize { get; set; }

        protected string AriaOrientation => Splitter?.Direction == SplitterDirection.Vertical ? "horizontal" : "vertical";

        protected string PanelClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-splitter-panel", Cls)
            .AddIf(Collapsible, "is-collapsible")
            .ToString();

        protected string BarClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-splitter__bar")
            .AddIf(resizing, "is-active")
            .ToString();

        protected string PanelStyle
        {
            get
            {
                var size = liveSize.HasValue ? $"{liveSize.Value.ToString("0.###", CultureInfo.InvariantCulture)}px" : ElementCssUtility.NormalizeCssSize(Size);
                var horizontal = Splitter?.Direction != SplitterDirection.Vertical;
                return HtmlPropertyBuilder.CreateCssStyleBuilder()
                    .AddIf(!string.IsNullOrWhiteSpace(size), $"flex-basis:{size}")
                    .AddIf(!string.IsNullOrWhiteSpace(size) && horizontal, $"width:{size}")
                    .AddIf(!string.IsNullOrWhiteSpace(size) && !horizontal, $"height:{size}")
                    .AddIf(!string.IsNullOrWhiteSpace(Min), $"min-{SizeAxis}:{ElementCssUtility.NormalizeCssSize(Min)}")
                    .AddIf(!string.IsNullOrWhiteSpace(Max), $"max-{SizeAxis}:{ElementCssUtility.NormalizeCssSize(Max)}")
                    .Add(Style)
                    .ToString();
            }
        }

        private string SizeAxis => Splitter?.Direction == SplitterDirection.Vertical ? "height" : "width";

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Splitter?.RegisterPanel();
        }

        private void OnResizeStart(MouseEventArgs args)
        {
            resizing = true;
            startPoint = Splitter?.Direction == SplitterDirection.Vertical ? args.ClientY : args.ClientX;
            currentBasis = liveSize.HasValue ? (double)liveSize.Value : ParsePixelSize(Size, 200);
        }

        private async Task OnResizeMove(MouseEventArgs args)
        {
            if (!resizing)
            {
                return;
            }

            var point = Splitter?.Direction == SplitterDirection.Vertical ? args.ClientY : args.ClientX;
            var next = currentBasis + point - startPoint;
            if (next < 0)
            {
                next = 0;
            }

            liveSize = (decimal)next;
            if (OnResize.HasDelegate)
            {
                await OnResize.InvokeAsync($"{liveSize.Value.ToString("0.###", CultureInfo.InvariantCulture)}px");
            }
        }

        private void OnResizeEnd()
        {
            resizing = false;
        }

        private async Task OnDoubleClickAsync()
        {
            if (!Collapsible)
            {
                return;
            }

            liveSize = 0;
            if (OnResize.HasDelegate)
            {
                await OnResize.InvokeAsync("0px");
            }
        }

        private static double ParsePixelSize(string value, double fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            var normalized = ElementCssUtility.NormalizeCssSize(value);
            normalized = Regex.Replace(normalized, "[^0-9.\\-]", string.Empty);
            return double.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed) ? parsed : fallback;
        }
    }
}
