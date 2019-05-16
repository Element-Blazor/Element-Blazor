using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Popup
{
    public class BPopupBase : ComponentBase
    {
        [Parameter]
        protected RenderFragment ChildContent { get; set; }

    }
}
