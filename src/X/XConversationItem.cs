using Element;
using Microsoft.AspNetCore.Components;

namespace Element.X
{
    public class XConversationItem
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Group { get; set; }

        public string Icon { get; set; }

        public bool Pinned { get; set; }

        public bool Disabled { get; set; }

        public RenderFragment Extra { get; set; }
    }
}
