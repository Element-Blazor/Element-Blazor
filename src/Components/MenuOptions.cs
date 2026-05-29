using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class MenuOptions
    {
        public string TextColor { get; set; }

        public string ActiveTextColor { get; set; }

        public string BorderColor { get; set; }

        public string BackgroundColor { get; set; }

        public string HoverColor { get; set; }

        public string DefaultActiveIndex { get; set; }

        public MenuMode Mode { get; set; }

        public bool Disabled { get; set; }

        public bool Collapse { get; set; }

        public bool Router { get; set; } = true;

        public MenuTrigger MenuTrigger { get; set; } = MenuTrigger.Hover;

        public string PopperClass { get; set; }

        public string PopperStyle { get; set; }

        public string PopperEffect { get; set; } = "dark";

        public MenuTheme Theme { get; set; } = MenuTheme.Light;
    }
}
