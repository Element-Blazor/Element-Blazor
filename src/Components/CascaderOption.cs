using System.Collections.Generic;
using System.Linq;

namespace Element
{
    public class CascaderOption
    {
        public string Value { get; set; }

        public string Label { get; set; }

        public bool Disabled { get; set; }

        public bool Leaf { get; set; }

        public bool Loading { get; set; }

        public object Data { get; set; }

        public IList<CascaderOption> Children { get; set; } = new List<CascaderOption>();

        internal bool Loaded { get; set; }

        internal bool HasChildren => Children != null && Children.Any();

        internal string DisplayLabel => string.IsNullOrWhiteSpace(Label) ? Value : Label;
    }
}
