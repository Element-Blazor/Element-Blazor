using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.NavMenu
{
    public interface IMenuItem
    {
        BMenuContainer Menu { get; set; }

        object Model { get; set; }
        MenuOptions Options { get; set; }

        void Activate();

        void DeActivate();
    }
}
