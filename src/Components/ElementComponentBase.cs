using Element;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Element
{
    public class ElementComponentBase : ComponentBase, IDisposable
    {
        [CascadingParameter(Name ="Page")]
        public ElementComponentBase Page { get; set; }
        protected bool RequireRender { get; set; } = true;

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// 若该项为 true，则该组件会始终允许刷新，不受 <seealso cref="ElementComponentBase.MarkAsRequireRender"/> 方法控制
        /// </summary>
        [Parameter]
        public bool EnableAlwaysRender { get; set; }
        [Inject]
        Element.MessageBox MessageBox { get; set; }

        [Inject]
        public DialogService DialogService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        MessageService MessageService { get; set; }

        [Inject]
        public LoadingService LoadingService { get; set; }

        [Parameter]
        public Func<object, Task> OnRenderCompleted { get; set; }

        /// <summary>
        /// 自定义 CSS 类
        /// </summary>
        [Parameter]
        public virtual string Cls { get; set; }

        [CascadingParameter]
        public BBadge Badge { get; set; }
        /// <summary>
        /// 设置自定义样式
        /// </summary>
        [Parameter]
        public string Style { get; set; } = string.Empty;

        /// <summary>
        /// 弹出 Alert 消息
        /// </summary>
        /// <param name="text"></param>
        public void Alert(string text)
        {
            _ = MessageBox.AlertAsync(text);
        }

        protected override void OnInitialized()
        {
            if (DialogContainer != null)
            {
                DialogContainer.OnShow += OnDialogShowAsync;
            }
        }

        /// <summary>
        /// 弹出 Confirm 消息
        /// </summary>
        /// <param name="text"></param>
        public async Task<MessageBoxResult> ConfirmAsync(string text)
        {
            return await MessageBox.ConfirmAsync(text);
        }

        /// <summary>
        /// 默认情况下所有复杂组件都只进行一次渲染，该方法将组件置为需要再次渲染
        /// </summary>
        public virtual void MarkAsRequireRender()
        {
            RequireRender = true;
        }

        [CascadingParameter]
        public BDialogBase DialogContainer { get; set; }

        protected virtual Task OnDialogShowAsync()
        {
            return Task.CompletedTask;
        }
        public void Toast(string text)
        {
            MessageService.Show(text);
        }
        public void Toast(string text, MessageType type)
        {
            MessageService.Show(text, type);
        }
        public async Task<MessageBoxResult> AlertAsync(string text)
        {
            return await MessageBox.AlertAsync(text);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            RequireRender = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            if (OnRenderCompleted != null)
            {
                await OnRenderCompleted(this);
            }
            RequireRender = false;
        }

        public virtual void Refresh()
        {
            MarkAsRequireRender();
            StateHasChanged();
        }

        protected override bool ShouldRender()
        {
            return RequireRender || EnableAlwaysRender;
        }

        public virtual void Dispose()
        {
            if (DialogContainer == null)
            {
                return;
            }
            DialogContainer.OnShow -= OnDialogShowAsync;
        }
    }
}
