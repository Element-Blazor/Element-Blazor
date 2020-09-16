using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public partial class BDropDownItem 
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [CascadingParameter]
        public DropDownOption Option { get; set; }

        [Parameter]
        public EventCallback<DropDownOption> OnClick { get; set; }

        internal void InternalOnClick()
        {
            _ = Option.Instance.CloseDropDownAsync(Option);
            if (OnClick.HasDelegate)
            {
                OnClick.InvokeAsync(Option);
            }
        }
    }
}
