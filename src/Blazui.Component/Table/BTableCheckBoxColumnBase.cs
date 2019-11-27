using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Table
{
    public class BTableCheckBoxColumnBase<TRow> : BTableColumn<TRow>
    {
        internal override bool IsCheckBox { get; set; } = true;
    }
}
