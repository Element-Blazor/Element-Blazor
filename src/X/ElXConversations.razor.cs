using Element;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.X
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
        public EventCallback OnCreate { get; set; }

        [Parameter]
        public EventCallback<XConversationItem> OnRename { get; set; }

        [Parameter]
        public EventCallback<XConversationItem> OnDelete { get; set; }

        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter]
        public RenderFragment Footer { get; set; }

        [Parameter]
        public bool ShowCreate { get; set; }

        [Parameter]
        public bool ShowActions { get; set; }

        [Parameter]
        public bool ShowRename { get; set; } = true;

        [Parameter]
        public bool ShowDelete { get; set; } = true;

        [Parameter]
        public bool ConfirmDelete { get; set; }

        [Parameter]
        public string CreateText { get; set; } = "New conversation";

        [Parameter]
        public string RenameText { get; set; } = "Rename";

        [Parameter]
        public string DeleteText { get; set; } = "Delete";

        [Parameter]
        public string DeleteConfirmText { get; set; } = "Delete this conversation?";

        [Parameter]
        public string CreateIcon { get; set; } = "plus";

        [Parameter]
        public string RenameIcon { get; set; } = "edit";

        [Parameter]
        public string DeleteIcon { get; set; } = "delete";

        [Parameter]
        public string CreateAriaLabel { get; set; } = "Create conversation";

        protected string ConversationsClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-conversations", Cls)
            .AddIf(ShowActions && HasItemActions, "has-actions")
            .ToString();

        protected IEnumerable<IGrouping<string, XConversationItem>> GroupedItems => (Items ?? Enumerable.Empty<XConversationItem>())
            .GroupBy(x => x.Group ?? string.Empty);

        protected bool HasItemActions => (ShowRename && OnRename.HasDelegate) || (ShowDelete && OnDelete.HasDelegate);

        protected bool IsActive(XConversationItem item) => string.Equals(item?.Id, ActiveId, StringComparison.Ordinal);

        protected string GetRowClass(XConversationItem item, bool active) => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-conversations__row")
            .AddIf(active, "is-active")
            .AddIf(item?.Disabled == true, "is-disabled")
            .ToString();

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

        private async Task CreateAsync()
        {
            if (OnCreate.HasDelegate)
            {
                await OnCreate.InvokeAsync(null);
            }
        }

        private async Task RenameAsync(XConversationItem item)
        {
            if (item == null || item.Disabled || !OnRename.HasDelegate)
            {
                return;
            }

            await OnRename.InvokeAsync(item);
        }

        private async Task DeleteAsync(XConversationItem item)
        {
            if (item == null || item.Disabled || !OnDelete.HasDelegate)
            {
                return;
            }

            if (ConfirmDelete)
            {
                var result = await ConfirmAsync(DeleteConfirmText);
                if (result != MessageBoxResult.Confirm && result != MessageBoxResult.Ok)
                {
                    return;
                }
            }

            await OnDelete.InvokeAsync(item);
        }

        protected string GetRenameAriaLabel(XConversationItem item) => $"{RenameText} {item?.Title}".Trim();

        protected string GetDeleteAriaLabel(XConversationItem item) => $"{DeleteText} {item?.Title}".Trim();
    }
}
