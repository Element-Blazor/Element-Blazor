using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.X
{
    public partial class ElXConversations : ElementComponentBase
    {
        private string editingId;
        private string editingTitle;
        private string createTitle;
        private bool creating;

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
        public EventCallback<XConversationCreateEventArgs> OnCreateConversation { get; set; }

        [Parameter]
        public EventCallback<XConversationItem> OnRename { get; set; }

        [Parameter]
        public EventCallback<XConversationRenameEventArgs> OnRenameConversation { get; set; }

        [Parameter]
        public EventCallback<XConversationItem> OnDelete { get; set; }

        [Parameter]
        public EventCallback<XConversationDeleteEventArgs> OnDeleteConversation { get; set; }

        [Parameter]
        public EventCallback<IEnumerable<XConversationItem>> ItemsChanged { get; set; }

        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter]
        public RenderFragment Footer { get; set; }

        [Parameter]
        public RenderFragment<XConversationItem> ItemTemplate { get; set; }

        [Parameter]
        public RenderFragment<XConversationItem> ActionTemplate { get; set; }

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
        public bool Editable { get; set; }

        [Parameter]
        public bool CreateInline { get; set; }

        [Parameter]
        public bool RenameInline { get; set; }

        [Parameter]
        public bool DeleteRemovesItem { get; set; }

        [Parameter]
        public bool ActivateCreatedItem { get; set; } = true;

        [Parameter]
        public bool ActivateNextAfterDelete { get; set; } = true;

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public Func<string, XConversationItem> CreateItemFactory { get; set; }

        [Parameter]
        public string CreateText { get; set; } = "New conversation";

        [Parameter]
        public string RenameText { get; set; } = "Rename";

        [Parameter]
        public string DeleteText { get; set; } = "Delete";

        [Parameter]
        public string DeleteConfirmText { get; set; } = "Delete this conversation?";

        [Parameter]
        public string CreatePlaceholder { get; set; } = "Conversation title";

        [Parameter]
        public string DefaultCreateTitle { get; set; } = "New conversation";

        [Parameter]
        public string RenameConfirmText { get; set; } = "Save";

        [Parameter]
        public string CreateConfirmText { get; set; } = "Create";

        [Parameter]
        public string CancelText { get; set; } = "Cancel";

        [Parameter]
        public string PinnedText { get; set; } = "Pin";

        [Parameter]
        public string CreateIcon { get; set; } = "plus";

        [Parameter]
        public string RenameIcon { get; set; } = "edit";

        [Parameter]
        public string DeleteIcon { get; set; } = "delete";

        [Parameter]
        public string ConfirmIcon { get; set; } = "check";

        [Parameter]
        public string CancelIcon { get; set; } = "close";

        [Parameter]
        public string CreateAriaLabel { get; set; } = "Create conversation";

        [Parameter]
        public string CreateInputAriaLabel { get; set; } = "New conversation title";

        [Parameter]
        public string CreateConfirmAriaLabel { get; set; } = "Confirm create conversation";

        [Parameter]
        public string CreateCancelAriaLabel { get; set; } = "Cancel create conversation";

        protected string ConversationsClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-conversations", Cls)
            .AddIf(ShowActions && HasItemActions, "has-actions")
            .AddIf(creating, "is-creating")
            .AddIf(Disabled, "is-disabled")
            .ToString();

        protected IEnumerable<IGrouping<string, XConversationItem>> GroupedItems => (Items ?? Enumerable.Empty<XConversationItem>())
            .GroupBy(x => x.Group ?? string.Empty);

        protected bool HasItemActions => ActionTemplate != null
            || (ShowRename && (RenameInline || Editable || OnRename.HasDelegate || OnRenameConversation.HasDelegate))
            || (ShowDelete && (DeleteRemovesItem || OnDelete.HasDelegate || OnDeleteConversation.HasDelegate));

        protected bool IsActive(XConversationItem item) => string.Equals(item?.Id, ActiveId, StringComparison.Ordinal);

        protected string GetRowClass(XConversationItem item, bool active) => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-conversations__row")
            .AddIf(active, "is-active")
            .AddIf(item?.Disabled == true, "is-disabled")
            .AddIf(IsEditing(item), "is-editing")
            .ToString();

        protected string GetItemClass(XConversationItem item, bool active) => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-conversations__item")
            .AddIf(active, "is-active")
            .AddIf(item?.Disabled == true, "is-disabled")
            .AddIf(item?.Pinned == true, "is-pinned")
            .ToString();

        private async Task SelectAsync(XConversationItem item)
        {
            if (item == null || item.Disabled || Disabled || IsEditing(item))
            {
                return;
            }

            await SetActiveIdAsync(item.Id);
            if (OnSelect.HasDelegate)
            {
                await OnSelect.InvokeAsync(item);
            }
        }

        private async Task CreateAsync()
        {
            if (Disabled)
            {
                return;
            }

            if (CreateInline)
            {
                creating = true;
                createTitle = DefaultCreateTitle;
                editingId = null;
                editingTitle = null;
                return;
            }

            if (OnCreate.HasDelegate)
            {
                await OnCreate.InvokeAsync(null);
            }

            if (OnCreateConversation.HasDelegate)
            {
                var item = CreateConversationItem(DefaultCreateTitle);
                await OnCreateConversation.InvokeAsync(new XConversationCreateEventArgs { Item = item });
            }
        }

        private async Task RenameAsync(XConversationItem item)
        {
            if (item == null || item.Disabled || Disabled)
            {
                return;
            }

            if (RenameInline || Editable)
            {
                editingId = item.Id;
                editingTitle = item.Title;
                item.Editing = true;
                creating = false;
                return;
            }

            if (OnRename.HasDelegate)
            {
                await OnRename.InvokeAsync(item);
            }
        }

        private async Task DeleteAsync(XConversationItem item)
        {
            if (item == null || item.Disabled || Disabled)
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

            var deletedId = item.Id;
            var nextActiveId = DeleteRemovesItem
                ? ResolveNextActiveId(item)
                : ActiveId;

            if (DeleteRemovesItem)
            {
                var next = (Items ?? Enumerable.Empty<XConversationItem>())
                    .Where(x => !ReferenceEquals(x, item) && !string.Equals(x?.Id, item.Id, StringComparison.Ordinal))
                    .ToList();
                Items = next;
                await NotifyItemsChangedAsync();
                if (ActivateNextAfterDelete && string.Equals(ActiveId, deletedId, StringComparison.Ordinal))
                {
                    await SetActiveIdAsync(nextActiveId);
                }
            }

            if (OnDelete.HasDelegate)
            {
                await OnDelete.InvokeAsync(item);
            }

            if (OnDeleteConversation.HasDelegate)
            {
                await OnDeleteConversation.InvokeAsync(new XConversationDeleteEventArgs
                {
                    Item = item,
                    DeletedId = deletedId,
                    NextActiveId = nextActiveId
                });
            }
        }

        protected string GetRenameAriaLabel(XConversationItem item) => $"{RenameText} {item?.Title}".Trim();

        protected string GetDeleteAriaLabel(XConversationItem item) => $"{DeleteText} {item?.Title}".Trim();

        protected bool IsEditing(XConversationItem item)
        {
            return item != null
                && (item.Editing
                    || (!string.IsNullOrWhiteSpace(editingId)
                        && string.Equals(item.Id, editingId, StringComparison.Ordinal)));
        }

        protected string GetRenameInputAriaLabel(XConversationItem item) => $"{RenameText} {item?.Title}".Trim();

        protected string GetRenameConfirmAriaLabel(XConversationItem item) => $"{RenameConfirmText} {item?.Title}".Trim();

        protected string GetRenameCancelAriaLabel(XConversationItem item) => $"{CancelText} {item?.Title}".Trim();

        private Task OnCreateInputAsync(ChangeEventArgs args)
        {
            createTitle = Convert.ToString(args.Value);
            return Task.CompletedTask;
        }

        private async Task OnCreateKeyDownAsync(KeyboardEventArgs args)
        {
            if (args?.Key == "Enter")
            {
                await ConfirmCreateAsync();
            }
            else if (args?.Key == "Escape")
            {
                await CancelCreateAsync();
            }
        }

        private Task OnRenameInputAsync(ChangeEventArgs args)
        {
            editingTitle = Convert.ToString(args.Value);
            return Task.CompletedTask;
        }

        private async Task OnRenameKeyDownAsync(KeyboardEventArgs args, XConversationItem item)
        {
            if (args?.Key == "Enter")
            {
                await ConfirmRenameAsync(item);
            }
            else if (args?.Key == "Escape")
            {
                await CancelRenameAsync();
            }
        }

        private async Task ConfirmCreateAsync()
        {
            if (Disabled || !creating)
            {
                return;
            }

            var title = NormalizeTitle(createTitle, DefaultCreateTitle);
            var item = CreateConversationItem(title);
            var next = (Items ?? Enumerable.Empty<XConversationItem>()).ToList();
            next.Insert(0, item);
            Items = next;
            creating = false;
            createTitle = null;
            await NotifyItemsChangedAsync();

            if (OnCreate.HasDelegate)
            {
                await OnCreate.InvokeAsync(null);
            }
            if (OnCreateConversation.HasDelegate)
            {
                await OnCreateConversation.InvokeAsync(new XConversationCreateEventArgs { Item = item });
            }
            if (ActivateCreatedItem)
            {
                await SetActiveIdAsync(item.Id);
                if (OnSelect.HasDelegate)
                {
                    await OnSelect.InvokeAsync(item);
                }
            }
        }

        private Task CancelCreateAsync()
        {
            creating = false;
            createTitle = null;
            return Task.CompletedTask;
        }

        private async Task ConfirmRenameAsync(XConversationItem item)
        {
            if (item == null || item.Disabled || Disabled)
            {
                return;
            }

            var oldTitle = item.Title;
            var nextTitle = NormalizeTitle(editingTitle, oldTitle);
            item.Title = nextTitle;
            item.Editing = false;
            editingId = null;
            editingTitle = null;
            await NotifyItemsChangedAsync();

            if (OnRename.HasDelegate)
            {
                await OnRename.InvokeAsync(item);
            }
            if (OnRenameConversation.HasDelegate)
            {
                await OnRenameConversation.InvokeAsync(new XConversationRenameEventArgs
                {
                    Item = item,
                    OldTitle = oldTitle,
                    Title = nextTitle
                });
            }
        }

        private async Task CancelRenameAsync()
        {
            var item = (Items ?? Enumerable.Empty<XConversationItem>())
                .FirstOrDefault(x => string.Equals(x?.Id, editingId, StringComparison.Ordinal));
            if (item != null)
            {
                item.Editing = false;
            }

            editingId = null;
            editingTitle = null;
            await Task.CompletedTask;
        }

        private async Task SetActiveIdAsync(string id)
        {
            ActiveId = id;
            if (ActiveIdChanged.HasDelegate)
            {
                await ActiveIdChanged.InvokeAsync(id);
            }
        }

        private async Task NotifyItemsChangedAsync()
        {
            if (ItemsChanged.HasDelegate)
            {
                await ItemsChanged.InvokeAsync(Items);
            }
        }

        private XConversationItem CreateConversationItem(string title)
        {
            var item = CreateItemFactory?.Invoke(title) ?? new XConversationItem
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = title
            };

            item.Title = NormalizeTitle(item.Title, title);
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Guid.NewGuid().ToString("N");
            }

            return item;
        }

        private string ResolveNextActiveId(XConversationItem deleted)
        {
            if (deleted == null || !string.Equals(ActiveId, deleted.Id, StringComparison.Ordinal))
            {
                return ActiveId;
            }

            var items = (Items ?? Enumerable.Empty<XConversationItem>()).ToList();
            var index = items.FindIndex(x => ReferenceEquals(x, deleted) || string.Equals(x?.Id, deleted.Id, StringComparison.Ordinal));
            if (index < 0)
            {
                return items.FirstOrDefault(x => !x.Disabled)?.Id;
            }

            return items.Skip(index + 1).Concat(items.Take(index)).FirstOrDefault(x => !x.Disabled)?.Id;
        }

        private static string NormalizeTitle(string title, string fallback)
        {
            return string.IsNullOrWhiteSpace(title)
                ? fallback
                : title.Trim();
        }
    }
}
