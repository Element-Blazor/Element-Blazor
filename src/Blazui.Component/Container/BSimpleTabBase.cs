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
    public class BSimpleTabBase : BComponentBase, IDisposable
    {
        [Parameter]
        public ObservableCollection<TabOption> DataSource { get; set; }
        private bool requireRerender = true;
        [Parameter]
        public bool IsAddable { get; set; }
        /// <summary>
        /// 渲染后的内容区域
        /// </summary>
        public ElementReference Content { get; set; }
        [Parameter]
        public TabType Type { get; set; }

        [Parameter]
        public bool IsEditable { get; set; }

        [Parameter]
        public TabPosition TabPosition { get; set; }
        private List<BSimpleTabPanelBase> tabPanels { get; set; } = new List<BSimpleTabPanelBase>();

        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Tab 页被切换后触发
        /// </summary>
        [Parameter]
        public EventCallback<BChangeEventArgs<BSimpleTabPanelBase>> OnActiveTabChanged { get; set; }

        /// <summary>
        /// Tab 页被切换前触发
        /// </summary>
        [Parameter]
        public EventCallback<BChangeEventArgs<BSimpleTabPanelBase>> OnActiveTabChanging { get; set; }

        /// <summary>
        /// Tab 页被关闭后触发
        /// </summary>
        [Parameter]
        public EventCallback<BSimpleTabPanelBase> OnTabClose { get; set; }

        /// <summary>
        /// Tab 页被关闭时触发
        /// </summary>
        [Parameter]
        public EventCallback<BClosingEventArgs<BSimpleTabPanelBase>> OnTabClosing { get; set; }
        internal async Task CloseTabAsync(BSimpleTabPanelBase tab)
        {
            if (OnTabClosing.HasDelegate)
            {
                var arg = new BClosingEventArgs<BSimpleTabPanelBase>();
                arg.Target = tab;
                await OnTabClosing.InvokeAsync(arg);
                if (arg.Cancel)
                {
                    return;
                }
            }

            requireRerender = true;
            ResetActiveTab(tab);
            if (OnTabClose.HasDelegate)
            {
                _ = OnTabClose.InvokeAsync(tab);
            }
        }

        private void ResetActiveTab(BSimpleTabPanelBase tab)
        {
            if (DataSource == null)
            {
                return;
            }
            if (DataSource.Count <= 1)
            {
                return;
            }
            var activeOption = DataSource.FirstOrDefault(x => x.IsActive);
            if (activeOption.Title != tab.Title)
            {
                return;
            }
            var activeIndex = DataSource.IndexOf(activeOption);
            var newActiveIndex = activeIndex;
            if (activeIndex == DataSource.Count - 1)
            {
                newActiveIndex = DataSource.Count - 2;
            }
            DataSource.RemoveAt(activeIndex);
            activeOption = DataSource.ElementAt(newActiveIndex);
            activeOption.IsActive = true;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (IsEditable || IsAddable)
            {
                if (DataSource == null)
                {
                    throw new BlazuiException("标签页组件启用可编辑模式时必须指定 DataSource 属性，硬编码无效");
                }
                var activeTab = DataSource.FirstOrDefault(x => x.IsActive);
                if (activeTab == null)
                {
                    activeTab = DataSource.FirstOrDefault();
                    activeTab.IsActive = true;
                }
                DataSource.CollectionChanged -= DataSource_CollectionChanged;
                DataSource.CollectionChanged += DataSource_CollectionChanged;
            }
        }

        private void DataSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DataSource.GroupBy(x => x.Name).Where(x => x.Count() > 1);
        }

        /// <summary>
        /// 点击加号按钮增加 Tab 页时触发
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnAddingTab { get; set; }

        internal BSimpleTabPanelBase ActiveTab { get; private set; }

        internal int BarOffsetLeft { get; set; }
        internal int BarWidth { get; set; }
        internal void AddTab(BSimpleTabPanelBase tab)
        {
            if (Exists(tab.Name))
            {
                return;
            }
            tabPanels.Add(tab);
        }

        internal void RemoveTab(string name)
        {
            tabPanels.Remove(tabPanels.FirstOrDefault(x => x.Name == name));
        }

        internal bool Exists(string name)
        {
            if (tabPanels.Any(x => x.Name == name))
            {
                return true;
            }
            return false;
        }

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
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (requireRerender)
            {
                requireRerender = false;
                if (DataSource == null)
                {
                    var activeTab = tabPanels.FirstOrDefault(x => x.IsActive);
                    if (activeTab == null)
                    {
                        activeTab = tabPanels.FirstOrDefault();
                        activeTab.Activate();
                    }
                    await SetActivateTabAsync(activeTab);
                    return;
                }
            }
        }


        public void Refresh()
        {
            StateHasChanged();
        }

        internal async Task UpdateHeaderSizeAsync(BSimpleTabPanelBase tabPanel, int barWidth, int barOffsetLeft)
        {
            if (BarWidth == barWidth && barOffsetLeft == BarOffsetLeft)
            {
                await TabRenderCompletedAsync(tabPanel);
                return;
            }
            BarWidth = barWidth;
            BarOffsetLeft = barOffsetLeft;
            StateHasChanged();
        }
        internal async Task TabRenderCompletedAsync(BSimpleTabPanelBase tabPanel)
        {
            if (OnTabRenderComplete.HasDelegate)
            {
                await OnTabRenderComplete.InvokeAsync(tabPanel);
            }
        }

        public async Task SetActivateTabAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ActiveTab = null;
                return;
            }
            var tab = tabPanels.FirstOrDefault(x => x.Name == name);
            if (tab == null)
            {
                throw new BlazuiException($"Name 为 {name} 的 Tab 不存在");
            }
            await SetActivateTabAsync(tab);
        }
        internal async Task<bool> SetActivateTabAsync(BSimpleTabPanelBase tab)
        {
            if (OnActiveTabChanging.HasDelegate)
            {
                var arg = new BChangeEventArgs<BSimpleTabPanelBase>();
                arg.NewValue = tab;
                arg.OldValue = ActiveTab;
                await OnActiveTabChanging.InvokeAsync(arg);
                if (arg.DisallowChange)
                {
                    return false;
                }
            }
            if (DataSource == null)
            {
                foreach (var tabPanel in tabPanels)
                {
                    if (tabPanel == tab)
                    {
                        tabPanel.Activate();
                        continue;
                    }
                    tabPanel.DeActivate();
                }
            }
            else
            {
                foreach (var item in DataSource)
                {
                    item.IsActive = item.Name == tab.Name;
                }
            }
            ActiveTab = tab;
            var eventArgs = new BChangeEventArgs<BSimpleTabPanelBase>();
            eventArgs.OldValue = ActiveTab;
            eventArgs.NewValue = tab;
            if (OnActiveTabChanged.HasDelegate)
            {
                await OnActiveTabChanged.InvokeAsync(eventArgs);
            }
            else
            {
                StateHasChanged();
            }
            return true;
        }

        [Parameter]
        public EventCallback<BSimpleTabPanelBase> OnTabRenderComplete { get; set; }

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
