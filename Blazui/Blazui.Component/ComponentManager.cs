using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    internal class ComponentManager
    {
        private static int zindex = 2000;
        public static int GenerateZIndex()
        {
            return zindex++;
        }
    }
}
