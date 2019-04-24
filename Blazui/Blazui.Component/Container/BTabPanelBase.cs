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
        public ElementRef Element { get; set; }

        [CascadingParameter]
        private BTabs BTabs { get; set; }

        protected async Task Activate(UIMouseEventArgs e)
        {
            await BTabs.SetActivateTabAsync(this);
        }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string IsActive { get; set; }

        protected override async Task OnInitAsync()
        {
            await BTabs.AddTabAsync(this);
        }

        public void Dispose()
        {
            BTabs.RemoveTabAsync(this).GetAwaiter().GetResult();
        }

        private async Task ActivateAsync()
        {
            await BTabs.SetActivateTabAsync(this);
        }
    }
}
