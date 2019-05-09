using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.DropDownList
{
    public class BSimpleDropDownListContentBase : ComponentBase
    {
        public ElementRef Element { get; protected set; }
        [Parameter]
        public int ZIndex { get; set; }

        [Parameter]
        public bool IsShow { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
