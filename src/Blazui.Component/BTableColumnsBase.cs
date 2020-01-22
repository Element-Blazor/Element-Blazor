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
    public class BTableColumnsBase : BComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [CascadingParameter]
        public BTable Table { get; set; }

        public void AddColumn(BTableColumnBase column)
        {
            if (Table.DataType == null)
            {
                throw new BlazuiException($"表格 {Table.GetType().Name} 没有设置 DataType 属性");
            }
            if (column.Property == null && !(column is BTableTemplateColumnBase))
            {
                throw new BlazuiException($"列 {column.Text} 没有设置 {nameof(BTableColumnBase.Property)} 属性");
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
                Property = property,
                Eval = column.Property == null ? null : (Func<object, object>)(row =>
                {
                    var value = property.GetValue(row);
                    if (string.IsNullOrWhiteSpace(column.Format))
                    {
                        return value;
                    }
                    if (value == null)
                    {
                        return null;
                    }

                    try
                    {
                        return Convert.ToDateTime(value).ToString(column.Format);
                    }
                    catch (InvalidCastException)
                    {
                        throw new BlazuiException("仅日期列支持 Format 参数");
                    }
                }),
                Text = column.Text,
                Width = column.Width,
                IsCheckBox = column.IsCheckBox,
                Template = column.ChildContent,
                Format = column.Format
            };
            Table.Headers.Add(columnConfig);
        }
    }
}
