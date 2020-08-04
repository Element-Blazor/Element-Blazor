using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Blazui.Component
{
    public class RenderConfig
    {
        public bool IsRequired { get; set; }
        public string RequiredMessage { get; set; }
        public Type InputControlType { get; set; }
        public object ControlAttribute { get; set; }
        public string Placeholder { get; set; }
        public PropertyInfo Property { get; set; }
        public object InputControl { get; set; }
        public PropertyInfo EntityProperty { get; set; }
        public object DataSource { get; set; }
        public Type DataSourceLoader { get; set; }
        public object EditingValue { get; internal set; }
        public object Page { get; internal set; }
        public object RawValue { get; set; }
        public object RawLabel { get; set; }
        public bool RawInfoHasSet { get; set; }
    }
}
