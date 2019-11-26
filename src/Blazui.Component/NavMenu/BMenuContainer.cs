using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.NavMenu
{
    public class BMenuContainer : ComponentBase
    {
        protected List<IMenuItem> _children = new List<IMenuItem>();

        [CascadingParameter]
        public BMenu TopMenu { get; set; }
    

    }
}
