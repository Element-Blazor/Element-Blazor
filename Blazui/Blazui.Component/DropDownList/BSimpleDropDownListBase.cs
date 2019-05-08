using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.DropDownList
{
    public class BSimpleDropDownListBase : ComponentBase
    {
        [Parameter]
        public string Placeholder { get; set; } = "请选择";

        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
