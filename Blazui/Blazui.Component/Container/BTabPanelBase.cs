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
        public BTabs Tabs { get; set; }

        protected async Task Activate(UIMouseEventArgs e)
        {
            await Tabs.SetActivateTabAsync(this);
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
            Index = Tabs.TabPanels.Count;
            await Tabs.AddTabAsync(this);
        }

        public void Dispose()
        {
            Tabs.RemoveTabAsync(this).GetAwaiter().GetResult();
        }
    }
}
