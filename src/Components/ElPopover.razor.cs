using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElPopover : ElementComponentBase, IDisposable
    {
        private static int nextId;
        private bool internalVisible;
        private CancellationTokenSource delayCancellationTokenSource;

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment TitleContent { get; set; }

        [Parameter]
        public string Content { get; set; }

        [Parameter]
        public RenderFragment ContentTemplate { get; set; }

        [Parameter]
        public RenderFragment Footer { get; set; }

        [Parameter]
        public RenderFragment Reference { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Placement { get; set; } = "bottom";

        [Parameter]
        public PopperTrigger Trigger { get; set; } = PopperTrigger.Click;

        [Parameter]
        public bool ModelValue { get; set; }

        [Parameter]
        public EventCallback<bool> ModelValueChanged { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public int Width { get; set; } = 150;

        [Parameter]
        public bool ShowArrow { get; set; } = true;

        [Parameter]
        public int OpenDelay { get; set; }

        [Parameter]
        public int HideAfter { get; set; } = 200;

        [Parameter]
        public string PopperClass { get; set; }

        [Parameter]
        public string PopperStyle { get; set; }

        [Parameter]
        public string Role { get; set; } = "tooltip";

        [Parameter]
        public int Tabindex { get; set; }

        [Parameter]
        public EventCallback OnShow { get; set; }

        [Parameter]
        public EventCallback OnHide { get; set; }

        [Parameter]
        public EventCallback<bool> OnVisibleChange { get; set; }

        protected string PopoverId { get; } = $"el-popover-{Interlocked.Increment(ref nextId)}";

        protected bool PreventContextMenu => Trigger == PopperTrigger.ContextMenu;

        protected bool ShouldShowPopover => !Disabled && (Trigger == PopperTrigger.Manual ? ModelValue : internalVisible);

        protected string ReferenceStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add("position:relative", "display:inline-block")
            .Add(Style)
            .ToString();

        protected string PopoverClassValue => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-popover", "el-popper")
            .Add(PopperClass)
            .Add(Cls)
            .ToString();

        protected string PopoverStyleValue => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add("position:absolute")
            .Add("z-index:var(--el-index-popper, 2000)")
            .AddIf(Width > 0, $"width:{Width}px")
            .Add(GetPlacementStyle())
            .Add(PopperStyle)
            .ToString();

        public Task ShowAsync()
        {
            return SetVisibleAsync(true);
        }

        public Task HideAsync()
        {
            return SetVisibleAsync(false);
        }

        protected Task OnMouseOverAsync(MouseEventArgs e)
        {
            return Trigger == PopperTrigger.Hover ? SetVisibleAsync(true, OpenDelay) : Task.CompletedTask;
        }

        protected Task OnMouseOutAsync(MouseEventArgs e)
        {
            return Trigger == PopperTrigger.Hover ? SetVisibleAsync(false, HideAfter) : Task.CompletedTask;
        }

        protected Task OnClickAsync(MouseEventArgs e)
        {
            return Trigger == PopperTrigger.Click ? SetVisibleAsync(!internalVisible) : Task.CompletedTask;
        }

        protected Task OnFocusInAsync(FocusEventArgs e)
        {
            return Trigger == PopperTrigger.Focus ? SetVisibleAsync(true, OpenDelay) : Task.CompletedTask;
        }

        protected Task OnFocusOutAsync(FocusEventArgs e)
        {
            return Trigger == PopperTrigger.Focus ? SetVisibleAsync(false, HideAfter) : Task.CompletedTask;
        }

        protected Task OnContextMenuAsync(MouseEventArgs e)
        {
            return Trigger == PopperTrigger.ContextMenu ? SetVisibleAsync(!internalVisible) : Task.CompletedTask;
        }

        protected async Task SetVisibleAsync(bool visible, int delay = 0)
        {
            if (Disabled)
            {
                visible = false;
            }

            delayCancellationTokenSource?.Cancel();
            delayCancellationTokenSource?.Dispose();
            delayCancellationTokenSource = new CancellationTokenSource();
            var token = delayCancellationTokenSource.Token;

            if (delay > 0)
            {
                try
                {
                    await Task.Delay(delay, token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }

            if (Trigger == PopperTrigger.Manual)
            {
                if (ModelValueChanged.HasDelegate)
                {
                    await ModelValueChanged.InvokeAsync(visible);
                }
            }
            else
            {
                if (internalVisible == visible)
                {
                    return;
                }

                internalVisible = visible;
            }

            if (visible && OnShow.HasDelegate)
            {
                await OnShow.InvokeAsync(null);
            }
            if (!visible && OnHide.HasDelegate)
            {
                await OnHide.InvokeAsync(null);
            }
            if (OnVisibleChange.HasDelegate)
            {
                await OnVisibleChange.InvokeAsync(visible);
            }
        }

        private string GetPlacementStyle()
        {
            var placement = string.IsNullOrWhiteSpace(Placement) ? "bottom" : Placement.ToLowerInvariant();
            if (placement.StartsWith("top", StringComparison.Ordinal))
            {
                return "bottom:100%;left:50%;transform:translate(-50%, -8px)";
            }
            if (placement.StartsWith("left", StringComparison.Ordinal))
            {
                return "right:100%;top:50%;transform:translate(-8px, -50%)";
            }
            if (placement.StartsWith("right", StringComparison.Ordinal))
            {
                return "left:100%;top:50%;transform:translate(8px, -50%)";
            }

            return "top:100%;left:50%;transform:translate(-50%, 8px)";
        }

        public override void Dispose()
        {
            delayCancellationTokenSource?.Cancel();
            delayCancellationTokenSource?.Dispose();
            base.Dispose();
        }
    }
}
