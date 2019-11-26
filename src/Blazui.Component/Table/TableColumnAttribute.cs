using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Table
{
    public class TableColumnAttribute : Attribute
    {
        public string Text { get; set; }
        public int Width { get; set; }
    }
}
