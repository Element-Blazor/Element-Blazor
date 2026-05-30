using Element;
using Microsoft.AspNetCore.Components;

namespace Element.X
{
    public class XThoughtItem
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Duration { get; set; }

        public string Icon { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public bool Expanded { get; set; }

        public RenderFragment Content { get; set; }
    }
}
