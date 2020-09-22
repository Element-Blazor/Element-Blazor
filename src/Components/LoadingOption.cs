using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class LoadingOption
    {
        public ElementReference Target { get; set; }
        public string Text { get; set; }
        public string Background { get; set; }
        public string IconClass { get; set; }
        internal int ZIndex { get; set; }
        internal bool IsNew { get; set; }
        internal ElementReference Element { get; set; }
    }
}
