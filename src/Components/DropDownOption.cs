

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class DropDownOption : PopupOption
    {
        public Action Refresh { get; set; }
        public object Select { get; set; }
        public RenderFragment OptionContent { get; set; }
        public bool IsTree { get; set; }
        public string PopperClass { get; set; }
        public int MaxHeight { get; set; }
        public bool FitInputWidth { get; set; } = true;
        internal string Placement { get; set; } = "bottom-start";
        internal string TransformOrigin { get; set; } = "center top 0px";
        internal float Width { get; set; }
    }
}
