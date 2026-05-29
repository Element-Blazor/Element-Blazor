using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElXWelcome : ElementComponentBase
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Description { get; set; }

        [Parameter]
        public string Icon { get; set; } = "magic-stick";

        [Parameter]
        public string Avatar { get; set; }

        [Parameter]
        public RenderFragment IconContent { get; set; }

        [Parameter]
        public RenderFragment Actions { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string WelcomeClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-welcome", Cls)
            .ToString();
    }
}
