using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Element.DisplayRenders
{
    internal class DateTimeRender : IDisplayRender
    {
        public Func<object, object> CreateRender(TableHeader tableHeader)
        {
            return row =>
            {
                var propertyInfo = tableHeader.Property;
                var value = propertyInfo.GetValue(row);
                if (string.IsNullOrWhiteSpace(tableHeader.Format))
                {
                    return value?.ToString();
                }
                return Convert.ToDateTime(value).ToString(tableHeader.Format);
            };
        }
    }
}
