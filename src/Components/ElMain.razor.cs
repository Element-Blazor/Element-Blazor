using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElMain : ElementComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string MainClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-main", Cls)
            .ToString();

        protected string MainStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add(Style)
            .ToString();
    }
}
