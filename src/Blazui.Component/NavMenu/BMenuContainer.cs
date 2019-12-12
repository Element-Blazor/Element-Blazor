using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.NavMenu
{
    public class BMenuContainer : ComponentBase
    {
        internal List<IMenuItem> Children { get; set; } = new List<IMenuItem>();

        [CascadingParameter]
        public BMenu TopMenu { get; set; }


        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
