using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BTableTemplateColumnBase : BTableColumn
    {
        [Parameter]
        public override RenderFragment<object> ChildContent { get; set; }
    }
}
