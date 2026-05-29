using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElXThinking : ElementComponentBase
    {
        private bool expanded;

        [Parameter]
        public string Title { get; set; } = "Thinking";

        [Parameter]
        public string Content { get; set; }

        [Parameter]
        public string Duration { get; set; }

        [Parameter]
        public bool Loading { get; set; } = true;

        [Parameter]
        public bool DefaultExpanded { get; set; } = true;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string ThinkingClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-thinking", Cls)
            .AddIf(Loading, "is-loading")
            .AddIf(expanded, "is-expanded")
            .ToString();

        protected override void OnInitialized()
        {
            expanded = DefaultExpanded;
        }

        private void Toggle()
        {
            expanded = !expanded;
        }
    }
}
