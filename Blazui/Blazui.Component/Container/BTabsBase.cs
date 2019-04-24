using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazui.Component.Container
{
    public class BTabsBase : ComponentBase
    {
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

        public void AddTab(ITab tab)
        {
            if (ActiveTab == null)
            {
                SetActivateTab(tab);
            }
        }

        public void RemoveTab(ITab tab)
        {
            if (ActiveTab == tab)
            {
                SetActivateTab(null);
            }
        }

        public void SetActivateTab(ITab tab)
        {
            if (ActiveTab != tab)
            {
                ActiveTab = tab;
                StateHasChanged();
            }
        }
    }
}
