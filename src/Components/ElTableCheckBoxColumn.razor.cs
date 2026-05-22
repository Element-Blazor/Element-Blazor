using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElTableCheckBoxColumn : ElTableColumn
    {
        internal override bool IsCheckBox { get; set; } = true;
    }
}
