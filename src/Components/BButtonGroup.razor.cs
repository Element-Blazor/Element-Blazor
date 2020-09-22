using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class BButtonGroup
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
