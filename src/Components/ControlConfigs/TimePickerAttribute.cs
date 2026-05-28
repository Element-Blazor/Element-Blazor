namespace Element.ControlConfigs
{
    public class TimePickerAttribute : BaseAttribute
    {
        public TimePickerType Type { get; set; } = TimePickerType.Time;

        public bool IsRange { get; set; }

        public string Format { get; set; } = "HH:mm:ss";

        public string ValueFormat { get; set; }

        public string Placeholder { get; set; }

        public string StartPlaceholder { get; set; }

        public string EndPlaceholder { get; set; }

        public string RangeSeparator { get; set; } = "-";

        public bool IsDisabled { get; set; }

        public bool Readonly { get; set; }

        public bool Editable { get; set; } = true;

        public bool Clearable { get; set; } = true;

        public InputSize Size { get; set; } = InputSize.Normal;

        public string PrefixIcon { get; set; } = "el-icon-time";

        public string ClearIcon { get; set; } = "el-icon-circle-close";

        public bool ValidateEvent { get; set; } = true;
    }
}
