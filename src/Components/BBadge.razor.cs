using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public partial class BBadge 
    {
        internal BHtml html;
        [Parameter]
        public int Value { get; set; }
        /// <summary>
        /// Badge 类型
        /// </summary>
        [Parameter]
        public BadgeType Type { get; set; } = BadgeType.Info;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        internal void SetValue(int value)
        {
            html.SetHtml(value.ToString());
            html.Refresh();
        }
    }
}
