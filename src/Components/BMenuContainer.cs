using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class BMenuContainer : ElementComponentBase, IContainerComponent
    {
        internal List<IMenuItem> Children { get; set; } = new List<IMenuItem>();

        [CascadingParameter]
        public BMenu TopMenu { get; set; }
        public ElementReference Container { get; set; }

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
