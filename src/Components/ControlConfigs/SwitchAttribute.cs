namespace Element.ControlConfigs
{
    public class SwitchAttribute : BaseAttribute
    {
        public bool IsDisabled { get; set; }

        public string ActiveText { get; set; }

        public string InactiveText { get; set; }

        public string ActiveColor { get; set; } = "#409EFF";

        public string InactiveColor { get; set; } = "#C0CCDA";
    }
}
