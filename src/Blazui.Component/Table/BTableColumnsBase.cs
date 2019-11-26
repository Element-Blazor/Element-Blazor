using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blazui.Component.Table
{
    public class BTableColumnsBase<TRow> : ComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [CascadingParameter]
        public BTable<TRow> Table { get; set; }

        public void AddColumn(BTableColumnBase<TRow> column)
        {
            var columnConfig = new TableHeader<TRow>
            {
                Property = column.Property == null ? string.Empty : GetPropertyName(column.Property),
                Eval = column.Property == null ? null : column.Property.Compile(),
                Text = column.Text,
                Width = column.Width,
                IsCheckBox = column.IsCheckBox,
                Template = column.ChildContent
            };
            Table.Headers.Add(columnConfig);
        }

        private string GetPropertyName(Expression<Func<TRow, object>> propertyGetter)
        {
            if (propertyGetter.Body is UnaryExpression unaryExpression)
            {
                return ((MemberExpression)unaryExpression.Operand).Member.Name;
            }
            return ((MemberExpression)propertyGetter.Body).Member.Name;
        }
    }
}
