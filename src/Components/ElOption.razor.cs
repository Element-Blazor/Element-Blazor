


using Element.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElOption<TValue> 
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
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        protected override void OnInitialized()
        {
            currentResultModel = new SelectResultModel<TValue>()
            {
                Key = Value,
                Text = Text ?? Convert.ToString(Value),
                Disabled = Disabled
            };
            ((ElSelect<TValue>)Option.Select).RegisterOption(currentResultModel);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (currentResultModel != null)
            {
                currentResultModel.Key = Value;
                currentResultModel.Text = Text ?? Convert.ToString(Value);
                currentResultModel.Disabled = Disabled;
            }
        }

        public async Task SelectItemAsync(MouseEventArgs e)
        {
            if (Disabled)
            {
                return;
            }
            await ((ElSelect<TValue>)Option.Select).OnInternalSelectAsync(currentResultModel);
        }
        protected override bool ShouldRender()
        {
            return true;
        }

        private bool IsSelected => Option?.Select is ElSelect<TValue> select && select.IsOptionSelected(currentResultModel);

        private bool IsHover => Option?.Select is ElSelect<TValue> select && select.IsOptionHover(currentResultModel);

        private bool IsVisible => Option?.Select is not ElSelect<TValue> select || select.IsOptionVisible(currentResultModel);
    }
}
