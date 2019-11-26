using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Table
{
    public class BTableDateTimeColumnBase<TRow> : BTableColumn<TRow>
    {
        [Parameter]
        public string Format { get; set; }
    }
}
