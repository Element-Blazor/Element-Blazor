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
        /// <summary>
        /// 渲染后的内容区域
        /// </summary>
        public ElementRef Content { get; set; }
        [Parameter]
        public TabType Type { get; set; }

        [Parameter]
        public bool Editable { get; set; }

        internal (string headerPosition, string tabPosition) GetPosition()
        {
            var headerPosition = string.Empty;
            var tabPosition = string.Empty;
            switch (TabPosition)
            {
                case TabPosition.Top:
                    tabPosition = "el-tabs--top";
                    headerPosition = "is-top";
                    break;
                case TabPosition.Bottom:
                    tabPosition = "el-tabs--bottom";
                    headerPosition = "is-bottom";
                    break;
                case TabPosition.Left:
                    tabPosition = "el-tabs--left";
                    headerPosition = "is-left";
                    break;
                case TabPosition.Right:
                    tabPosition = "el-tabs--right";
                    headerPosition = "is-right";
                    break;
            }
            return (headerPosition, tabPosition);
        }

        [Parameter]
        public TabPosition TabPosition { get; set; }
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

        protected override void OnParametersSet()
        {
            if (Type == TabType.Normal && Editable)
            {
                throw new NotSupportedException("TabType为Card的情况下才能进行编辑");
            }
            base.OnParametersSet();
        }
    }
}
