using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
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
    }
}
