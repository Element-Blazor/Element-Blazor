namespace Element.ControlConfigs
{
    public class SliderAttribute : BaseAttribute
    {
        public double Min { get; set; }

        public double Max { get; set; } = 100;

        public double Step { get; set; } = 1;

        public bool IsDisabled { get; set; }

        public bool ShowStops { get; set; }

        public bool ShowInput { get; set; }

        public SliderMark[] Marks { get; set; }
    }
}
