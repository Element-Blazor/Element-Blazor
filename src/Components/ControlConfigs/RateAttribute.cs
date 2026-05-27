namespace Element.ControlConfigs
{
    public class RateAttribute : BaseAttribute
    {
        public int Max { get; set; } = 5;

        public bool IsDisabled { get; set; }

        public bool AllowHalf { get; set; }

        public bool Clearable { get; set; } = true;

        public bool ShowText { get; set; }

        public bool ShowScore { get; set; }

        public string ScoreTemplate { get; set; } = "{0}";

        public string[] Texts { get; set; }

        public string[] Colors { get; set; }

        public string VoidColor { get; set; }

        public string DisabledVoidColor { get; set; }

        public string Icon { get; set; }

        public string VoidIcon { get; set; }
    }
}
