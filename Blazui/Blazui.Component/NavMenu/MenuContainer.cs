using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.NavMenu
{
    public class MenuContainer : ComponentBase
    {
        protected List<IMenuItem> _children = new List<IMenuItem>();
        protected IMenuItem _activedItem;

        public virtual void AppendItem(IMenuItem item) {
            _children.Add(item);
            item.Container = this;
        }

        public virtual void ActiveItem(IMenuItem item) {
            if (item == null)
                return;
            if (item == _activedItem)
                return;

            item.Active();

            if (_activedItem != null)
                _activedItem.DeActive();

            _activedItem = item;
           // StateHasChanged();
        }
    }
}
