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
            if (removingTab != null)
            {
                For.Remove(removingTab);
            }

            if (OnTabCloseAsync != null)
            {
                await OnTabCloseAsync(tab);
            }
            tab.TabContainer.Refresh();
        }

        [Parameter]
        public Func<ITab, Task> OnRenderCompletedAsync { get; set; }

        [Parameter]
        public EventCallback<BTabPanelBase<T>> OnAllTabRenderCompletedAsync { get; set; }

        protected override async Task OnAfterRenderAsync()
        {
            if (OnAllTabRenderCompletedAsync.HasDelegate)
            {
                await OnAllTabRenderCompletedAsync.InvokeAsync(this);
            }
            await base.OnAfterRenderAsync();
        }
    }
}
