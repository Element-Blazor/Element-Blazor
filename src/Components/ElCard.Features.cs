using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElCard
    {
        [Parameter]
        public string BodyStyle { get; set; }

        [Parameter]
        public RenderFragment ChildContent
        {
            get => Body;
            set => Body = value;
        }

        [Parameter]
        public RenderFragment Footer { get; set; }
    }
}
