using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class BBreadcrumbItem : BComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
