using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component.Dom;
using Blazui.Component.EventArgs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Blazui.Component.Container
{
    public class BSimpleTabBase : ComponentBase, IDisposable
    {
        [Parameter]
        public bool? IsClosable { get; set; }
        [Parameter]
        public bool? IsAddable { get; set; }
        /// <summary>
        /// 渲染后的内容区域
        /// </summary>
        public ElementReference Content { get; set; }
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
        public ObservableCollection<ITab> TabPanels { get; private set; } = new ObservableCollection<ITab>();

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
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback<BChangeEventArgs<ITab>> OnActiveTabChanged { get; set; }

        [Parameter]
        public Func<ITab, Task<bool>> OnActiveTabChangingAsync { get; set; }

        internal string activeTabName;
        [Parameter]
        public string ActiveTabName
        {
            get
            {
                return activeTabName;
            }
            set
            {
                activeTabName = value;
            }
        }
        public ITab ActiveTab { get; internal set; }

        internal async Task AddTabAsync(ITab tab)
        {
            if (TabPanels.Any(x => x.Name == tab.Name))
            {
                return;
            }
            TabPanels.Add(tab);
            if (ActiveTab == null)
            {
                await SetActivateTabAsync(tab);
            }
        }

        [Parameter]
        public EventCallback<MouseEventArgs> OnAddingTab { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!TabPanels.Any())
            {
                return;
            }
            var oldActiveTab = ActiveTab;
            if (!string.IsNullOrWhiteSpace(ActiveTabName))
            {
                ActiveTab = TabPanels.FirstOrDefault(x => x.Name == ActiveTabName);
            }
            if (ActiveTab == null)
            {
                return;
            }
            ActiveTab.OnRenderCompletedAsync += AcitveTabOnRenderCompletedAsync;
            if (oldActiveTab == null || oldActiveTab.Name != ActiveTab.Name)
            {
                StateHasChanged();
            }
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
                if (OnTabRenderComplete.HasDelegate)
                {
                    await OnTabRenderComplete.InvokeAsync(arg);
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
            if (OnActiveTabChangingAsync != null)
            {
                var allowSwitching = await OnActiveTabChangingAsync(tab);
                if (!allowSwitching)
                {
                    return;
                }
            }
            if (ActiveTab != null && ActiveTab.Name == tab.Name)
            {
                return;
            }
            ActiveTabName = tab.Name;
            var eventArgs = new BChangeEventArgs<ITab>();
            eventArgs.OldValue = ActiveTab;
            eventArgs.NewValue = tab;
            ActiveTab = tab;
            StateHasChanged();
            if (OnActiveTabChanged.HasDelegate)
            {
                await OnActiveTabChanged.InvokeAsync(eventArgs);
            }
        }

        [Parameter]
        public EventCallback<ITab> OnTabRenderComplete { get; set; }

        protected override void OnParametersSet()
        {
            if (Type == TabType.Normal && IsEditable)
            {
                throw new NotSupportedException("TabType为Card的情况下才能进行编辑");
            }
            base.OnParametersSet();
        }

        public void Dispose()
        {
        }
    }
}
