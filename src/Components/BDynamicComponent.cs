using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    /// <summary>
    /// 提供动态加载组件的功能
    /// </summary>
    public class BDynamicComponent : ComponentBase
    {
        [Parameter]
        public object Component { get; set; }

        [Parameter]
        public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (Component is Type type)
            {
                builder.OpenComponent(0, type);
                var seq = 1;
                foreach (var key in Parameters.Keys)
                {
                    builder.AddAttribute(seq++, key, Parameters[key]);
                }
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
