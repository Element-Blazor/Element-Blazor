using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Dynamic
{
    public class BDynamicComponent : ComponentBase
    {
        [Parameter]
        public object Component { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (Component is Type type)
            {
                builder.OpenComponent(0, type);
                builder.CloseComponent();
                return;
            }
            if (Component is RenderFragment renderFragment)
            {
                builder.AddContent(0, renderFragment);
                return;
            }
            builder.AddMarkupContent(0, Convert.ToString(Component));
        }
    }
}
