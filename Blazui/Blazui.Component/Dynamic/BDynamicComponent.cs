using Microsoft.AspNetCore.Components;
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
        public Type Component { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent(0, Component);
            builder.CloseComponent();
        }
    }
}
