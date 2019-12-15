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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blazui.Component.Select
{
    public class BSelectBase<TValue> : BFieldComponentBase<OptionModel<TValue>>, IDisposable
    {

        protected ElementReference elementSelect;

        internal bool IsClearButtonClick { get; set; }

        internal string Label { get; set; }
        internal ObservableCollection<BSelectOptionBase<TValue>> Options { get; set; } = new ObservableCollection<BSelectOptionBase<TValue>>();

        [Parameter]
        public TValue InitialValue { get; set; }
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
        public EventCallback<BChangeEventArgs<BSelectOptionBase<TValue>>> OnChange { get; set; }

        [Parameter]
        public EventCallback<BChangeEventArgs<BSelectOptionBase<TValue>>> OnChanging { get; set; }

        internal BSelectOptionBase<TValue> SelectedOption
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
                    Label = string.Empty;
                }
                else
                {
                    Value = value.Value;
                    Label = value.Text;
                }
                selectedOption = value;
                if (ValueChanged.HasDelegate)
                {
                    _ = ValueChanged.InvokeAsync(Value);
                }

            }
        }

        private BSelectOptionBase<TValue> selectedOption;
        protected DropDownOption DropDownOption;
        
        internal async Task OnInternalSelectAsync(BSelectOptionBase<TValue> item)
        {
            var args = new BChangeEventArgs<BSelectOptionBase<TValue>>();
            args.NewValue = item;
            args.OldValue = SelectedOption;
            if (OnChanging.HasDelegate)
            {
                await OnChanging.InvokeAsync(args);
                if (args.DisallowChange)
                {
                    return;
                }
            }

            await DropDownOption.Instance.CloseDropDownAsync(DropDownOption);
            SelectedOption = item;
            SetFieldValue(new OptionModel<TValue>(item.Text, item.Value), true);
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(args);
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
            if (PopupService.SelectDropDownOptions.Any(x => x.Target.Id == elementSelect.Id))
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
            PopupService.SelectDropDownOptions.Add(DropDownOption);
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            var optionModel = (OptionModel<TValue>)value;
            if (optionModel == null)
            {
                SelectedOption = null;
            }
            else
            {
                Label = optionModel.Text;
                Value = optionModel.Value;
            }
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            else
            {
                StateHasChanged();
            }
        }
        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
