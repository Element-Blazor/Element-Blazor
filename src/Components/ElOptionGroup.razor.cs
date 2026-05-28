using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElOptionGroup<TValue>
    {
        [CascadingParameter]
        public DropDownOption Option { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        internal bool EffectiveDisabled => Disabled;
    }
}
