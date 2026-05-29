using Microsoft.AspNetCore.Components;

namespace Element
{
    public class SegmentedOption
    {
        public string Label { get; set; }

        public string Value { get; set; }

        public bool Disabled { get; set; }

        public RenderFragment Content { get; set; }
    }
}
