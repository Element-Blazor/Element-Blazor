using Blazui.Component.EventArgs;
using Blazui.Component.Popup;
using Blazui.Component.Select;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Select
{
    public class BSelectOptionBase<TValue> : ComponentBase
    {
        [CascadingParameter]
        public DropDownOption Option { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        protected override void OnInitialized()
        {
            ((BSelect<TValue>)Option.Select).Options.Add(this);
        }

        public async Task SelectItemAsync(MouseEventArgs e)
        {
            if (IsDisabled)
            {
                return;
            }
            await ((BSelect<TValue>)Option.Select).OnInternalSelectAsync(this);
        }
        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
