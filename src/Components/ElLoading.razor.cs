using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElLoading : ElementComponentBase
    {
        [Parameter]
        public bool Loading { get; set; } = true;

        [Parameter]
        public string IconClass { get; set; }

        [Parameter]
        public string Text { get; set; } = "拼命加载中";

        [Parameter]
        public string Background { get; set; }

        [Parameter]
        public bool Fullscreen { get; set; }

        [Parameter]
        public bool Lock { get; set; }

        [Parameter]
        public RenderFragment Spinner { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private string WrapperClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-loading-parent")
            .AddIf(Loading, "is-loading")
            .Add(Cls)
            .ToString();

        private string WrapperStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add("position:relative")
            .Add(Style)
            .ToString();

        private string MaskClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-loading-mask")
            .AddIf(Fullscreen, "is-fullscreen")
            .ToString();

        private string MaskStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Background), $"background-color:{Background}")
            .ToString();
    }
}