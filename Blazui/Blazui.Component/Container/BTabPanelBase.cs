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
        protected ElementRef tabPanel;

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [CascadingParameter]
        private BTabs BTabs { get; set; }

        protected async Task Activate(UIMouseEventArgs e)
        {
            var dom = tabPanel.Dom(JSRuntime);
            var width = await dom.GetClientWidthAsync();
            var offsetLeft = await dom.GetOffsetLeftAsync();
            BTabs.BarWidth = width;
            BTabs.BarOffsetLeft = offsetLeft;
        }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string IsActive { get; set; }

        protected override void OnInit()
        {
            BTabs.AddTab(this);
        }

        public void Dispose()
        {
            BTabs.RemoveTab(this);
        }

        private void Activate()
        {
            BTabs.SetActivateTab(this);
        }
    }
}
