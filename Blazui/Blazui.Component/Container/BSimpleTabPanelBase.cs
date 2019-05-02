using Blazui.Component.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Container
{
    public class BSimpleTabPanelBase : ComponentBase, ITab
    {
        private static int tabIndex = 0;
        protected bool isClosable
        {
            get
            {
                if (IsClosable.HasValue)
                {
                    return IsClosable.Value;
                }
                if (TabContainer.IsClosable.HasValue)
                {
                    return TabContainer.IsClosable.Value;
                }
                return TabContainer.IsEditable;
            }
        }

        [Parameter]
        public EventCallback<ITab> OnTabCloseAsync { get; set; }

        protected async Task RemoveTabPanelAsync(UIMouseEventArgs e)
        {
            if (!OnTabCloseAsync.HasDelegate)
            {
                return;
            }
            await OnTabCloseAsync.InvokeAsync(this);
        }

        public bool IsActive { get; set; }
        public ElementRef Element { get; set; }

        [CascadingParameter]
        public BSimpleTab TabContainer { get; set; }

        protected async Task Activate(UIMouseEventArgs e)
        {
            await TabContainer.SetActivateTabAsync(this);
        }

        [Parameter]
        public string Name { get; set; }

        public int Index { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool? IsClosable { get; set; }

        protected override async Task OnInitAsync()
        {
            Index = TabContainer.TabPanels.Count;
            await TabContainer.AddTabAsync(this);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                return;
            }
            Name = "tab_" + (++tabIndex);
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            TabContainer.RemoveTabAsync(this).GetAwaiter().GetResult();
        }


        [Parameter]
        public Func<ITab, Task> OnRenderCompletedAsync { get; set; }

        protected override async Task OnAfterRenderAsync()
        {
            if (OnRenderCompletedAsync != null)
            {
                await OnRenderCompletedAsync(this);
            }
            await base.OnAfterRenderAsync();
        }
    }
}
