using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Element
{
    public partial class BRibbonItem
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
