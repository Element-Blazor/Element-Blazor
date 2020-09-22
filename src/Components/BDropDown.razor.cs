using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class BDropDown
    {
        internal ElementReference Target;

        [Parameter]
        public RenderFragment Trigger { get; set; }

        [Parameter]
        public RenderFragment Items { get; set; }

        [Inject]
        private PopupService PopupService { get; set; }
        internal void ShowDropDown()
        {
            if (PopupService.DropDownMenuOptions.Any(x => x.Target.Id == Target.Id))
            {
                return;
            }
            PopupService.DropDownMenuOptions.Add(new DropDownOption()
            {
                Target = Target,
                OptionContent = Items,
                Select = this
            });
        }
    }
}
