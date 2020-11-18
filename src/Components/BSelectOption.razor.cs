


using Element.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class BSelectOption<TValue> 
    {
        private SelectResultModel<TValue> currentResultModel;

        [CascadingParameter]
        public DropDownOption Option { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        protected override void OnInitialized()
        {
            currentResultModel = new SelectResultModel<TValue>()
            {
                Key = Value,
                Text = Text
            };
            ((BSelect<TValue>)Option.Select).Options.Add(currentResultModel);
        }

        public async Task SelectItemAsync(MouseEventArgs e)
        {
            if (IsDisabled)
            {
                return;
            }
            await ((BSelect<TValue>)Option.Select).OnInternalSelectAsync(currentResultModel);
        }
        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
