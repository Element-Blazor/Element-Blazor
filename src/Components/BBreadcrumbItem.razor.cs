using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazui.Component
{
    public partial class BBreadcrumbItem
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
