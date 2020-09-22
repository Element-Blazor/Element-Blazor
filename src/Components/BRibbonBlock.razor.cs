using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Element
{
    public partial class BRibbonBlock
    {
        /// <summary>
        /// Ribbon Tab 标签页区块标题
        /// </summary>
        [Parameter]
        public string Title { get; set; }

        /// <summary>
        /// 区块内容
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
