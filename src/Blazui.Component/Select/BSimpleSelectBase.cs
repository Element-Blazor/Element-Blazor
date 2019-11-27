using Blazui.Component.Dom;
using Blazui.Component.EventArgs;
using Blazui.Component.Form;
using Blazui.Component.Input;
using Blazui.Component.Popup;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blazui.Component.Select
{
    public class BSimpleSelectBase<TValue> : BFieldComponentBase<TValue>
    {

        protected ElementReference elementSelect;

        internal bool IsClearButtonClick { get; set; }

        internal List<BSimpleOptionBase<TValue>> Options { get; set; } = new List<BSimpleOptionBase<TValue>>();

        [Parameter]
        public string Placeholder { get; set; } = "请选择";
        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        internal void UpdateValue(string text)
        {
            var option = Options.FirstOrDefault(x => x.Text == text);
            if (option == null)
            {
                Value = default;
            }
            else
            {
                Value = option.Value;
            }
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            if (TypeHelper.Equal(Value, default))
            {
                SelectedOption = null;
            }
        }
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Inject]
        internal PopupService PopupService { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<BChangeEventArgs<BSimpleOptionBase<TValue>>> OnSelect { get; set; }

        [Parameter]
        public EventCallback<BChangeEventArgs<BSimpleOptionBase<TValue>>> OnSelecting { get; set; }

        internal BSimpleOptionBase<TValue> SelectedOption
        {
            get
            {
                return selectedOption;
            }
            set
            {
                if (value == null)
                {
                    Value = default;
                }
                else
                {
                    Value = value.Value;
                }
                selectedOption = value;
                if (ValueChanged.HasDelegate)
                {
                    _ = ValueChanged.InvokeAsync(Value);
                }

            }
        }

        private BSimpleOptionBase<TValue> selectedOption;
        protected DropDownOption DropDownOption;

        internal void Refresh()
        {
            StateHasChanged();
        }

        internal async Task OnInternalSelectAsync(BSimpleOptionBase<TValue> item)
        {
            var args = new BChangeEventArgs<BSimpleOptionBase<TValue>>();
            args.NewValue = item;
            args.OldValue = SelectedOption;
            if (OnSelecting.HasDelegate)
            {
                await OnSelecting.InvokeAsync(args);
                if (args.DisallowChange)
                {
                    return;
                }
            }

            await DropDownOption.Instance.CloseDropDownAsync(DropDownOption);
            SelectedOption = item;
            SetFieldValue(item.Value);
            if (OnSelect.HasDelegate)
            {
                await OnSelect.InvokeAsync(args);
            }
            IsClearButtonClick = false;
            StateHasChanged();
        }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected void OnSelectClick(MouseEventArgs e)
        {
            if (IsClearButtonClick)
            {
                IsClearButtonClick = false;
                return;
            }
            if (PopupService.DropDownOptions.Any(x => x.Target.Id == elementSelect.Id))
            {
                return;
            }

            DropDownOption = new DropDownOption()
            {
                Select = this,
                Target = elementSelect,
                OptionContent = ChildContent,
                Refresh = () =>
                {
                    StateHasChanged();
                },
                IsShow = true
            };
            PopupService.DropDownOptions.Add(DropDownOption);
        }

        protected override void FormItem_OnReset()
        {
            selectedOption = null;
        }
    }
}
