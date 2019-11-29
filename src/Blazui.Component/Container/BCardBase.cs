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
        public Func<TModel, string> Cls { get; set; }
        [Parameter]
        public Func<TModel, string> Style { get; set; }
        [Parameter]
        public Func<TModel, string> BodyCls { get; set; }

        [Parameter]
        public Func<TModel, ShadowShowType> Shadow { get; set; }
        [Parameter]
        public IEnumerable<TModel> DataSource { get; set; }
        [Parameter]
        public RenderFragment<TModel> Header { get; set; }

        [Parameter]
        public RenderFragment<TModel> Body { get; set; }
    }
}
