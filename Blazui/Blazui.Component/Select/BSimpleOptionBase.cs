using Blazui.Component.Select;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Select
{
    public class BSimpleOptionBase : ComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [CascadingParameter]
        public BSimpleSelect DropDownList { get; set; }
    }
}
