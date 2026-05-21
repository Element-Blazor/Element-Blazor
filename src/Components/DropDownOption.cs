

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
        public Func<Task> OnClosed { get; set; }
        public object Select { get; set; }
        public RenderFragment OptionContent { get; set; }
        public bool IsTree { get; set; }
        public string PopperClass { get; set; }
        public string PopperStyle { get; set; }
        public int MaxHeight { get; set; }
        public bool FitInputWidth { get; set; }
        internal string DropDownId { get; set; }
        internal string Placement { get; set; } = "bottom-start";
        internal string TransformOrigin { get; set; } = "center top 0px";
        internal float MinWidth { get; set; }
        internal float Width { get; set; }
        internal ElementReference ScrollElement { get; set; }
    }
}
