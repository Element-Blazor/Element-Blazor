using Blazui.Component.Dom;
using Blazui.Component.EventArgs;
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
        [Parameter]
        public bool IsShow { get; set; }
        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Value { get; set; }

        public string Text { get; set; }

        private bool stopRender;

        public event Func<ChangeEventArgs<BSimpleOptionBase>, Task<bool>> OnSelectingAsync;

        public event Func<ChangeEventArgs<BSimpleOptionBase>, Task> OnSelectAsync;

        [Parameter]
        public EventCallback<ChangeEventArgs<BSimpleOptionBase>> OnSelect { get; set; }

        [Parameter]
        public EventCallback<ChangeEventArgs<BSimpleOptionBase>> OnSelecting { get; set; }

        internal BSimpleOptionBase CurrentSelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                Value = value.Value;
                selectedItem = value;
            }
        }

        private BSimpleOptionBase selectedItem;

        internal void Refresh()
        {
            StateHasChanged();
        }

        internal async Task OnInternalSelectAsync(BSimpleOptionBase item)
        {
            var args = new ChangeEventArgs<BSimpleOptionBase>();
            args.NewValue = item;
            args.OldValue = CurrentSelectedItem;
            if (OnSelectingAsync != null)
            {
                var disallowChange = await OnSelectingAsync(args);
                if (disallowChange || args.DisallowChange)
                {
                    return;
                }
            }
            if (OnSelecting.HasDelegate)
            {
                await OnSelecting.InvokeAsync(args);
                if (args.DisallowChange)
                {
                    return;
                }
            }
            CurrentSelectedItem = item;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(item.Value);
            }
            if (OnSelectAsync != null)
            {
                await OnSelectAsync(args);
            }
            await Toggle();
            if (OnSelect.HasDelegate)
            {
                await OnSelect.InvokeAsync(args);
            }
            else
            {
                StateHasChanged();
            }
        }

        private async Task Toggle()
        {
            if (IsShow)
            {
                stopRender = true;
                await style.SetTransformAsync("scaleY(0)");
                await Task.Delay(10);
            }
            stopRender = false;
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

        [Parameter]
        internal EventCallback<UIMouseEventArgs> OnClick { get; set; }
        protected async Task OnSelectClickAsync(UIMouseEventArgs e)
        {
            await Toggle();
        }


        protected override async Task OnAfterRenderAsync()
        {
            //if (previousSelectedItem != CurrentSelectedItem)
            //{
            //    previousSelectedItem = CurrentSelectedItem;
            //    StateHasChanged();
            //    return;
            //}
            if (stopRender)
            {
                return;
            }
            style = content.Dom(JSRuntime).Style;
            if (!IsShow)
            {
                return;
            }
            await document.AppendAsync(content);
            await Task.Delay(10);
            await style.SetTransformAsync("scaleY(1)");
        }
    }
}
