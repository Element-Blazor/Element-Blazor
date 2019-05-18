using Blazui.Component.Dom;
using Blazui.Component.EventArgs;
using Blazui.Component.Popup;
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

        protected BPopupBase popup;

        [Inject]
        private Document document { get; set; }

        private BoundingClientRect rect;
        private Style style;

        protected float left { get; set; }
        protected float top { get; set; }
        protected float width { get; set; }
        [Parameter]
        public string Placeholder { get; set; } = "请选择";
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
        public event Action OnDispose;

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
            await ToggleAsync();
            if (OnSelect.HasDelegate)
            {
                await OnSelect.InvokeAsync(args);
            }
            else
            {
                StateHasChanged();
            }
        }

        private bool executingToogle = false;

        private async Task ToggleAsync()
        {
            if (executingToogle)
            {
                return;
            }
            executingToogle = true;
            if (IsShow)
            {
                popup.IsAlllowReRender = false;
                await style.SetTransformAsync("scaleY(0)");
                await Task.Delay(200);
                popup.IsAlllowReRender = true;
            }
            IsShow = !IsShow;
            if (!IsShow)
            {
                executingToogle = false;
                return;
            }
            var selectDom = elementSelect.Dom(JSRuntime);
            rect = await selectDom.GetBoundingClientRectAsync();
            left = rect.Left;
            top = rect.Top + rect.Height;
            width = rect.Width;
            popup.OnRenderCompleted += Popup_OnRenderCompletedAsync;
        }

        private async Task Popup_OnRenderCompletedAsync()
        {
            popup.OnRenderCompleted -= Popup_OnRenderCompletedAsync;
            await Task.Delay(10);
            await style.SetTransformAsync("scaleY(1)");
            executingToogle = false;
        }

        [Parameter]
        internal EventCallback<UIMouseEventArgs> OnClick { get; set; }

        protected async Task OnSelectClickAsync(UIMouseEventArgs e)
        {
            await ToggleAsync();
        }

        protected override async Task OnAfterRenderAsync()
        {
            popup.OnHide += HideAsync;
            style = content.Dom(JSRuntime).Style;
            await Task.CompletedTask;
        }

        private async Task HideAsync()
        {
            popup.OnHide -= HideAsync;
            if (!IsShow)
            {
                return;
            }
            await ToggleAsync();
        }
    }
}
