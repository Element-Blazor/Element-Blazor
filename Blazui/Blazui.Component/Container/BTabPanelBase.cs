using Blazui.Component.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Container
{
    public class BTabPanelBase : ComponentBase, ITab
    {
        public bool IsActive { get; set; }
        public ElementRef Element { get; set; }

        [CascadingParameter]
        public BTab TabContainer { get; set; }

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

        protected override async Task OnInitAsync()
        {
            Index = TabContainer.TabPanels.Count;
            await TabContainer.AddTabAsync(this);
        }

        public void Dispose()
        {
            TabContainer.RemoveTabAsync(this).GetAwaiter().GetResult();
        }

        public event Func<ITab, Task> OnRenderCompletedAsync;

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
