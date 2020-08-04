using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    /// <summary>
    /// 对话框基类，提供对话框常用方法
    /// </summary>
    public class BDialogBase : BComponentBase
    {
        /// <summary>
        /// 可用于操作当前窗口
        /// </summary>
        [Parameter]
        public DialogOption Dialog { get; set; }

        public event Func<Task> OnShow;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Dialog.OnShow = () =>
            {
                if (OnShow != null)
                {
                    return OnShow();
                }
                return Task.CompletedTask;
            };
        }
        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        /// <param name="result">窗口返回值，该值将作为 <seealso cref="DialogResult"/> 的 <seealso cref="DialogResult.Result"/> 属性</param>
        /// <returns></returns>
        public virtual Task CloseAsync<T>(T result)
        {
            return Dialog.CloseDialogAsync(result);
        }

        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        /// <returns></returns>
        public virtual Task CloseAsync()
        {
            return Dialog.CloseDialogAsync();
        }
    }
}
