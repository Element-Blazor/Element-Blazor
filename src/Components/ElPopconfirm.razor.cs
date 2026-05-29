using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElPopconfirm : ElementComponentBase
    {
        private bool internalVisible;

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment TitleContent { get; set; }

        [Parameter]
        public RenderFragment Reference { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Placement { get; set; } = "top";

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
        public string Icon { get; set; } = "el-icon-question";

        [Parameter]
        public string IconColor { get; set; } = "#f90";

        [Parameter]
        public string ConfirmButtonText { get; set; } = "确定";

        [Parameter]
        public string CancelButtonText { get; set; } = "取消";

        [Parameter]
        public ButtonType ConfirmButtonType { get; set; } = ButtonType.Primary;

        [Parameter]
        public ButtonType CancelButtonType { get; set; } = ButtonType.Text;

        [Parameter]
        public bool ShowArrow { get; set; } = true;

        [Parameter]
        public string PopperClass { get; set; }

        [Parameter]
        public string PopperStyle { get; set; }

        [Parameter]
        public EventCallback OnConfirm { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback<bool> OnVisibleChange { get; set; }

        protected bool IsVisible => !Disabled && (Trigger == PopperTrigger.Manual ? ModelValue : internalVisible);

        protected string ReferenceStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add("position:relative", "display:inline-block")
            .Add(Style)
            .ToString();

        protected string PopoverClassValue => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-popover", "el-popper", "el-popconfirm")
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

        protected string IconClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-popconfirm__icon")
            .Add(Icon)
            .ToString();

        protected string IconStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(IconColor), $"color:{IconColor}")
            .ToString();

        protected Task ToggleAsync(MouseEventArgs e)
        {
            return Trigger == PopperTrigger.Click ? SetVisibleAsync(!IsVisible) : Task.CompletedTask;
        }

        protected Task OnMouseOverAsync(MouseEventArgs e)
        {
            return Trigger == PopperTrigger.Hover ? SetVisibleAsync(true) : Task.CompletedTask;
        }

        protected Task OnMouseOutAsync(MouseEventArgs e)
        {
            return Trigger == PopperTrigger.Hover ? SetVisibleAsync(false) : Task.CompletedTask;
        }

        protected async Task ConfirmAsync(MouseEventArgs e)
        {
            if (OnConfirm.HasDelegate)
            {
                await OnConfirm.InvokeAsync(null);
            }

            await SetVisibleAsync(false);
        }

        protected async Task CancelAsync(MouseEventArgs e)
        {
            if (OnCancel.HasDelegate)
            {
                await OnCancel.InvokeAsync(null);
            }

            await SetVisibleAsync(false);
        }

        public async Task SetVisibleAsync(bool visible)
        {
            if (Disabled)
            {
                visible = false;
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
                internalVisible = visible;
            }

            if (OnVisibleChange.HasDelegate)
            {
                await OnVisibleChange.InvokeAsync(visible);
            }
        }

        private string GetPlacementStyle()
        {
            var placement = string.IsNullOrWhiteSpace(Placement) ? "top" : Placement.ToLowerInvariant();
            if (placement.StartsWith("bottom", System.StringComparison.Ordinal))
            {
                return "top:100%;left:50%;transform:translate(-50%, 8px)";
            }
            if (placement.StartsWith("left", System.StringComparison.Ordinal))
            {
                return "right:100%;top:50%;transform:translate(-8px, -50%)";
            }
            if (placement.StartsWith("right", System.StringComparison.Ordinal))
            {
                return "left:100%;top:50%;transform:translate(8px, -50%)";
            }

            return "bottom:100%;left:50%;transform:translate(-50%, -8px)";
        }
    }
}
