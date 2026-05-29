using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElTooltip : ElementComponentBase, IDisposable
    {
        private static int nextId;
        private bool internalVisible;
        private CancellationTokenSource delayCancellationTokenSource;

        [Parameter]
        public string Content { get; set; }

        [Parameter]
        public RenderFragment ContentTemplate { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Placement { get; set; } = "bottom";

        [Parameter]
        public PopperTrigger Trigger { get; set; } = PopperTrigger.Hover;

        [Parameter]
        public bool ModelValue { get; set; }

        [Parameter]
        public EventCallback<bool> ModelValueChanged { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public string Effect { get; set; } = "dark";

        [Parameter]
        public bool ShowArrow { get; set; } = true;

        [Parameter]
        public int OpenDelay { get; set; }

        [Parameter]
        public int HideAfter { get; set; }

        [Parameter]
        public string PopperClass { get; set; }

        [Parameter]
        public string PopperStyle { get; set; }

        [Parameter]
        public EventCallback OnShow { get; set; }

        [Parameter]
        public EventCallback OnHide { get; set; }

        [Parameter]
        public EventCallback<bool> OnVisibleChange { get; set; }

        protected string PopperId { get; } = $"el-tooltip-{Interlocked.Increment(ref nextId)}";

        protected bool PreventContextMenu => Trigger == PopperTrigger.ContextMenu;

        protected bool ShouldShowPopper => !Disabled && (Trigger == PopperTrigger.Manual ? ModelValue : internalVisible);

        protected string TriggerStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add("position:relative", "display:inline-block")
            .Add(Style)
            .ToString();

        protected string PopperClassValue => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-tooltip__popper", "el-popper")
            .AddIf(!string.IsNullOrWhiteSpace(Effect), $"is-{Effect}")
            .Add(PopperClass)
            .ToString();

        protected string PopperStyleValue => BuildPopperStyle();

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

        private async Task SetVisibleAsync(bool visible, int delay = 0)
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

        private string BuildPopperStyle()
        {
            var style = HtmlPropertyBuilder.CreateCssStyleBuilder()
                .Add("position:absolute")
                .Add("z-index:var(--el-index-popper, 2000)")
                .Add("min-width:max-content")
                .Add(GetPlacementStyle())
                .Add(PopperStyle);
            return style.ToString();
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
