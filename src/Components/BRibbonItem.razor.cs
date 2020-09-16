using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component
{
    public partial class BRibbonItem
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
