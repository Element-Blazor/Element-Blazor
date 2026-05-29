using Microsoft.AspNetCore.Components;

namespace Element
{
    public class TourStep
    {
        public string Target { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public RenderFragment Content { get; set; }

        public string Placement { get; set; } = "bottom";
    }
}
