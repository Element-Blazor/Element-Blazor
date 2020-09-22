using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Element
{
    public partial class BIcon
    {
        private string icon;
        /// <summary>
        /// 图标类型
        /// </summary>
        [Parameter]
        public Icon Icon { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnParametersSet()
        {
            icon = Icon.GetType().GetField(Icon.ToString()).GetCustomAttribute<DisplayAttribute>()?.Prompt;
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        [Parameter]
        public EventCallback OnClick { get; set; }

        private async System.Threading.Tasks.Task OnElClickAsync(MouseEventArgs e)
        {
            if (!OnClick.HasDelegate)
            {
                return;
            }
            await OnClick.InvokeAsync(e);
        }
    }
}
