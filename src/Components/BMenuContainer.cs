using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class BMenuContainer : BComponentBase, IContainerComponent
    {
        internal List<IMenuItem> Children { get; set; } = new List<IMenuItem>();

        internal void AddMenuItem(IMenuItem item)
        {
            if (item == null || Children.Contains(item))
            {
                return;
            }
            Children.Add(item);
        }

        internal void RemoveMenuItem(IMenuItem item)
        {
            if (item == null)
            {
                return;
            }
            Children.Remove(item);
        }

        [CascadingParameter]
        public BMenu TopMenu { get; set; }
        public ElementReference Container { get; set; }

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
