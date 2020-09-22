using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Element.DisplayRenders
{
    internal class DisplayRenderFactory
    {
        private static IDictionary<Func<TableHeader, bool>, Type> renderMap = new Dictionary<Func<TableHeader, bool>, Type>();
        private readonly IServiceProvider provider;

        static DisplayRenderFactory()
        {
            renderMap.Add(x => x.Property.PropertyType == typeof(string), typeof(GenericRender));
            renderMap.Add(x => x.Property.PropertyType == typeof(int), typeof(GenericRender));
            renderMap.Add(x => x.Property.PropertyType == typeof(DateTime), typeof(DateTimeRender));
            renderMap.Add(x => x.Property.PropertyType == typeof(int?), typeof(GenericRender));
            renderMap.Add(x => x.Property.PropertyType == typeof(DateTime?), typeof(DateTimeRender));
            renderMap.Add(x => x.Property.PropertyType.IsEnum, typeof(EnumRender));
            renderMap.Add(x => x.Property.PropertyType.IsGenericType && (Nullable.GetUnderlyingType(x.Property.PropertyType) ?? x.Property.PropertyType).IsEnum, typeof(EnumRender));
        }
        public DisplayRenderFactory(IServiceProvider provider)
        {
            this.provider = provider;
        }
        public IDisplayRender CreateRenderFactory(TableHeader tableHeader)
        {
            var type = renderMap.FirstOrDefault(x => x.Key(tableHeader)).Value;
            return (IDisplayRender)provider.GetRequiredService(type);
        }
    }
}
