using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazui.Component.Table
{
    public class TableHeader<TRow>
    {
        public string Text { get; set; }
        public float? Width { get; set; }
        public string Property { get; set; }
        public Func<TRow, object> Eval { get; set; }
        public bool IsCheckBox { get; set; }
        public RenderFragment<TRow> Template { get; set; }
    }
}
