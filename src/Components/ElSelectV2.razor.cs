using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElSelectV2<TValue> : ElementFieldComponentBase<TValue>
    {
        private readonly string dropdownId = $"el-select-v2-dropdown-{Guid.NewGuid():N}";
        private readonly List<SelectV2Option> selectedOptions = new List<SelectV2Option>();
        private HtmlPropertyBuilder wrapperClsBuilder;
        private bool effectiveDisabled;
        private InputSize effectiveSize = InputSize.Normal;
        private bool dropdownVisible;
        private string filterText;
        private string activeOptionId;

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
        public bool Multiple { get; set; }

        [Parameter]
        public ICollection<TValue> Values { get; set; }

        [Parameter]
        public EventCallback<ICollection<TValue>> ValuesChanged { get; set; }

        [Parameter]
        public bool CollapseTags { get; set; }

        [Parameter]
        public int MaxCollapseTags { get; set; } = 1;

        [Parameter]
        public bool Remote { get; set; }

        [Parameter]
        public EventCallback<string> RemoteMethod { get; set; }

        [Parameter]
        public EventCallback<string> OnFilter { get; set; }

        [Parameter]
        public bool Loading { get; set; }

        [Parameter]
        public string LoadingText { get; set; } = "加载中";

        [Parameter]
        public string NoDataText { get; set; } = "无数据";

        [Parameter]
        public string NoMatchText { get; set; } = "无匹配数据";

        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        [Parameter]
        public int ItemHeight { get; set; } = 34;

        [Parameter]
        public int Height { get; set; } = 274;

        [Parameter]
        public int OverscanCount { get; set; } = 3;

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
                .AddIf(Multiple, "is-multiple")
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

            SyncSelectedOptions();
            EnsureActiveOption();
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = value == null ? default : (TValue)TypeHelper.ChangeType(value, typeof(TValue));
            filterText = null;
            dropdownVisible = false;
            activeOptionId = null;
            SyncSelectedOptions();
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
            EnsureActiveOption();
        }

        private async Task OnFilterInputAsync(string value)
        {
            filterText = value;
            dropdownVisible = true;
            EnsureActiveOption();
            if (Remote && RemoteMethod.HasDelegate)
            {
                await RemoteMethod.InvokeAsync(value);
            }
            if (OnFilter.HasDelegate)
            {
                await OnFilter.InvokeAsync(value);
            }
        }

        private void OpenDropdownFromKeyboard(KeyboardEventArgs e)
        {
            if (!effectiveDisabled)
            {
                dropdownVisible = true;
                EnsureActiveOption();
            }
        }

        private async Task OnRootKeyDownAsync(KeyboardEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            switch (e.Key)
            {
                case "ArrowDown":
                    dropdownVisible = true;
                    MoveActiveOption(1);
                    break;
                case "ArrowUp":
                    dropdownVisible = true;
                    MoveActiveOption(-1);
                    break;
                case "Home":
                    dropdownVisible = true;
                    activeOptionId = SelectableRows.FirstOrDefault()?.Id;
                    break;
                case "End":
                    dropdownVisible = true;
                    activeOptionId = SelectableRows.LastOrDefault()?.Id;
                    break;
                case "Enter":
                    if (!dropdownVisible)
                    {
                        dropdownVisible = true;
                        EnsureActiveOption();
                        return;
                    }
                    await SelectActiveOptionAsync();
                    break;
                case "Escape":
                    dropdownVisible = false;
                    break;
                case "Backspace":
                    if (Multiple && SelectedOptions.Any() && string.IsNullOrWhiteSpace(filterText))
                    {
                        await RemoveTagAsync(SelectedOptions.Last());
                    }
                    break;
            }
        }

        private async Task SelectActiveOptionAsync()
        {
            var option = SelectableRows.FirstOrDefault(x => x.Id == activeOptionId)?.Option;
            if (option != null)
            {
                await SelectOptionAsync(option);
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

            if (Multiple)
            {
                await ToggleMultipleOptionAsync(option);
            }
            else
            {
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
            }

            if (OnSelectedOptionChange.HasDelegate)
            {
                await OnSelectedOptionChange.InvokeAsync(args);
            }

            EnsureActiveOption();
        }

        private async Task ToggleMultipleOptionAsync(SelectV2Option option)
        {
            var existing = selectedOptions.FirstOrDefault(x => TypeHelper.Equal(ConvertOptionValue(x.Value), ConvertOptionValue(option.Value)));
            if (existing == null)
            {
                selectedOptions.Add(option);
            }
            else
            {
                selectedOptions.Remove(existing);
            }

            var values = selectedOptions.Select(x => ConvertOptionValue(x.Value)).ToList();
            Values = values;
            Value = values.LastOrDefault();
            SetFieldValue(Value, ValidateEvent);
            if (ValuesChanged.HasDelegate)
            {
                await ValuesChanged.InvokeAsync(Values);
            }
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
        }

        private async Task RemoveTagAsync(SelectV2Option option)
        {
            if (option == null || effectiveDisabled)
            {
                return;
            }

            selectedOptions.RemoveAll(x => TypeHelper.Equal(ConvertOptionValue(x.Value), ConvertOptionValue(option.Value)));
            var values = selectedOptions.Select(x => ConvertOptionValue(x.Value)).ToList();
            Values = values;
            Value = values.LastOrDefault();
            SetFieldValue(Value, ValidateEvent);
            if (ValuesChanged.HasDelegate)
            {
                await ValuesChanged.InvokeAsync(Values);
            }
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
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
            activeOptionId = null;
            selectedOptions.Clear();
            if (Multiple)
            {
                Values = Array.Empty<TValue>();
                if (ValuesChanged.HasDelegate)
                {
                    await ValuesChanged.InvokeAsync(Values);
                }
            }
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

        private void SyncSelectedOptions()
        {
            selectedOptions.Clear();
            if (Multiple)
            {
                var values = Values ?? ExtractValues(Value);
                foreach (var value in values)
                {
                    var option = FlatOptions.FirstOrDefault(x => TypeHelper.Equal(ConvertOptionValue(x.Value), value));
                    if (option != null)
                    {
                        selectedOptions.Add(option);
                    }
                }
                return;
            }

            var selected = FlatOptions.FirstOrDefault(x => TypeHelper.Equal(ConvertOptionValue(x.Value), Value));
            if (selected != null)
            {
                selectedOptions.Add(selected);
            }
        }

        private IEnumerable<TValue> ExtractValues(object value)
        {
            if (value is IEnumerable<TValue> typed && value is not string)
            {
                return typed;
            }
            return Enumerable.Empty<TValue>();
        }

        private void EnsureActiveOption()
        {
            if (!SelectableRows.Any())
            {
                activeOptionId = null;
                return;
            }

            if (string.IsNullOrWhiteSpace(activeOptionId) || SelectableRows.All(x => x.Id != activeOptionId))
            {
                var selected = SelectableRows.FirstOrDefault(x => IsSelected(x.Option));
                activeOptionId = (selected ?? SelectableRows.First()).Id;
            }
        }

        private void MoveActiveOption(int step)
        {
            var rows = SelectableRows.ToList();
            if (!rows.Any())
            {
                activeOptionId = null;
                return;
            }

            var index = rows.FindIndex(x => x.Id == activeOptionId);
            if (index < 0)
            {
                index = 0;
            }
            else
            {
                index = (index + step + rows.Count) % rows.Count;
            }
            activeOptionId = rows[index].Id;
        }

        private bool IsSelected(SelectV2Option option)
        {
            if (option == null)
            {
                return false;
            }
            return Multiple
                ? selectedOptions.Any(x => TypeHelper.Equal(ConvertOptionValue(x.Value), ConvertOptionValue(option.Value)))
                : TypeHelper.Equal(ConvertOptionValue(option.Value), Value);
        }

        private IEnumerable<SelectV2Option> FilteredOptions
        {
            get
            {
                var options = Options ?? Enumerable.Empty<SelectV2Option>();
                if (!Filterable || string.IsNullOrWhiteSpace(filterText))
                {
                    return options.Where(x => x != null).ToList();
                }
                return options.Where(x => x != null && (x.Label ?? string.Empty).IndexOf(filterText, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();
            }
        }

        private List<SelectV2Row> VirtualRows
        {
            get
            {
                var rows = new List<SelectV2Row>();
                string currentGroup = null;
                foreach (var option in FilteredOptions)
                {
                    if (!string.IsNullOrWhiteSpace(option.Group) && !string.Equals(currentGroup, option.Group, StringComparison.Ordinal))
                    {
                        currentGroup = option.Group;
                        rows.Add(new SelectV2Row
                        {
                            Id = $"{dropdownId}-group-{rows.Count}",
                            Group = option.Group,
                            IsGroup = true
                        });
                    }

                    rows.Add(new SelectV2Row
                    {
                        Id = GetOptionId(option),
                        Option = option,
                        Group = option.Group,
                        IsGroup = false
                    });
                }
                return rows;
            }
        }

        private IEnumerable<SelectV2Row> SelectableRows => VirtualRows.Where(x => !x.IsGroup && x.Option != null && !x.Option.Disabled);

        private IEnumerable<SelectV2Option> FlatOptions => (Options ?? Enumerable.Empty<SelectV2Option>()).Where(x => x != null);

        private SelectV2Option SelectedOption => FlatOptions.FirstOrDefault(IsSelected);

        private string DisplayLabel => Multiple
            ? (Filterable && dropdownVisible ? filterText : string.Empty)
            : (Filterable && dropdownVisible ? filterText : SelectedOption?.Label);

        private string EffectivePlaceholder => Multiple && SelectedOptions.Any()
            ? string.Empty
            : (string.IsNullOrEmpty(DisplayLabel) ? Placeholder : null);

        private bool ShowClear => Clearable && !effectiveDisabled && (Multiple ? SelectedOptions.Any() : SelectedOption != null);

        private bool IsSelectV2Disabled => effectiveDisabled;

        private bool IsLoading => Loading;

        private string EmptyText => Filterable && !Remote && !string.IsNullOrWhiteSpace(filterText) && !FilteredOptions.Any()
            ? NoMatchText
            : NoDataText;

        private int EffectiveItemHeight => Math.Max(24, ItemHeight);

        private int EffectiveHeight => Math.Max(EffectiveItemHeight * 2, Height);

        private IReadOnlyList<SelectV2Option> SelectedOptions => selectedOptions;

        private IEnumerable<SelectV2Option> VisibleSelectedOptions => CollapseTags
            ? selectedOptions.Take(Math.Max(1, MaxCollapseTags))
            : selectedOptions;

        private int CollapsedTagCount => CollapseTags
            ? Math.Max(0, selectedOptions.Count - Math.Max(1, MaxCollapseTags))
            : 0;

        private string ActiveDescendantId => dropdownVisible ? activeOptionId : null;

        private string GetOptionId(SelectV2Option option)
        {
            var value = option?.Value == null ? "null" : Convert.ToString(option.Value, CultureInfo.InvariantCulture)?.Replace(' ', '-');
            return $"{dropdownId}-option-{value}";
        }

        private static string GetSizeCssValue(InputSize size) => size switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };

        private sealed class SelectV2Row
        {
            public string Id { get; set; }

            public bool IsGroup { get; set; }

            public string Group { get; set; }

            public SelectV2Option Option { get; set; }
        }
    }
}
