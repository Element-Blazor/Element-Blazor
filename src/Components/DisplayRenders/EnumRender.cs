using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Blazui.Component.DisplayRenders
{
    internal class EnumRender : IDisplayRender
    {
        public Func<object, object> CreateRender(TableHeader tableHeader)
        {
            return row =>
            {
                object value = tableHeader.Property.GetValue(row);
                if (value != null)
                {
                    var valueType = value.GetType();
                    var finalType = Nullable.GetUnderlyingType(valueType) ?? valueType;
                    if (finalType.IsEnum)
                    {
                        var enumObj = Convert.ChangeType(value, finalType).ToString();
                        var attrs = finalType.GetField(enumObj).GetCustomAttributes();
                        var display = (attrs.OfType<DescriptionAttribute>().FirstOrDefault()?.Description ?? attrs.OfType<DisplayAttribute>().FirstOrDefault()?.Description) ?? enumObj;
                        value = display;
                    }
                }
                return value;
            };
        }
    }
}
