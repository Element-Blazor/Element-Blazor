using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component.Dom;
using Blazui.Component.EventArgs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazui.Component.Container
{
    public class BTabBase : ComponentBase
    {
        protected string tabType;

        /// <summary>
        /// 渲染后的内容区域
        /// </summary>
        public ElementRef Content { get; set; }
        [Parameter]
        public TabType Type { get; set; }
        public ObservableCollection<ITab> TabPanels { get; set; } = new ObservableCollection<ITab>();
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

        [Parameter]
        public EventCallback<UIChangeEventArgs> OnActiveTabChanged { get; set; }
        public ITab ActiveTab { get; protected set; }

        public async Task AddTabAsync(ITab tab)
        {
            if (!TabPanels.Contains(tab))
            {
                TabPanels.Add(tab);
            }
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
                if (OnRenderCompleteAsync != null)
                {
                    await OnRenderCompleteAsync();
                }
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
            var eventArgs = new ChangeEventArgs<ITab>();
            eventArgs.OldValue = ActiveTab;
            eventArgs.NewValue = tab;
            if (ActiveTab != tab)
            {
                if (ActiveTab != null)
                {

                    ActiveTab.IsActive = false;
                }
                ActiveTab = tab;
                ActiveTab.IsActive = true;
                StateHasChanged();
                if (OnActiveTabChanged.HasDelegate)
                {
                    await OnActiveTabChanged.InvokeAsync(eventArgs);
                }
            }
        }

        public event Func<Task> OnRenderCompleteAsync;
    }
}
