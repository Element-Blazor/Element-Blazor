using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.NavMenu
{
    public interface IMenuItem
    {
        MenuContainer Container { get; set; }

        MenuOptions Options { get; set; }

        void Active();

        void DeActive();
    }
}
