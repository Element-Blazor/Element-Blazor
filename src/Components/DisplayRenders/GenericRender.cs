using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Blazui.Component.DisplayRenders
{
    internal class GenericRender : IDisplayRender
    {
        public Func<object, object> CreateRender(TableHeader tableHeader)
        {
            return row =>
            {
                var propertyInfo = tableHeader.Property;
                var value = propertyInfo.GetValue(row);
                return value?.ToString();
            };
        }
    }
}
