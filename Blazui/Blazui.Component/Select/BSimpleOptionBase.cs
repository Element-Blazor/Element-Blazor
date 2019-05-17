using Blazui.Component.EventArgs;
using Blazui.Component.Select;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Select
{
    public class BSimpleOptionBase : ComponentBase
    {
        [CascadingParameter]
        public BSimpleSelect Select { get; set; }

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public string Text { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                Value = Text;
            }
            if (Select.Value == Value && Select.CurrentSelectedItem != this)
            {
                Select.CurrentSelectedItem = this;
                Select.Refresh();
            }
            await Task.CompletedTask;
        }

        public async Task SelectItemAsync()
        {
            await Select.OnInternalSelectAsync(this);
        }
    }
}
