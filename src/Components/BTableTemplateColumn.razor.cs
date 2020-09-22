using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class BTableTemplateColumn
    {
        [Parameter]
        public override RenderFragment<object> ChildContent { get; set; }
    }
}
