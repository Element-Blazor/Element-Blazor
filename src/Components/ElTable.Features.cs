using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Element
{
    public partial class ElTable
    {
        [Parameter]
        public RenderFragment<object> ExpandContent { get; set; }

        [Parameter]
        public HashSet<object> ExpandedRows { get; set; } = new HashSet<object>();

        [Parameter]
        public bool ShowSummary { get; set; }

        [Parameter]
        public string SumText { get; set; } = "合计";

        [Parameter]
        public bool Virtualized { get; set; }

        [Parameter]
        public int VirtualStartIndex { get; set; }

        [Parameter]
        public int VirtualItemCount { get; set; } = 50;
    }
}
