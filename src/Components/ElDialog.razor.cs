using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElDialog : BComponentBase
    {
        private static int nextZIndex = 2000;
        private ElementReference wrapperElement;
        private bool hasOpenedRender;

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
        public string Width { get; set; } = "50%";

        [Parameter]
        public bool Fullscreen { get; set; }

        [Parameter]
        public string Top { get; set; } = "15vh";

        [Parameter]
        public bool Modal { get; set; } = true;

        [Parameter]
        public bool CloseOnClickModal { get; set; } = true;

        [Parameter]
        public bool CloseOnPressEscape { get; set; } = true;

        [Parameter]
        public bool ShowClose { get; set; } = true;

        [Parameter]
        public bool Draggable { get; set; }

        [Parameter]
        public bool DestroyOnClose { get; set; }

        [Parameter]
        public int? ZIndex { get; set; }

        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment Footer { get; set; }

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

        private bool wasOpen;
        protected int ResolvedZIndex { get; private set; }
        protected int ResolvedModalZIndex => Modal ? ResolvedZIndex - 1 : ResolvedZIndex;

        protected bool ShouldRenderDialog => ModelValue || !DestroyOnClose;

        protected override async Task OnParametersSetAsync()
        {
            if (ModelValue && !wasOpen)
            {
                wasOpen = true;
                hasOpenedRender = false;
                ResolvedZIndex = ZIndex ?? Interlocked.Add(ref nextZIndex, 2);
                if (OnOpen.HasDelegate)
                {
                    await OnOpen.InvokeAsync();
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
            if (!ModelValue || hasOpenedRender)
            {
                return;
            }
            hasOpenedRender = true;
            await wrapperElement.Dom(JSRuntime).FocusAsync();
            if (OnOpened.HasDelegate)
            {
                await OnOpened.InvokeAsync();
            }
        }

        protected async Task CloseAsync(MouseEventArgs e = null)
        {
            if (!ModelValue)
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
                await OnClose.InvokeAsync();
            }
            if (OnVisibleChange.HasDelegate)
            {
                await OnVisibleChange.InvokeAsync(false);
            }
            if (OnClosed.HasDelegate)
            {
                await OnClosed.InvokeAsync();
            }
        }

        protected async Task CloseOnModalClickAsync(MouseEventArgs e)
        {
            if (!CloseOnClickModal)
            {
                return;
            }
            await CloseAsync(e);
        }

        protected async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (!CloseOnPressEscape || e.Key != "Escape")
            {
                return;
            }
            await CloseAsync();
        }
    }
}
