using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blazui.Component.Table
{
    public class BTableColumnBase<TRow> : ComponentBase
    {
        internal virtual bool IsCheckBox { get; set; }
        [Parameter]
        public virtual int? Width { get; set; }

        [Parameter]
        public Expression<Func<TRow, object>> Property { get; set; }

        [CascadingParameter]
        public BTableColumns<TRow> Columns { get; set; }

        [Parameter]
        public virtual RenderFragment<TRow> ChildContent { get; set; }

        [Parameter]
        public string Text { get; set; }

        protected override void OnParametersSet()
        {
            if (Columns == null)
            {
                return;
            }
            Columns.AddColumn(this);
        }
    }
}
