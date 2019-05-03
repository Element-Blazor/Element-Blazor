using Blazui.Component.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Container
{
    public class BTabPanelBase<T> : ComponentBase
    {
        [Parameter]
        public IList<T> For { get; set; }

        [Parameter]
        public Func<T, string> Name { get; set; }
        [Parameter]
        public Func<T, bool> IsDisabled { get; set; }

        //protected async Task RemoveTabPanelAsync(UIMouseEventArgs e)
        //{
        //    var name = Name(this);
        //    For.Remove(For.FirstOrDefault(x => Name(x) == name));
        //    await TabContainer.RemoveTabAsync(this);
        //}

        public bool IsActive { get; set; }
        public ElementRef Element { get; set; }

        [CascadingParameter]
        public BTab TabContainer { get; set; }

        //protected async Task Activate(UIMouseEventArgs e)
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

        //protected override async Task OnInitAsync()
        //{
        //    Index = For.Count;
        //    await TabContainer.AddTabAsync(this);
        //}

        protected override async Task OnParametersSetAsync()
        {
            if (Name == null)
            {
                throw new ArgumentException("必须指定Name属性");
            }
            if (Title == null)
            {
                throw new ArgumentException("必须指定Title属性");
            }
            models = For.Select(x => new TabOption()
            {
                IsClosable = IsClosable?.Invoke(x),
                IsDisabled = IsDisabled?.Invoke(x),
                Name = Name(x),
                Title = Title(x),
                Content = Content(x)
            }).ToList();
            await Task.CompletedTask;
        }

        protected async Task OnRenderCompletedInternalAsync(ITab tab)
        {
            if (OnRenderCompletedAsync != null)
            {
                await OnRenderCompletedAsync(tab);
            }
        }

        [Parameter]
        public Func<ITab, Task<bool>> OnTabClosingAsync { get; set; }

        [Parameter]
        public Func<ITab, Task> OnTabCloseAsync { get; set; }

        protected async Task RemoveTabCloseAsync(ITab tab)
        {
            if (OnTabClosingAsync != null)
            {
                if (!await OnTabClosingAsync(tab))
                {
                    return;
                }
            }

            var removingTab = For.FirstOrDefault(x => Name(x) == tab.Name);
            var removingIndex = For.IndexOf(removingTab);
            if (removingTab != null)
            {
                For.Remove(removingTab);
            }
            var activeIndex = 0;
            if (removingIndex > For.Count - 1)
            {
                activeIndex = For.Count - 1;
            }

            if (OnTabCloseAsync != null)
            {
                await OnTabCloseAsync(tab);
            }
            var activeModel = For.ElementAt(activeIndex);
            await tab.TabContainer.SetActivateTabAsync(Name(activeModel));
        }

        public event Func<ITab, Task> OnRenderCompletedAsync;

        [Parameter]
        public Func<ITab, Task> OnEachTabRenderCompleted { get; set; }

        protected async Task OnInternalEachTabRenderCompleted(ITab tab)
        {
            if (OnEachTabRenderCompleted != null)
            {
                await OnEachTabRenderCompleted(tab);
            }
        }

        [Parameter]
        public Func<BTabPanelBase<T>, Task> OnAllTabRenderCompletedAsync { get; set; }

        protected override async Task OnAfterRenderAsync()
        {
            if (OnAllTabRenderCompletedAsync != null)
            {
                await OnAllTabRenderCompletedAsync(this);
            }
            await base.OnAfterRenderAsync();
        }
    }
}
