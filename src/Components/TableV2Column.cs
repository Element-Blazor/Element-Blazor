using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Element
{
    public class TableV2Column
    {
        public string Title { get; set; }

        public string Property { get; set; }

        public string Width { get; set; }

        public RenderFragment<object> Template { get; set; }

        public object GetValue(object row)
        {
            if (row == null || string.IsNullOrWhiteSpace(Property))
            {
                return null;
            }

            return row.GetType().GetProperty(Property, BindingFlags.Instance | BindingFlags.Public)?.GetValue(row);
        }
    }
}
