using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Element
{
    public class XPromptItem
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public string Tag { get; set; }

        public bool Disabled { get; set; }

        public IEnumerable<XPromptItem> Children { get; set; }

        public RenderFragment Extra { get; set; }
    }
}
