using Blazui.Component.Dom;
using Blazui.Component.EventArgs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Container
{
    public class BTabPanelBase : BComponentBase, IDisposable
    {
        [Parameter]
        public EventCallback<BChangeEventArgs<BTabPanelBase>> OnTabPanelChanging { get; set; }
        private static int tabIndex = 0;


        public override string ToString()
        {
            return Name;
        }

        protected async Task OnInternalTabCloseAsync(MouseEventArgs e)
        {
            await TabContainer.CloseTabAsync(this);
        }

        [Parameter]
        public bool IsActive { get; set; }
        public ElementReference Element { get; set; }

        [CascadingParameter]
        public BTab TabContainer { get; set; }

        protected async Task Activate(MouseEventArgs e)
        {
            IsActive = await TabContainer.SetActivateTabAsync(this);
        }

        public void Activate()
        {
            IsActive = true;
        }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsClosable { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                return;
            }
            Name = "tab_" + (++tabIndex);
            await Task.CompletedTask;
        }


        [Parameter]
        public Func<BTabPanelBase, Task> OnRenderCompleted { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (!TabContainer.Exists(Name))
            {
                TabContainer.AddTab(this);
            }
        }

        private async Task AcitveTabOnRenderCompletedAsync()
        {
            if (TabContainer.Type == TabType.Normal)
            {
                var dom = Element.Dom(JSRuntime);
                var width = await dom.GetClientWidthAsync();
                var paddingLeft = await dom.Style.GetPaddingLeftAsync();
                var offsetLeft = await dom.GetOffsetLeftAsync();
                var padding = paddingLeft + (await dom.Style.GetPaddingRightAsync());
                var barWidth = width - padding;
                var barOffsetLeft = offsetLeft + paddingLeft;
                await TabContainer.UpdateHeaderSizeAsync(this, barWidth, barOffsetLeft);
            }
            else
            {
                await TabContainer.TabRenderCompletedAsync(this);
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsActive)
            {
                await AcitveTabOnRenderCompletedAsync();
            }
            else if (OnRenderCompleted != null)
            {
                await OnRenderCompleted(this);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            TabContainer.RemoveTab(this.Name);
        }

        internal void DeActivate()
        {
            IsActive = false;
        }
    }
}
