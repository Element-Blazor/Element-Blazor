using Blazui.Component.DisplayRenders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public partial class BTableColumns
    {
        [Inject]
        DisplayRenderFactory displayRender { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [CascadingParameter]
        public BTable Table { get; set; }

        public void AddColumn(BTableColumn column)
        {
            if (Table.DataType == null)
            {
                throw new BlazuiException($"表格 {Table.GetType().Name} 没有设置 {nameof(BTable.DataSource)} 属性或该属性为空");
            }
            if (column.Property == null && !(column is BTableTemplateColumn))
            {
                throw new BlazuiException($"列 {column.Text} 没有设置 {nameof(BTableColumn.Property)} 属性");
            }

            PropertyInfo property = null;
            if (!string.IsNullOrWhiteSpace(column.Property))
            {
                property = Table.DataType.GetProperty(column.Property);
                if (property == null)
                {
                    throw new BlazuiException($"属性 {column.Property} 在 {Table.DataType.Name} 中不存在");
                }
            }
            var columnConfig = new TableHeader
            {
                EvalRaw = row =>
                {
                    object value = property.GetValue(row);
                    return value;
                },
                Property = property,
                Text = column.Text,
                SortNo = column.SortNo,
                Width = column.Width,
                IsCheckBox = column.IsCheckBox,
                Template = column.ChildContent,
                Format = column.Format,
                IsTree = column.IsTree,
                IsEditable = column.IsEditable
            };
            if (columnConfig.Property != null)
            {
                columnConfig.Eval = displayRender.CreateRenderFactory(columnConfig).CreateRender(columnConfig);
            }
            Table.Headers.Add(columnConfig);
        }
    }
}
