using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Element
{
    public partial class ElXBubbleList : ElementComponentBase
    {
        [Parameter]
        public IEnumerable<XMessageItem> Items { get; set; }

        [Parameter]
        public RenderFragment<XMessageItem> ItemTemplate { get; set; }

        [Parameter]
        public RenderFragment EmptyContent { get; set; }

        [Parameter]
        public string EmptyText { get; set; } = "No messages";

        [Parameter]
        public bool AutoScroll { get; set; } = true;

        protected string ListClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-bubble-list", Cls)
            .AddIf(AutoScroll, "is-auto-scroll")
            .ToString();
    }
}
