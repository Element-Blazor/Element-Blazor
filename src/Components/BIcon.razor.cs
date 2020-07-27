using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Blazui.Component
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
    }
}
