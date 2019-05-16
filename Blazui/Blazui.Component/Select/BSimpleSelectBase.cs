using Blazui.Component.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blazui.Component.Select
{
    public class BSimpleSelectBase : ComponentBase
    {
        protected ElementRef content;

        protected ElementRef elementSelect;
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private Document document { get; set; }

        private BoundingClientRect rect;
        private Style style;

        protected float left { get; set; }
        protected float top { get; set; }
        protected float width { get; set; }
        [Parameter]
        public string Placeholder { get; set; } = "请选择";
        protected int zIndex { get; set; } = ComponentManager.GenerateZIndex();
        private Task hideTask;
        [Parameter]
        public bool IsShow { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        internal EventCallback<UIMouseEventArgs> OnClick { get; set; }
        protected async Task OnSelectClickAsync(UIMouseEventArgs e)
        {
            if (IsShow)
            {
                await style.SetTransformAsync("scaleY(0)");
                await Task.Delay(10);
            }
            IsShow = !IsShow;
            if (!IsShow)
            {
                return;
            }
            var selectDom = elementSelect.Dom(JSRuntime);
            rect = await selectDom.GetBoundingClientRectAsync();
            left = rect.Left;
            top = rect.Top + rect.Height;
            width = rect.Width;
        }

        protected override async Task OnAfterRenderAsync()
        {
            if (!IsShow)
            {
                return;
            }
            await document.AppendAsync(content);
            style = content.Dom(JSRuntime).Style;
            await Task.Delay(10);
            await style.SetTransformAsync("scaleY(1)");
        }
    }
}
