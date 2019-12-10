using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.DropDown
{
    public class BDropDownBase : BComponentBase
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
            PopupService.DropDownMenuOptions.Add(new DropDownOption()
            {
                Target = Target,
                OptionContent = Items,
                Select = this
            });
        }
    }
}
