

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public partial class BTabPanel :  IDisposable
    {
        [Parameter]
        public EventCallback<BChangeEventArgs<BTabPanel>> OnTabPanelChanging { get; set; }
        private static int tabIndex = 0;

        public override string ToString()
        {
            return Name;
        }

        protected async Task OnInternalTabCloseAsync(MouseEventArgs e)
        {
            RequireRender = true;
            await TabContainer.CloseTabAsync(this);
        }

        [Parameter]
        public bool IsActive { get; set; }
        public ElementReference Element { get; set; }

        [CascadingParameter]
        public BTab TabContainer { get; set; }

        protected async Task Activate(MouseEventArgs e)
        {
            RequireRender = true;
            IsActive = await TabContainer.SetActivateTabAsync(this);
        }

        public void Activate()
        {
            IsActive = true;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            RequireRender = TabContainer?.ActiveTab != null;
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
            if (IsActive && TabContainer.Type == TabType.Normal)
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
            await AcitveTabOnRenderCompletedAsync();
            if (!firstRender)
            {
                return;
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            TabContainer?.RemoveTab(this.Name);
        }

        internal void DeActivate()
        {
            IsActive = false;
        }

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
