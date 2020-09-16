using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;


using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Blazui.Component
{
    public partial class BTab
    {
        internal bool headerSizeUpdated = false;
        /// <summary>
        /// 数据源
        /// </summary>
        [Parameter]
        public ObservableCollection<TabOption> DataSource { get; set; }

        /// <summary>
        /// 是否显示增加图标
        /// </summary>
        [Parameter]
        public bool IsAddable { get; set; }

        /// <summary>
        /// 是否可关闭
        /// </summary>
        public bool IsRemovable { get; set; }
        /// <summary>
        /// 渲染后的内容区域
        /// </summary>
        public ElementReference Content { get; set; }

        /// <summary>
        /// Tab 类型
        /// </summary>
        [Parameter]
        public TabType Type { get; set; }

        [Parameter]
        public bool IsEditable { get; set; }

        [Parameter]
        public TabPosition TabPosition { get; set; }
        private List<BTabPanel> tabPanels { get; set; } = new List<BTabPanel>();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Tab 页被切换后触发
        /// </summary>
        [Parameter]
        public EventCallback<BChangeEventArgs<BTabPanel>> OnActiveTabChanged { get; set; }

        /// <summary>
        /// Tab 页被切换前触发
        /// </summary>
        [Parameter]
        public EventCallback<BChangeEventArgs<BTabPanel>> OnActiveTabChanging { get; set; }

        /// <summary>
        /// Tab 页被关闭后触发
        /// </summary>
        [Parameter]
        public EventCallback<BTabPanel> OnTabClose { get; set; }

        /// <summary>
        /// Tab 页被关闭时触发
        /// </summary>
        [Parameter]
        public EventCallback<BClosingEventArgs<BTabPanel>> OnTabClosing { get; set; }
        internal async Task CloseTabAsync(BTabPanel tab)
        {
            if (OnTabClosing.HasDelegate)
            {
                var arg = new BClosingEventArgs<BTabPanel>();
                arg.Target = tab;
                await OnTabClosing.InvokeAsync(arg);
                if (arg.Cancel)
                {
                    return;
                }
            }

            ResetActiveTab(tab);
            RequireRender = true;
            if (OnTabClose.HasDelegate)
            {
                _ = OnTabClose.InvokeAsync(tab);
            }
            else
            {
                StateHasChanged();
            }
        }

        private TabOption ResetActiveTab(BTabPanel tab)
        {
            if (DataSource == null)
            {
                return null;
            }
            if (DataSource.Count <= 0)
            {
                return null;
            }
            var activeOption = DataSource.FirstOrDefault(x => x.IsActive);
            var activeIndex = DataSource.IndexOf(activeOption);
            if (activeOption.Title != tab.Title)
            {
                var removingIndex = DataSource.IndexOf(DataSource.FirstOrDefault(x => x.Name == tab.Name));
                DataSource.RemoveAt(removingIndex);
                if (activeIndex > removingIndex)
                {
                    activeIndex--;
                }
                return activeOption;
            }
            DataSource.RemoveAt(activeIndex);
            var newActiveIndex = activeIndex;
            if (newActiveIndex >= DataSource.Count - 1)
            {
                newActiveIndex = DataSource.Count - 1;
            }
            if (newActiveIndex == -1)
            {
                return null;
            }
            activeOption = DataSource.ElementAt(newActiveIndex);
            activeOption.IsActive = true;
            return activeOption;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (DataSource == null)
            {
                if (IsAddable)
                {
                    throw new BlazuiException("标签页组件启用可编辑模式时必须指定 DataSource 属性，硬编码无效");
                }
            }
            else
            {
                var activeTab = DataSource.FirstOrDefault(x => x.IsActive);
                if (activeTab == null)
                {
                    activeTab = DataSource.FirstOrDefault();
                    if (activeTab != null)
                    {
                        activeTab.IsActive = true;
                    }
                }
                DataSource.CollectionChanged -= DataSource_CollectionChanged;
                DataSource.CollectionChanged += DataSource_CollectionChanged;
            }
        }

        private void DataSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }
            foreach (var item in DataSource.Take(DataSource.Count - 1))
            {
                item.IsActive = false;
            }
            var repeatKeys = DataSource.GroupBy(x => x.Name).Where(x => x.Count() > 1).Select(x => x.Key).ToArray();
            if (repeatKeys.Any())
            {
                throw new BlazuiException($"Tab 页以下 Name 重复 {string.Join(",", repeatKeys)}");
            }
        }

        /// <summary>
        /// 点击加号按钮增加 Tab 页时触发
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnAddingTab { get; set; }

        internal BTabPanel ActiveTab { get; private set; }

        internal float BarOffsetLeft { get; set; }
        internal float BarWidth { get; set; }
        internal void AddTab(BTabPanel tab)
        {
            if (Exists(tab.Name))
            {
                return;
            }

            Console.WriteLine(tab.Title);
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
            var activeTabOption = DataSource?.FirstOrDefault(x => x.IsActive);
            var activeTab = tabPanels.FirstOrDefault(x => x.IsActive);
            if (activeTab == null)
            {
                activeTab = tabPanels.FirstOrDefault();
                if (activeTab != null)
                {
                    activeTab.Activate();
                    activeTab.Refresh();
                }
                else if (activeTabOption == null)
                {
                    if (firstRender)
                    {
                        Refresh();
                    }
                }
                else
                {
                    foreach (var item in tabPanels)
                    {
                        if (item.Name != activeTabOption.Name)
                        {
                            continue;
                        }
                        item.Refresh();
                    }
                }
            }
            if (!firstRender && !RequireRender)
            {
                return;
            }
            await SetActivateTabAsync(activeTab);
            await base.OnAfterRenderAsync(firstRender);
        }

        internal async Task UpdateHeaderSizeAsync(BTabPanel tabPanel, float barWidth, float barOffsetLeft)
        {
            if (headerSizeUpdated)
            {
                await TabRenderCompletedAsync(tabPanel);
                return;
            }
            headerSizeUpdated = true;
            BarWidth = barWidth;
            BarOffsetLeft = barOffsetLeft;
            RequireRender = true;
            StateHasChanged();
        }
        internal async Task TabRenderCompletedAsync(BTabPanel tabPanel)
        {
            if (OnRenderCompleted != null)
            {
                await OnRenderCompleted(tabPanel);
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
                ExceptionHelper.Throw(ExceptionHelper.TabNameNotFound, $"Name 为 {name} 的 Tab 不存在");
            }
            await SetActivateTabAsync(tab);
        }
        internal async Task<bool> SetActivateTabAsync(BTabPanel tab)
        {
            if (OnActiveTabChanging.HasDelegate)
            {
                var arg = new BChangeEventArgs<BTabPanel>();
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
            var eventArgs = new BChangeEventArgs<BTabPanel>();
            eventArgs.OldValue = ActiveTab;
            eventArgs.NewValue = tab;
            RequireRender = true;
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

        protected override void OnParametersSet()
        {
            if (Type == TabType.Normal && IsRemovable)
            {
                throw new NotSupportedException("TabType为Card的情况下才能进行移除");
            }
            base.OnParametersSet();
        }
    }
}
