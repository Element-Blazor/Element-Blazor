using System;

namespace Element.ControlConfigs
{
    public class RadioAttribute : BaseAttribute
    {
        public RadioSize Size { get; set; }

        public bool IsDisabled { get; set; }

        public bool Button { get; set; }

        public bool Bordered { get; set; }

        public string Display { get; set; } = "Text";

        public string Value { get; set; } = "Value";

        public Type DataSourceLoader { get; set; }
    }
}
