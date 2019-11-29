using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Container
{
    public class BCardBase<TModel> : ComponentBase
    {
        [Parameter]
        public RenderFragment<TModel> Cls { get; set; }
        [Parameter]
        public RenderFragment<TModel> BodyCls { get; set; }
        [Parameter]
        public IEnumerable<TModel> DataSource { get; set; }
        [Parameter]
        public RenderFragment<TModel> Header { get; set; }

        [Parameter]
        public RenderFragment<TModel> Body { get; set; }
    }
}
