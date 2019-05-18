using Blazui.Component.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Popup
{
    public class BPopupBase : ComponentBase, IDisposable
    {
        [Parameter]
        protected RenderFragment ChildContent { get; set; }
        protected ElementRef parent;
        private bool stopRender;
        private Style style;
        private bool isShow;
        private Element parentDom;

        public event Func<Task> OnRenderCompleted;
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private Document document { get; set; }
        public Action OnDispose { get; internal set; }
        public int ZIndex { get; internal set; }
        public bool IsAlllowReRender { get; set; } = true;

        public event Func<Task> OnHide;
        protected override async Task OnInitAsync()
        {
            await ComponentManager.RegisterPopupComponentAsync(this);
        }

        protected override async Task OnAfterRenderAsync()
        {
            //Console.WriteLine("OnAfterRenderAsync");
            if (stopRender)
            {
                return;
            }
            //parentDom = parent.Dom(JSRuntime);
            //await parentDom.ChildMoveToBodyAsync();
            //var child = await parentDom.GetChildAsync(0);
            await document.AppendAsync(parent);
            //style = childDom.Style;
            //if (!isShow)
            //{
            //    return;
            //}
            //await Task.Delay(10);
            //await style.SetTransformAsync("scaleY(1)");
            if (OnRenderCompleted != null)
            {
                await OnRenderCompleted();
            }
        }

        protected override bool ShouldRender()
        {
            return IsAlllowReRender;
        }

        public async Task HideAsync()
        {
            if (OnHide == null)
            {
                return;
            }
            await OnHide();
        }

        public void Dispose()
        {
            OnDispose?.Invoke();
        }
    }
}
