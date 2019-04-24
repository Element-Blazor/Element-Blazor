using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazui.Component.Container
{
    public class BTabsBase : ComponentBase
    {

        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        private int barOffsetLeft;

        public int BarOffsetLeft
        {
            get
            {
                return barOffsetLeft;
            }
            set
            {
                barOffsetLeft = value;
                this.StateHasChanged();
            }
        }
        private int barWidth;
        public int BarWidth
        {
            get
            {
                return barWidth;
            }
            set
            {
                barWidth = value;
                this.StateHasChanged();
            }
        }
        [Parameter]
        protected RenderFragment ChildContent { get; set; }
        public ITab ActiveTab { get; protected set; }

        public async Task AddTabAsync(ITab tab)
        {
            if (ActiveTab == null)
            {
                await SetActivateTabAsync(tab);
            }
        }

        protected override async Task OnAfterRenderAsync()
        {
            var dom = ActiveTab.Element.Dom(JSRuntime);
            var width = await dom.GetClientWidthAsync();
            var paddingLeft = await dom.Style.GetPaddingLeftAsync();
            var offsetLeft = await dom.GetOffsetLeftAsync();
            var padding = paddingLeft + (await dom.Style.GetPaddingRightAsync());
            var barWidth = width - padding;
            var barOffsetLeft = offsetLeft + paddingLeft;
            if (BarWidth == barWidth && barOffsetLeft == BarOffsetLeft)
            {
                return;
            }
            BarWidth = barWidth;
            BarOffsetLeft = barOffsetLeft;
        }

        public async Task RemoveTabAsync(ITab tab)
        {
            if (ActiveTab == tab)
            {
                await SetActivateTabAsync(null);
            }
        }

        public async Task SetActivateTabAsync(ITab tab)
        {
            if (ActiveTab != tab)
            {
                ActiveTab = tab;
                StateHasChanged();
            }
        }
    }
}
