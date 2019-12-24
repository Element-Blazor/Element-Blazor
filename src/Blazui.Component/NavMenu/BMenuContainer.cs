using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.NavMenu
{
    public class BMenuContainer : ComponentBase, IContainerComponent
    {
        internal List<IMenuItem> Children { get; set; } = new List<IMenuItem>();

        [CascadingParameter]
        public BMenu TopMenu { get; set; }
        [Inject]
        public LoadingService LoadingService { get; set; }
        public ElementReference Container { get; set; }

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
