using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElTimeline : ElementComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string TimelineClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-timeline", Cls)
            .ToString();
    }
}
