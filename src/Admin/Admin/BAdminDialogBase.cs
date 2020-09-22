using Blazui.Admin.Abstract;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Admin
{
    public class BAdminDialogBase : BAdminPageBase
    {
        /// <summary>
        /// 可用于操作当前窗口
        /// </summary>
        [Parameter]
        public DialogOption Dialog { get; set; }

        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        /// <param name="result">窗口返回值，该值将作为 <seealso cref="DialogResult"/> 的 <seealso cref="DialogResult.Result"/> 属性</param>
        /// <returns></returns>
        public Task CloseAsync<T>(T result)
        {
            return Dialog.CloseDialogAsync(result);
        }

        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        /// <returns></returns>
        public Task CloseAsync()
        {
            return Dialog.CloseDialogAsync();
        }
    }
}
