using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Element.X
{
    public partial class ElXBubbleList : ElementComponentBase
    {
        private ElementReference listElement;
        private IReadOnlyList<XMessageItem> resolvedItems = new List<XMessageItem>();
        private int previousItemCount = -1;
        private string previousLastItemKey;
        private bool pendingScrollRequest;

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

        [Parameter]
        public bool AutoScrollOnUpdate { get; set; } = true;

        [Parameter]
        public bool SmoothScroll { get; set; } = true;

        [Parameter]
        public bool ScrollOnlyWhenNearBottom { get; set; } = true;

        [Parameter]
        public int BottomThreshold { get; set; } = 80;

        [Parameter]
        public bool AutoScrollOnStreaming { get; set; } = true;

        [Parameter]
        public bool LockAutoScrollWhenUserScrolls { get; set; }

        [Parameter]
        public bool UserPinned { get; set; }

        [Parameter]
        public EventCallback<bool> UserPinnedChanged { get; set; }

        [Parameter]
        public bool Reverse { get; set; }

        [Parameter]
        public string MaxHeight { get; set; }

        [Parameter]
        public string Role { get; set; } = "log";

        [Parameter]
        public string AriaLive { get; set; } = "polite";

        [Parameter]
        public string AriaRelevant { get; set; } = "additions text";

        [Parameter]
        public EventCallback OnAutoScrolled { get; set; }

        [Parameter]
        public EventCallback<bool> OnUserPinnedChanged { get; set; }

        protected IReadOnlyList<XMessageItem> ResolvedItems => resolvedItems;

        protected string AriaBusyValue => resolvedItems.Any(x => x?.Loading == true) ? "true" : "false";

        protected string ListClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-bubble-list", Cls)
            .AddIf(AutoScroll, "is-auto-scroll")
            .AddIf(Reverse, "is-reverse")
            .AddIf(UserPinned, "is-user-pinned")
            .ToString();

        protected string ListStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add(Style)
            .AddIf(!string.IsNullOrWhiteSpace(MaxHeight), $"max-height:{NormalizeCssSize(MaxHeight)}")
            .ToString();

        protected override void OnParametersSet()
        {
            resolvedItems = (Items ?? Enumerable.Empty<XMessageItem>()).ToList();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!ShouldAutoScroll(firstRender))
            {
                CaptureSnapshot();
                return;
            }

            try
            {
                await JSRuntime.InvokeVoidAsync(
                    "elementXBubbleListScrollToEnd",
                    listElement,
                    Reverse,
                    SmoothScroll,
                    ScrollOnlyWhenNearBottom || UserPinned,
                    BottomThreshold);

                if (OnAutoScrolled.HasDelegate)
                {
                    await OnAutoScrolled.InvokeAsync(null);
                }
            }
            catch
            {
            }

            CaptureSnapshot();
            pendingScrollRequest = false;
        }

        public async Task ScrollToEndAsync(bool smooth = true)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("elementXBubbleListScrollToEnd", listElement, Reverse, smooth, false, BottomThreshold);
            }
            catch
            {
            }
        }

        public async Task RequestScrollToEndAsync(bool smooth = true)
        {
            pendingScrollRequest = true;
            await ScrollToEndAsync(smooth);
            pendingScrollRequest = false;
            await SetUserPinnedAsync(false);
        }

        private bool ShouldAutoScroll(bool firstRender)
        {
            if (!AutoScroll || !AutoScrollOnUpdate || resolvedItems.Count == 0)
            {
                return false;
            }

            if (pendingScrollRequest)
            {
                return true;
            }

            if (firstRender || previousItemCount < 0)
            {
                return true;
            }

            var lastItemKey = GetLastItemKey();
            if (previousItemCount != resolvedItems.Count)
            {
                return true;
            }

            return AutoScrollOnStreaming && previousLastItemKey != lastItemKey;
        }

        private async Task HandleScrollAsync(EventArgs args)
        {
            if (!LockAutoScrollWhenUserScrolls && !UserPinned)
            {
                return;
            }

            try
            {
                var isPinned = await JSRuntime.InvokeAsync<bool>(
                    "elementXBubbleListIsUserPinned",
                    listElement,
                    Reverse,
                    BottomThreshold);
                await SetUserPinnedAsync(isPinned);
            }
            catch
            {
            }
        }

        private async Task SetUserPinnedAsync(bool value)
        {
            if (UserPinned == value)
            {
                return;
            }

            UserPinned = value;
            if (UserPinnedChanged.HasDelegate)
            {
                await UserPinnedChanged.InvokeAsync(value);
            }
            if (OnUserPinnedChanged.HasDelegate)
            {
                await OnUserPinnedChanged.InvokeAsync(value);
            }
        }

        private void CaptureSnapshot()
        {
            previousItemCount = resolvedItems.Count;
            previousLastItemKey = GetLastItemKey();
        }

        private string GetLastItemKey()
        {
            var item = resolvedItems.LastOrDefault();
            if (item == null)
            {
                return null;
            }

            return item.Id ?? $"{item.Role}|{item.Content}|{item.Loading}|{item.Typing}";
        }

        private static string NormalizeCssSize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var trimmed = value.Trim();
            return decimal.TryParse(trimmed, NumberStyles.Number, CultureInfo.InvariantCulture, out _)
                ? $"{trimmed}px"
                : trimmed;
        }
    }
}
