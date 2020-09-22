using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Element
{
    public partial class BGroupBox
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
