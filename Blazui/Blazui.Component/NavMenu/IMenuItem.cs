using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.NavMenu
{
    public interface IMenuItem
    {
        void Active();

        void DeActive();
    }
}
