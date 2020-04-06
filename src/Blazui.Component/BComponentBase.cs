

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BComponentBase : ComponentBase
    {
        protected bool RequireRender { get; set; }

        /// <summary>
        /// 若该项为 true，则该组件会始终允许刷新，不受 <seealso cref="BComponentBase.MarkAsRequireRender"/> 方法控制
        /// </summary>
        [Parameter]
        public bool EnableAlwaysRender { get; set; }
        [Inject]
        MessageBox MessageBox { get; set; }

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
        public BBadgeBase Badge { get; set; }
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
        public void MarkAsRequireRender()
        {
            RequireRender = true;
        }

        public void Toast(string text)
        {
            MessageService.Show(text);
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

        public void Refresh()
        {
            StateHasChanged();
        }

        protected override bool ShouldRender()
        {
            return RequireRender || EnableAlwaysRender;
        }
    }
}
