namespace Element.ControlConfigs
{
    public class SwitchAttribute : BaseAttribute
    {
        public bool IsDisabled { get; set; }

        public string ActiveText { get; set; }

        public string InactiveText { get; set; }

        public string ActiveValue { get; set; }

        public string InactiveValue { get; set; }

        public string ActiveColor { get; set; } = "#409EFF";

        public string InactiveColor { get; set; } = "#C0CCDA";

        public bool Loading { get; set; }

        public string LoadingIcon { get; set; } = "el-icon-loading";
    }
}
