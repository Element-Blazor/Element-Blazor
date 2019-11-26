using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Container
{
    public class BSimpleCardBase : ComponentBase
    {
        protected List<string> titles { get; set; } = new List<string>();
        [Parameter]
        public string Title { get; set; }

        protected override Task OnParametersSetAsync()
        {
            titles.Add(Title);
            return base.OnParametersSetAsync();
        }
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
