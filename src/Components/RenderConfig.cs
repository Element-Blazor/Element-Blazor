using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Element
{
    public class RenderConfig
    {
        public RenderConfig()
        {
            Console.WriteLine("create render config");
        }
        public bool IsRequired { get; set; }
        public string RequiredMessage { get; set; }
        public Type InputControlType { get; set; }
        public object ControlAttribute { get; set; }
        public string Placeholder { get; set; }
        public PropertyInfo Property { get; set; }
        public object InputControl { get; set; }
        public object DataSource { get; set; }
        public Type DataSourceLoader { get; set; }

        private object editValue = null;
        public object EditingValue
        {
            get
            {
                Console.WriteLine("getValue:" + editValue);
                return editValue;
            }
            set
            {
                editValue = value;
                Console.WriteLine("setValue:" + value);
            }
        }
        public object Page { get; internal set; }
        public object RawValue { get; set; }
        public object RawLabel { get; set; }
        public bool RawInfoHasSet { get; set; }
    }
}
