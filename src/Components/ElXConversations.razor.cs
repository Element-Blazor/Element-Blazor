using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElXConversations : ElementComponentBase
    {
        [Parameter]
        public IEnumerable<XConversationItem> Items { get; set; } = Enumerable.Empty<XConversationItem>();

        [Parameter]
        public string ActiveId { get; set; }

        [Parameter]
        public EventCallback<string> ActiveIdChanged { get; set; }

        [Parameter]
        public EventCallback<XConversationItem> OnSelect { get; set; }

        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter]
        public RenderFragment Footer { get; set; }

        protected string ConversationsClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-conversations", Cls)
            .ToString();

        protected IEnumerable<IGrouping<string, XConversationItem>> GroupedItems => (Items ?? Enumerable.Empty<XConversationItem>())
            .GroupBy(x => x.Group ?? string.Empty);

        protected bool IsActive(XConversationItem item) => string.Equals(item?.Id, ActiveId, StringComparison.Ordinal);

        protected string GetItemClass(XConversationItem item, bool active) => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-conversations__item")
            .AddIf(active, "is-active")
            .AddIf(item?.Disabled == true, "is-disabled")
            .AddIf(item?.Pinned == true, "is-pinned")
            .ToString();

        private async Task SelectAsync(XConversationItem item)
        {
            if (item == null || item.Disabled)
            {
                return;
            }

            ActiveId = item.Id;
            if (ActiveIdChanged.HasDelegate)
            {
                await ActiveIdChanged.InvokeAsync(ActiveId);
            }
            if (OnSelect.HasDelegate)
            {
                await OnSelect.InvokeAsync(item);
            }
        }
    }
}
