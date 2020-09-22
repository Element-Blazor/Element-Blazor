using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public interface IMenuItem
    {
        string Icon { get; set; }
        BMenuContainer Menu { get; set; }

        object Model { get; set; }
        MenuOptions Options { get; set; }

        void Activate();

        void DeActivate();
    }
}
