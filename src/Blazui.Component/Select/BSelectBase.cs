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
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blazui.Component.Select
{
    public class BSelectBase<TValue> : BFieldComponentBase<TValue>, IDisposable
    {

        internal ElementReference elementSelect;
        private Type valueType;
        private Type nullable;
        internal bool isClearable = true;
        internal bool IsClearButtonClick { get; set; }

        internal string Label { get; set; }
        internal ObservableCollection<BSelectOptionBase<TValue>> Options { get; set; } = new ObservableCollection<BSelectOptionBase<TValue>>();

        [Parameter]
        public TValue InitialValue { get; set; }
        [Parameter]
        public string Placeholder { get; set; } = "请选择";
        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        /// <summary>
        /// 当绑定为枚举时，指定哪些枚举名需要忽略
        /// </summary>
        [Parameter]
        public string[] IgnoreEnumNames { get; set; } = new string[0];

        protected override void OnParametersSet()
        {
            if (valueType != null)
            {
                return;
            }
            valueType = typeof(TValue);
            nullable = Nullable.GetUnderlyingType(valueType);
            valueType = nullable ?? valueType;
            if (valueType.IsEnum)
            {
                var names = Enum.GetNames(valueType);
                var values = Enum.GetValues(valueType);
                var valueInitilized = false;
                dict = new Dictionary<TValue, string>();
                for (int i = 0; i < names.Length; i++)
                {
                    var name = names[i];
                    if (IgnoreEnumNames.Contains(name, StringComparer.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                    var value = values.GetValue(i);
                    var field = valueType.GetField(name);
                    var text = field.GetCustomAttributes(typeof(DescriptionAttribute), true)
                        .Cast<DescriptionAttribute>()
                        .FirstOrDefault()?.Description ?? name;
                    if (!valueInitilized)
                    {
                        valueInitilized = true;
                        if (nullable == null)
                        {
                            Value = (TValue)value;
                            InitialValue = Value;
                            SetFieldValue(Value, false);
                            Label = text;
                            isClearable = false;
                        }
                    }
                    dict.Add((TValue)value, text);
                }
                ChildContent = builder =>
                {
                    int seq = 0;
                    foreach (var label in dict.Keys)
                    {
                        builder.OpenComponent<BSelectOption<TValue>>(seq++);
                        builder.AddAttribute(seq++, "Text", label);
                        builder.AddAttribute(seq++, "Value", dict[label]);
                        builder.CloseComponent();
                    }
                };
            }
            base.OnParametersSet();
        }

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

        /// <summary>
        /// 当前选中值
        /// </summary>
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
        private Dictionary<TValue, string> dict;

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
            SetFieldValue(item.Value, true);
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
            var enumValue = (TValue)value;
            if (nullable != null && value == null)
            {
                SelectedOption = null;
            }
            else
            {
                Label = dict[enumValue];
                Value = enumValue;
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
