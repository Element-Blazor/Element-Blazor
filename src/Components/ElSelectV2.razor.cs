using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElSelectV2<TValue> : ElementFieldComponentBase<TValue>
    {
        private HtmlPropertyBuilder wrapperClsBuilder;
        private bool effectiveDisabled;
        private InputSize effectiveSize = InputSize.Normal;
        private bool dropdownVisible;
        private string filterText;

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public TValue ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<TValue> ModelValueChanged { get; set; }

        [Parameter]
        public IEnumerable<SelectV2Option> Options { get; set; } = Enumerable.Empty<SelectV2Option>();

        [Parameter]
        public string Placeholder { get; set; } = "请选择";

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        [Parameter]
        public bool Clearable { get; set; } = true;

        [Parameter]
        public bool IsClearable
        {
            get => Clearable;
            set => Clearable = value;
        }

        [Parameter]
        public bool Filterable { get; set; }

        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        [Parameter]
        public int ItemHeight { get; set; } = 34;

        [Parameter]
        public int Height { get; set; } = 274;

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public EventCallback<TValue> OnChange { get; set; }

        [Parameter]
        public EventCallback<ElementChangeEventArgs<SelectV2Option>> OnChanging { get; set; }

        [Parameter]
        public EventCallback<ElementChangeEventArgs<SelectV2Option>> OnSelectedOptionChange { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            effectiveDisabled = Disabled || (FormItem?.Form?.Disabled ?? false);
            effectiveSize = Size == InputSize.Normal
                ? FormItem?.Size ?? FormItem?.Form?.EffectiveSize ?? ResolveInputSize(InputSize.Normal)
                : Size;
            var sizeCssValue = GetSizeCssValue(effectiveSize);
            wrapperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-select", "el-select-v2", Cls)
                .AddIf(effectiveDisabled, "is-disabled")
                .AddIf(Filterable, "is-filterable")
                .AddIf(Clearable, "is-clearable")
                .AddIf(dropdownVisible, "is-focus")
                .AddIf(sizeCssValue != null, $"el-select--{sizeCssValue}");

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    Value = FormItem.OriginValue == null
                        ? default
                        : (TValue)TypeHelper.ChangeType(FormItem.OriginValue, typeof(TValue));
                }
                SetFieldValue(Value, false);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = value == null ? default : (TValue)TypeHelper.ChangeType(value, typeof(TValue));
            filterText = null;
            dropdownVisible = false;
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                _ = ModelValueChanged.InvokeAsync(Value);
            }
            else
            {
                StateHasChanged();
            }
        }

        private void OpenDropdown()
        {
            if (effectiveDisabled)
            {
                return;
            }
            dropdownVisible = true;
        }

        private Task OnFilterInputAsync(string value)
        {
            filterText = value;
            dropdownVisible = true;
            return Task.CompletedTask;
        }

        private void OpenDropdownFromKeyboard(KeyboardEventArgs e)
        {
            if (!effectiveDisabled)
            {
                dropdownVisible = true;
            }
        }

        private async Task SelectOptionAsync(SelectV2Option option)
        {
            if (option == null || option.Disabled || effectiveDisabled)
            {
                return;
            }

            var args = new ElementChangeEventArgs<SelectV2Option>
            {
                OldValue = SelectedOption,
                NewValue = option
            };
            if (OnChanging.HasDelegate)
            {
                await OnChanging.InvokeAsync(args);
                if (args.DisallowChange)
                {
                    return;
                }
            }

            Value = ConvertOptionValue(option.Value);
            filterText = null;
            dropdownVisible = false;
            SetFieldValue(Value, ValidateEvent);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
            if (OnSelectedOptionChange.HasDelegate)
            {
                await OnSelectedOptionChange.InvokeAsync(args);
            }
        }

        private async Task ClearAsync(MouseEventArgs e)
        {
            if (!Clearable || effectiveDisabled)
            {
                return;
            }
            Value = default;
            filterText = null;
            dropdownVisible = false;
            SetFieldValue(Value, ValidateEvent);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
        }

        private TValue ConvertOptionValue(object value)
        {
            if (value == null)
            {
                return default;
            }
            if (value is TValue typedValue)
            {
                return typedValue;
            }
            return (TValue)TypeHelper.ChangeType(value, typeof(TValue));
        }

        private SelectV2Option SelectedOption => (Options ?? Enumerable.Empty<SelectV2Option>())
            .FirstOrDefault(x => TypeHelper.Equal(ConvertOptionValue(x.Value), Value));

        private string DisplayLabel => Filterable && dropdownVisible
            ? filterText
            : SelectedOption?.Label;

        private string EffectivePlaceholder => string.IsNullOrEmpty(DisplayLabel) ? Placeholder : null;

        private bool ShowClear => Clearable && !effectiveDisabled && SelectedOption != null;

        private ICollection<SelectV2Option> FilteredOptions
        {
            get
            {
                var options = Options ?? Enumerable.Empty<SelectV2Option>();
                if (!Filterable || string.IsNullOrWhiteSpace(filterText))
                {
                    return options.ToList();
                }
                return options.Where(x => (x.Label ?? string.Empty).IndexOf(filterText, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();
            }
        }

        private bool IsSelectV2Disabled => effectiveDisabled;

        private static string GetSizeCssValue(InputSize size) => size switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };
    }
}
