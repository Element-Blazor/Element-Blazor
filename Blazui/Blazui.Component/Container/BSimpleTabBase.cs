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
    public class BSimpleTabBase : ComponentBase
    {
        [Parameter]
        public bool? IsClosable { get; set; }
        [Parameter]
        public bool? IsAddable { get; set; }
        /// <summary>
        /// 渲染后的内容区域
        /// </summary>
        public ElementRef Content { get; set; }
        [Parameter]
        public TabType Type { get; set; }

        [Parameter]
        public bool IsEditable { get; set; }

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
        public ObservableCollection<ITab> RemovedTabPanels { get; set; } = new ObservableCollection<ITab>();
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

        internal async Task AddTabAsync(ITab tab)
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

        [Parameter]
        public EventCallback<UIMouseEventArgs> OnAddingTab { get; set; }

        protected override async Task OnAfterRenderAsync()
        {
            if (ActiveTab == null)
            {
                return;
            }
            ActiveTab.OnRenderCompletedAsync += AcitveTabOnRenderCompletedAsync;
        }

        private async Task AcitveTabOnRenderCompletedAsync(ITab arg)
        {
            arg.OnRenderCompletedAsync -= AcitveTabOnRenderCompletedAsync;
            var dom = arg.Element.Dom(JSRuntime);
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

        public void Refresh()
        {
            StateHasChanged();
        }

        public async Task SetActivateTabAsync(string name)
        {
            var tab = TabPanels.FirstOrDefault(x => x.Name == name);
            await SetActivateTabAsync(tab);
        }
        public async Task SetActivateTabAsync(ITab tab)
        {
            if (ActiveTab != null && ActiveTab.Name == tab.Name)
            {
                return;
            }
            var eventArgs = new ChangeEventArgs<ITab>();
            eventArgs.OldValue = ActiveTab;
            eventArgs.NewValue = tab;
            if (ActiveTab != null)
            {
                ActiveTab.IsActive = false;
            }
            ActiveTab = tab;
            if (ActiveTab != null)
            {
                ActiveTab.IsActive = true;
            }
            StateHasChanged();
            if (OnActiveTabChanged.HasDelegate)
            {
                await OnActiveTabChanged.InvokeAsync(eventArgs);
            }
        }

        public event Func<Task> OnRenderCompleteAsync;

        protected override void OnParametersSet()
        {
            if (Type == TabType.Normal && IsEditable)
            {
                throw new NotSupportedException("TabType为Card的情况下才能进行编辑");
            }
            base.OnParametersSet();
        }
    }
}
