using Blazui.Component.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Container
{
    public class BTabPanelBase<T> : ComponentBase
    {
        [Parameter]
        public ObservableCollection<T> DataSource { get; set; }

        [Parameter]
        public Func<T, string> Name { get; set; }
        [Parameter]
        public Func<T, bool> IsDisabled { get; set; }

        public ElementReference Element { get; set; }

        [CascadingParameter]
        public BTab TabContainer { get; set; }

        //protected async Task Activate(MouseEventArgs e)
        //{
        //    await TabContainer.SetActivateTabAsync(this);
        //}
        protected IList<TabOption> models = new List<TabOption>();

        [Parameter]
        public Func<T, string> Title { get; set; }

        [Parameter]
        public Func<T, bool> IsClosable { get; set; }

        [Parameter]
        public Func<T, object> Content { get; set; }

        protected override void OnParametersSet()
        {
            if (Name == null)
            {
                throw new ArgumentException("必须指定Name属性");
            }
            if (Title == null)
            {
                throw new ArgumentException("必须指定Title属性");
            }
        }

        protected override void OnInitialized()
        {
            DataSource.CollectionChanged -= DataSource_CollectionChanged;
            DataSource.CollectionChanged += DataSource_CollectionChanged;
            models = DataSource.Select(x => new TabOption()
            {
                IsClosable = IsClosable?.Invoke(x),
                IsDisabled = IsDisabled?.Invoke(x),
                Name = Name(x),
                Title = Title(x),
                Content = Content(x)
            }).ToList();
        }

        private void DataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            models = DataSource.Select(x => new TabOption()
            {
                IsClosable = IsClosable?.Invoke(x),
                IsDisabled = IsDisabled?.Invoke(x),
                Name = Name(x),
                Title = Title(x),
                Content = Content(x)
            }).ToList();
        }

        //protected async Task OnRenderCompletedInternalAsync(ITab tab)
        //{
        //    if (OnRenderCompleted.HasDelegate)
        //    {
        //        await OnRenderCompleted.InvokeAsync(tab);
        //    }
        //}

        [Parameter]
        public Func<ITab, Task<bool>> OnTabClosingAsync { get; set; }

        [Parameter]
        public EventCallback<ITab> OnTabClose { get; set; }

        protected async Task RemoveTabCloseAsync(ITab tab)
        {
            if (OnTabClosingAsync != null)
            {
                if (!await OnTabClosingAsync(tab))
                {
                    return;
                }
            }

            var removingTabModel = DataSource.FirstOrDefault(x => Name(x) == tab.Name);
            var removingIndex = DataSource.IndexOf(removingTabModel);
            if (removingTabModel != null)
            {
                DataSource.Remove(removingTabModel);
            }

            var removingTab = tab.TabContainer.TabPanels.FirstOrDefault(x => x.Name == tab.Name);
            //tab.TabContainer.TabPanels.Remove(removingTab);
            if (removingTab.Name == tab.TabContainer.activeTabName)
            {
                var activeIndex = removingIndex++;
                if (removingIndex > DataSource.Count - 1)
                {
                    activeIndex = DataSource.Count - 1;
                }
                else
                {
                    activeIndex = removingIndex;
                }
                var activeModel = DataSource.ElementAt(activeIndex);
                await tab.TabContainer.SetActivateTabAsync(Name(activeModel));
                tab.TabContainer.Refresh();
            }
            if (OnTabClose.HasDelegate)
            {
                _ = OnTabClose.InvokeAsync(tab);
            }
        }

        [Parameter]
        public EventCallback<ITab> OnRenderCompleted { get; set; }

        [Parameter]
        public EventCallback<ITab> OnEachTabRenderCompleted { get; set; }

        protected async Task OnInternalEachTabRenderCompleted(ITab tab)
        {
            if (OnEachTabRenderCompleted.HasDelegate)
            {
                await OnEachTabRenderCompleted.InvokeAsync(tab);
            }
        }

        [Parameter]
        public EventCallback<BTabPanelBase<T>> OnAllTabRenderCompleted { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (OnAllTabRenderCompleted.HasDelegate)
            {
                await OnAllTabRenderCompleted.InvokeAsync(this);
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
