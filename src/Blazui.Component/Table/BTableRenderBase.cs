using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Table
{
    public class BTableRenderBase<TRow> : ComponentBase
    {
        [Parameter]
        public BTable<TRow> Table { get; set; }

        public List<TableHeader> Headers
        {
            get
            {
                return Table.Headers;
            }
        }

        public List<TRow> Rows
        {
            get
            {
                return Table.DataSource;
            }
        }
    }
}
