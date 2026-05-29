using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElImage : ElementComponentBase
    {
        private bool loadFailed;
        private bool showViewer;
        private int previewIndex;

        [Parameter]
        public string Src { get; set; }

        [Parameter]
        public string Alt { get; set; }

        [Parameter]
        public string Fit { get; set; }

        [Parameter]
        public bool Lazy { get; set; }

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public string Height { get; set; }

        [Parameter]
        public RenderFragment Placeholder { get; set; }

        [Parameter]
        public RenderFragment ErrorContent { get; set; }

        [Parameter]
        public string ErrorText { get; set; } = "加载失败";

        [Parameter]
        public IList<string> PreviewSrcList { get; set; }

        [Parameter]
        public bool PreviewTeleported { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public EventCallback OnError { get; set; }

        protected bool ShowViewer => showViewer && PreviewList.Any();

        protected IReadOnlyList<string> PreviewList => PreviewSrcList != null && PreviewSrcList.Any()
            ? PreviewSrcList.Where(x => !string.IsNullOrWhiteSpace(x)).ToList()
            : string.IsNullOrWhiteSpace(Src) ? new List<string>() : new List<string> { Src };

        protected string CurrentPreviewSrc => PreviewList.Count == 0 ? Src : PreviewList[previewIndex];

        protected string LoadingMode => Lazy ? "lazy" : null;

        protected string ImageClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-image", Cls)
            .ToString();

        protected string WrapperStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Width), $"width:{ElementCssUtility.NormalizeCssSize(Width)}")
            .AddIf(!string.IsNullOrWhiteSpace(Height), $"height:{ElementCssUtility.NormalizeCssSize(Height)}")
            .Add(Style)
            .ToString();

        protected string ImageStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Fit), $"object-fit:{Fit}")
            .ToString();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            previewIndex = 0;
            if (!string.IsNullOrWhiteSpace(Src))
            {
                loadFailed = false;
            }
        }

        protected async Task OnErrorAsync()
        {
            loadFailed = true;
            if (OnError.HasDelegate)
            {
                await OnError.InvokeAsync(null);
            }
        }

        protected async Task OpenPreviewAsync(MouseEventArgs e)
        {
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(e);
            }

            if (!PreviewList.Any())
            {
                return;
            }

            showViewer = true;
        }

        protected Task ClosePreviewAsync()
        {
            showViewer = false;
            return Task.CompletedTask;
        }

        protected Task ShowPreviousAsync()
        {
            if (PreviewList.Count > 0)
            {
                previewIndex = (previewIndex - 1 + PreviewList.Count) % PreviewList.Count;
            }

            return Task.CompletedTask;
        }

        protected Task ShowNextAsync()
        {
            if (PreviewList.Count > 0)
            {
                previewIndex = (previewIndex + 1) % PreviewList.Count;
            }

            return Task.CompletedTask;
        }
    }
}
