using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElDialog : BComponentBase
    {
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
        public EventCallback OnClose { get; set; }

        private bool wasOpen;

        protected override async Task OnParametersSetAsync()
        {
            if (ModelValue && !wasOpen)
            {
                wasOpen = true;
                if (OnOpen.HasDelegate)
                {
                    await OnOpen.InvokeAsync();
                }
            }
            else if (!ModelValue)
            {
                wasOpen = false;
            }
        }

        protected async Task CloseAsync(MouseEventArgs e = null)
        {
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
