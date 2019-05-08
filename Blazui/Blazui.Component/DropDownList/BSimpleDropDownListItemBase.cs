using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.DropDownList
{
    public class BDropDownListItemBase : ComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [CascadingParameter]
        public BSimpleDropDownList DropDownList { get; set; }
    }
}
