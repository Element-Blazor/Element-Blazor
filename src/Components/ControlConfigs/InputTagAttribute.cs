namespace Element.ControlConfigs
{
    public class InputTagAttribute : BaseAttribute
    {
        public string Placeholder { get; set; } = "请输入";

        public bool Clearable { get; set; }

        public bool IsDisabled { get; set; }

        public bool Readonly { get; set; }

        public InputSize Size { get; set; } = InputSize.Normal;

        public int Max { get; set; }

        public bool AllowDuplicates { get; set; }

        public string[] TriggerKeys { get; set; } = new[] { "Enter", "," };

        public bool AddOnBlur { get; set; } = true;
    }
}
