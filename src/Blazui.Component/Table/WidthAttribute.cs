using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Table
{
    public class WidthAttribute : Attribute
    {
        public float Width { get; }
        public WidthAttribute(float width)
        {
            Width = width;
        }
    }
}
