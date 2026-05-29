using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElDrawer : ElementComponentBase
    {
        private static int nextZIndex = 2000;
        private bool wasOpen;
        private bool openedRendered;

        [Parameter]
        public bool ModelValue { get; set; }

        [Parameter]
        public EventCallback<bool> ModelValueChanged { get; set; }

        [Parameter]
        public bool Visible
        {
            get => ModelValue;
            set => ModelValue = value;
        }

        [Parameter]
        public EventCallback<bool> VisibleChanged { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment Footer { get; set; }

        [Parameter]
        public DrawerDirection Direction { get; set; } = DrawerDirection.Rtl;

        [Parameter]
        public string Size { get; set; } = "30%";

        [Parameter]
        public bool WithHeader { get; set; } = true;

        [Parameter]
        public bool Modal { get; set; } = true;

        [Parameter]
        public bool AppendToBody { get; set; }

        [Parameter]
        public bool LockScroll { get; set; } = true;

        [Parameter]
        public bool CloseOnClickModal { get; set; } = true;

        [Parameter]
        public bool CloseOnPressEscape { get; set; } = true;

        [Parameter]
        public bool ShowClose { get; set; } = true;

        [Parameter]
        public bool DestroyOnClose { get; set; }

        [Parameter]
        public int? ZIndex { get; set; }

        [Parameter]
        public Func<Task<bool>> BeforeClose { get; set; }

        [Parameter]
        public EventCallback<ElementClosingEventArgs<ElDrawer>> OnBeforeClose { get; set; }

        [Parameter]
        public EventCallback OnOpen { get; set; }

        [Parameter]
        public EventCallback OnOpened { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        [Parameter]
        public EventCallback OnClosed { get; set; }

        [Parameter]
        public EventCallback<bool> OnVisibleChange { get; set; }

        protected int ResolvedZIndex { get; private set; }

        protected bool ShouldRenderDrawer => ModelValue || !DestroyOnClose;

        private bool IsHorizontal => Direction == DrawerDirection.Rtl || Direction == DrawerDirection.Ltr;

        private string DirectionClass => Direction.ToString().ToLowerInvariant();

        private string WrapperClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-drawer__wrapper")
            .AddIf(ModelValue, "el-drawer__open")
            .Add(Cls)
            .ToString();

        private string WrapperStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add($"z-index:{ResolvedZIndex}")
            .AddIf(!ModelValue, "display:none")
            .ToString();

        private string ModalStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add($"z-index:{ResolvedZIndex - 1}")
            .AddIf(!ModelValue, "display:none")
            .ToString();

        private string DrawerClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-drawer", DirectionClass)
            .ToString();

        private string DrawerStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(IsHorizontal, $"width:{Size}")
            .AddIf(!IsHorizontal, $"height:{Size}")
            .Add(Style)
            .ToString();

        protected override async Task OnParametersSetAsync()
        {
            if (ModelValue && !wasOpen)
            {
                wasOpen = true;
                openedRendered = false;
                ResolvedZIndex = ZIndex ?? Interlocked.Add(ref nextZIndex, 2) + ConfigZIndex - ElementConfig.DefaultZIndex;
                if (OnOpen.HasDelegate)
                {
                    await OnOpen.InvokeAsync(null);
                }
                if (OnVisibleChange.HasDelegate)
                {
                    await OnVisibleChange.InvokeAsync(true);
                }
            }
            else if (!ModelValue)
            {
                wasOpen = false;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!ModelValue || openedRendered)
            {
                return;
            }

            openedRendered = true;
            if (OnOpened.HasDelegate)
            {
                await OnOpened.InvokeAsync(null);
            }
        }

        private async Task CloseAsync()
        {
            if (!ModelValue || !await CanCloseAsync())
            {
                return;
            }

            ModelValue = false;
            wasOpen = false;
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(false);
            }
            if (VisibleChanged.HasDelegate)
            {
                await VisibleChanged.InvokeAsync(false);
            }
            if (OnClose.HasDelegate)
            {
                await OnClose.InvokeAsync(null);
            }
            if (OnVisibleChange.HasDelegate)
            {
                await OnVisibleChange.InvokeAsync(false);
            }
            if (OnClosed.HasDelegate)
            {
                await OnClosed.InvokeAsync(null);
            }
        }

        private async Task<bool> CanCloseAsync()
        {
            if (BeforeClose != null && !await BeforeClose())
            {
                return false;
            }

            if (!OnBeforeClose.HasDelegate)
            {
                return true;
            }

            var args = new ElementClosingEventArgs<ElDrawer> { Target = this };
            await OnBeforeClose.InvokeAsync(args);
            return !args.Cancel;
        }

        private async Task CloseOnModalClickAsync(MouseEventArgs e)
        {
            if (!CloseOnClickModal)
            {
                return;
            }

            await CloseAsync();
        }

        private async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (!CloseOnPressEscape || e.Key != "Escape")
            {
                return;
            }

            await CloseAsync();
        }
    }
}
