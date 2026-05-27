using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElOptionGroup<TValue>
    {
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
