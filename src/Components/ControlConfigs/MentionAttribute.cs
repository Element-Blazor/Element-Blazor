namespace Element.ControlConfigs
{
    using System;

    public class MentionAttribute : BaseAttribute
    {
        public string Placeholder { get; set; }

        public string Prefix { get; set; } = "@";

        public string[] Prefixes { get; set; }

        public int Rows { get; set; } = 2;

        public bool IsDisabled { get; set; }

        public bool Readonly { get; set; }

        public InputSize Size { get; set; } = InputSize.Normal;

        public bool Clearable { get; set; }

        public string Display { get; set; } = "Label";

        public string Value { get; set; } = "Value";

        public Type DataSourceLoader { get; set; }
    }
}
