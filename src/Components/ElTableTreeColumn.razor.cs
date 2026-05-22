using System;
using System.Collections.Generic;
using System.Text;

namespace Element
{
    public partial class ElTableTreeColumn : ElTableColumn
    {
        internal override bool IsTree { get; set; } = true;
    }
}
