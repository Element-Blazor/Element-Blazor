




using Element.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class BSelect<TValue> : BFieldComponentBase<TValue>, ISelectDropDownContext, IDisposable
    {
        protected internal bool isTree;
        private HtmlPropertyBuilder warpperClsBuilder;
        internal ElementReference elementSelect;
        private Type valueType;
        private Type nullable;
        private InputSize effectiveSize = InputSize.Normal;
        private bool effectiveDisabled;
        internal bool EnableClearButton { get; set; }
        private int hoveredIndex = -1;
        private string filterText;
        private static long dropDownIdSeed;
        internal bool OptionsRendered { get; private set; }
        internal string DropDownId { get; } = $"el-select-dropdown-{Interlocked.Increment(ref dropDownIdSeed)}";

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public Action<string> LabelChanged { get; set; }
        internal ObservableCollection<SelectResultModel<TValue>> Options { get; set; } = new ObservableCollection<SelectResultModel<TValue>>();

        [Parameter]
        public TValue InitialValue { get; set; }
        [Parameter]
        public string Placeholder { get; set; } = "请选择";
        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<TValue> ModelValueChanged { get; set; }

        [Parameter]
        public TValue ModelValue
        {
            get => Value;
            set => Value = value;
        }

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
        public bool Loading { get; set; }

        [Parameter]
        public string LoadingText { get; set; } = "加载中";

        [Parameter]
        public string NoDataText { get; set; } = "暂无数据";

        [Parameter]
        public string NoMatchText { get; set; } = "无匹配数据";

        [Parameter]
        public bool Filterable { get; set; }

        [Parameter]
        public bool Multiple { get; set; }

        [Parameter]
        public ICollection<TValue> Values { get; set; }

        [Parameter]
        public EventCallback<ICollection<TValue>> ValuesChanged { get; set; }

        [Parameter]
        public string Separator { get; set; } = ", ";

        [Parameter]
        public bool CollapseTags { get; set; }

        [Parameter]
        public int MaxCollapseTags { get; set; } = 1;

        [Parameter]
        public bool ReserveKeyword { get; set; }

        [Parameter]
        public bool Remote { get; set; }

        [Parameter]
        public EventCallback<string> RemoteMethod { get; set; }

        [Parameter]
        public EventCallback<string> OnFilter { get; set; }

        [Parameter]
        public bool FitInputWidth { get; set; } = true;

        [Parameter]
        public int PopperMaxHeight { get; set; } = 274;

        [Parameter]
        public string PopperClass { get; set; }

        [Parameter]
        public string PopperStyle { get; set; }

        [Parameter]
        public EventCallback<bool> OnVisibleChange { get; set; }

        [Parameter]
        public EventCallback OnEndReached { get; set; }

        [Parameter]
        public EventCallback<FocusEventArgs> OnFocus { get; set; }

        [Parameter]
        public EventCallback<FocusEventArgs> OnBlur { get; set; }

        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        /// <summary>
        /// 当绑定为枚举时，指定哪些枚举名需要忽略
        /// </summary>
        [Parameter]
        public string[] IgnoreEnumNames { get; set; } = new string[0];

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            effectiveDisabled = Disabled || (FormItem?.Form?.Disabled ?? false);
            effectiveSize = Size == InputSize.Normal
                ? FormItem?.Size ?? FormItem?.Form?.Size ?? InputSize.Normal
                : Size;
            var sizeCssValue = GetSizeCssValue(effectiveSize);
            warpperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-select", Cls)
                .AddIf(effectiveDisabled, "is-disabled")
                .AddIf(Loading, "is-loading")
                .AddIf(Filterable, "is-filterable")
                .AddIf(Multiple, "is-multiple")
                .AddIf(Clearable, "is-clearable")
                .AddIf(IsDropDownOpen, "is-focus")
                .AddIf(sizeCssValue != null, $"el-select--{sizeCssValue}");
            if (Multiple)
            {
                SyncSelectedOptionsFromValues();
            }
            if (valueType == null)
            {
                InitilizeEnumValues(FormItem != null);
            }
            if (FormItem == null)
            {
                Label = Label ?? Options.FirstOrDefault(x => TypeHelper.Equal(x.Key, Value))?.Text;
                return;
            }

            if (FormItem.OriginValueHasRendered)
            {
                return;
            }
            FormItem.OriginValueHasRendered = true;
            if (FormItem.Form.Values.Any())
            {
                Value = FormItem.OriginValue;
            }

            if (dict != null && Value != null)
            {
                Label = Label ?? dict[Value];
            }
            SetFieldValue(Value, false);
        }

        private void InitilizeEnumValues(bool firstItemAsValue)
        {
            valueType = typeof(TValue);
            nullable = Nullable.GetUnderlyingType(valueType);
            Clearable = Clearable && nullable != null;
            valueType = nullable ?? valueType;
            var valueSet = false;
            if (valueType.IsEnum)
            {
                var names = Enum.GetNames(valueType);
                var values = Enum.GetValues(valueType);
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
                    if (!valueSet && firstItemAsValue)
                    {
                        valueSet = true;
                        if (nullable == null)
                        {
                            if (!TypeHelper.Equal(Value, (TValue)value))
                            {
                                Value = (TValue)value;
                                InitialValue = Value;
                                SetFieldValue(Value, false);
                            }
                            Label = text;
                        }
                    }
                    dict.Add((TValue)value, text);
                }
                ChildContent = builder =>
                {
                    int seq = 0;
                    foreach (var itemValue in dict.Keys)
                    {
                        builder.OpenComponent<BSelectOption<TValue>>(seq++);
                        builder.AddAttribute(seq++, "Text", dict[itemValue]);
                        builder.AddAttribute(seq++, "Value", itemValue);
                        builder.CloseComponent();
                    }
                };
            }
        }

        public void UpdateValue(string text)
        {
            _ = UpdateValueAsync(text);
        }

        private async Task UpdateValueAsync(string text)
        {
            if (!Filterable)
            {
                if (string.IsNullOrEmpty(text))
                {
                    ClearSelection();
                }
                return;
            }
            filterText = text;
            if (Remote)
            {
                if (RemoteMethod.HasDelegate)
                {
                    await RemoteMethod.InvokeAsync(text);
                }
                if (OnFilter.HasDelegate)
                {
                    await OnFilter.InvokeAsync(text);
                }
                StateHasChanged();
                return;
            }
            if (OnFilter.HasDelegate)
            {
                await OnFilter.InvokeAsync(text);
            }
            var option = Options.FirstOrDefault(x => x.Text == text);
            if (option == null)
            {
                if (string.IsNullOrEmpty(text))
                {
                    ClearSelection();
                }
            }
            else
            {
                _ = SelectValueAsync(option);
            }
            StateHasChanged();
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
        public EventCallback<BChangeEventArgs<SelectResultModel<TValue>>> OnChange { get; set; }

        [Parameter]
        public EventCallback<BChangeEventArgs<SelectResultModel<TValue>>> OnChanging { get; set; }

        internal SelectResultModel<TValue> SelectedOption
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
                    Value = value.Key;
                    Label = value.Text;
                }
                selectedOption = value;
                LabelChanged?.Invoke(Label);
                if (ValueChanged.HasDelegate)
                {
                    _ = ValueChanged.InvokeAsync(Value);
                }
                if (ModelValueChanged.HasDelegate)
                {
                    _ = ModelValueChanged.InvokeAsync(Value);
                }

            }
        }

        private SelectResultModel<TValue> selectedOption;
        internal DropDownOption dropDownOption;
        private Dictionary<TValue, string> dict;

        internal async Task OnInternalSelectAsync(SelectResultModel<TValue> item)
        {
            await SelectValueAsync(item);
        }

        private async Task SelectValueAsync(SelectResultModel<TValue> item)
        {
            var args = new BChangeEventArgs<SelectResultModel<TValue>>();
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

            if (!Multiple && dropDownOption?.Instance != null)
            {
                await dropDownOption.Instance.CloseDropDownAsync(dropDownOption);
            }
            if (Multiple)
            {
                await ToggleMultipleOptionAsync(item);
            }
            else
            {
                SelectedOption = item;
                SetFieldValue(item.Key, true);
                Value = item.Key;
                if (dict != null)
                {
                    dict.TryGetValue(Value, out var label);
                    Label = label;
                }
                if (OnChange.HasDelegate)
                {
                    await OnChange.InvokeAsync(args);
                }
            }
            EnableClearButton = false;
            if (!ReserveKeyword)
            {
                filterText = null;
            }
            StateHasChanged();
        }

        internal async Task OnSelectClick(MouseEventArgs e)
        {
            await ToggleDropDownAsync();
        }

        private async Task ToggleDropDownAsync()
        {
            if (effectiveDisabled)
            {
                return;
            }
            if (EnableClearButton)
            {
                EnableClearButton = false;
                ClearSelection();
                return;
            }
            if (IsDropDownOpen)
            {
                if (dropDownOption?.Instance != null)
                {
                    await dropDownOption.Instance.CloseDropDownAsync(dropDownOption);
                }
                return;
            }

            hoveredIndex = FindSelectedIndex();
            dropDownOption = new DropDownOption()
            {
                IsTree = isTree,
                Select = this,
                Target = elementSelect,
                OptionContent = ChildContent,
                PopperClass = PopperClass,
                PopperStyle = PopperStyle,
                MaxHeight = PopperMaxHeight,
                FitInputWidth = FitInputWidth,
                DropDownId = DropDownId,
                Refresh = () =>
                {
                    StateHasChanged();
                },
                OnClosed = () => NotifyVisibleChangeAsync(false),
                IsShow = true
            };
            PopupService.SelectDropDownOptions.Add(dropDownOption);
            await NotifyVisibleChangeAsync(true);
        }

        private void ClearSelection()
        {
            SelectedOption = null;
            filterText = null;
            if (Multiple)
            {
                selectedOptions.Clear();
                Values = CreateValuesCollection(selectedOptions.Select(x => x.Key));
                if (ValuesChanged.HasDelegate)
                {
                    _ = ValuesChanged.InvokeAsync(Values);
                }
                Label = string.Empty;
            }
            SetFieldValue(Value, true);
        }

        private async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }
            switch (e.Key)
            {
                case "ArrowDown":
                    if (!IsDropDownOpen)
                    {
                        await OpenDropDownAsync();
                        return;
                    }
                    MoveHover(1);
                    break;
                case "ArrowUp":
                    if (!IsDropDownOpen)
                    {
                        await OpenDropDownAsync();
                        return;
                    }
                    MoveHover(-1);
                    break;
                case "Enter":
                    if (!IsDropDownOpen)
                    {
                        await OpenDropDownAsync();
                        return;
                    }
                    await SelectHoveredOptionAsync();
                    break;
                case "Escape":
                    if (dropDownOption?.Instance != null)
                    {
                        await dropDownOption.Instance.CloseDropDownAsync(dropDownOption);
                    }
                    break;
                case "Backspace":
                    if (Multiple && SelectedOptions.Any() && string.IsNullOrWhiteSpace(filterText))
                    {
                        await RemoveTagAsync(SelectedOptions.Last());
                    }
                    else if (Clearable && !Filterable && !TypeHelper.Equal(Value, default))
                    {
                        ClearSelection();
                    }
                    break;
            }
        }

        private async Task OpenDropDownAsync()
        {
            if (IsDropDownOpen)
            {
                return;
            }
            await ToggleDropDownAsync();
        }

        private void MoveHover(int step)
        {
            var enabledOptions = FilteredOptions.Where(x => !x.Disabled).ToList();
            if (!enabledOptions.Any())
            {
                hoveredIndex = -1;
                return;
            }
            var currentOption = hoveredIndex >= 0 && hoveredIndex < Options.Count ? Options[hoveredIndex] : null;
            var currentIndex = currentOption == null ? -1 : enabledOptions.IndexOf(currentOption);
            currentIndex = (currentIndex + step + enabledOptions.Count) % enabledOptions.Count;
            hoveredIndex = Options.IndexOf(enabledOptions[currentIndex]);
            StateHasChanged();
        }

        private async Task SelectHoveredOptionAsync()
        {
            if (hoveredIndex < 0 || hoveredIndex >= Options.Count)
            {
                return;
            }
            if (Loading)
            {
                return;
            }
            var option = Options[hoveredIndex];
            if (option.Disabled)
            {
                return;
            }
            await SelectValueAsync(option);
        }

        internal bool IsOptionSelected(SelectResultModel<TValue> option)
        {
            if (option == null)
            {
                return false;
            }
            return Multiple
                ? selectedOptions.Any(x => TypeHelper.Equal(x.Key, option.Key))
                : TypeHelper.Equal(option.Key, Value);
        }

        internal bool IsOptionHover(SelectResultModel<TValue> option)
        {
            return option != null && hoveredIndex >= 0 && hoveredIndex < Options.Count && Options[hoveredIndex] == option;
        }

        internal bool IsOptionVisible(SelectResultModel<TValue> option)
        {
            if (!Filterable || string.IsNullOrWhiteSpace(filterText))
            {
                return true;
            }
            return (option.Text ?? string.Empty).IndexOf(filterText, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private int FindSelectedIndex()
        {
            for (var i = 0; i < Options.Count; i++)
            {
                if (IsOptionSelected(Options[i]))
                {
                    return i;
                }
            }
            return Options.Count > 0 ? 0 : -1;
        }

        private IReadOnlyList<SelectResultModel<TValue>> FilteredOptions => Options.Where(IsOptionVisible).ToList();

        private Task NotifyVisibleChangeAsync(bool visible)
        {
            if (!visible && dropDownOption != null)
            {
                if (!ReserveKeyword)
                {
                    filterText = null;
                }
                dropDownOption = null;
            }
            if (!OnVisibleChange.HasDelegate)
            {
                StateHasChanged();
                return Task.CompletedTask;
            }
            return OnVisibleChange.InvokeAsync(visible);
        }

        protected async Task OnInputFocusAsync(FocusEventArgs e)
        {
            if (OnFocus.HasDelegate)
            {
                await OnFocus.InvokeAsync(e);
            }
        }

        protected async Task OnInputBlurAsync(FocusEventArgs e)
        {
            if (OnBlur.HasDelegate)
            {
                await OnBlur.InvokeAsync(e);
            }
        }

        internal void RegisterOption(SelectResultModel<TValue> option)
        {
            OptionsRendered = true;
            if (Options.Contains(option))
            {
                return;
            }
            Options.Add(option);
            if (Multiple)
            {
                SyncSelectedOptionsFromValues();
            }
        }

        internal bool IsDropDownOpen => dropDownOption != null && dropDownOption.IsShow;

        protected bool IsSelectDisabled => effectiveDisabled;

        protected InputSize EffectiveSize => effectiveSize;

        internal string DisplayLabel => Multiple
            ? (Filterable && IsDropDownOpen ? filterText : string.Empty)
            : (Filterable && IsDropDownOpen && filterText != null ? filterText : Label);

        protected string EffectivePlaceholder => Multiple && SelectedOptions.Any() ? string.Empty : Placeholder;

        private readonly List<SelectResultModel<TValue>> selectedOptions = new List<SelectResultModel<TValue>>();

        internal IReadOnlyList<SelectResultModel<TValue>> SelectedOptions => selectedOptions;

        protected IEnumerable<SelectResultModel<TValue>> VisibleSelectedOptions => CollapseTags
            ? selectedOptions.Take(Math.Max(1, MaxCollapseTags))
            : selectedOptions;

        protected int CollapsedTagCount => CollapseTags
            ? Math.Max(0, selectedOptions.Count - Math.Max(1, MaxCollapseTags))
            : 0;

        private async Task ToggleMultipleOptionAsync(SelectResultModel<TValue> item)
        {
            var existing = selectedOptions.FirstOrDefault(x => TypeHelper.Equal(x.Key, item.Key));
            if (existing == null)
            {
                selectedOptions.Add(item);
            }
            else
            {
                selectedOptions.Remove(existing);
            }
            await CommitMultipleSelectionAsync(true);
        }

        protected async Task RemoveTagAsync(SelectResultModel<TValue> item)
        {
            if (IsSelectDisabled || item == null)
            {
                return;
            }
            selectedOptions.RemoveAll(x => TypeHelper.Equal(x.Key, item.Key));
            await CommitMultipleSelectionAsync(true);
        }

        protected Task OnInputClearAsync(MouseEventArgs e)
        {
            if (Multiple)
            {
                ClearSelection();
            }
            return Task.CompletedTask;
        }

        private async Task CommitMultipleSelectionAsync(bool validate)
        {
            Values = CreateValuesCollection(selectedOptions.Select(x => x.Key));
            Label = string.Join(Separator, selectedOptions.Select(x => x.Text).Where(x => !string.IsNullOrWhiteSpace(x)));
            if (Values.Any())
            {
                Value = Values.Last();
                selectedOption = selectedOptions.LastOrDefault();
            }
            else
            {
                Value = default;
                selectedOption = null;
            }
            SetFieldValue(Value, validate);
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
                await OnChange.InvokeAsync(new BChangeEventArgs<SelectResultModel<TValue>>
                {
                    NewValue = selectedOptions.LastOrDefault()
                });
            }
        }

        private void SyncSelectedOptionsFromValues()
        {
            if (!Multiple)
            {
                return;
            }
            var sourceValues = Values ?? ExtractValues(Value);
            selectedOptions.Clear();
            foreach (var value in sourceValues)
            {
                var option = Options.FirstOrDefault(x => TypeHelper.Equal(x.Key, value));
                selectedOptions.Add(option ?? new SelectResultModel<TValue>
                {
                    Key = value,
                    Text = dict != null && dict.TryGetValue(value, out var text) ? text : Convert.ToString(value)
                });
            }
            Label = string.Join(Separator, selectedOptions.Select(x => x.Text).Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private ICollection<TValue> CreateValuesCollection(IEnumerable<TValue> values)
        {
            if (Values != null)
            {
                return values.ToList();
            }
            return values.ToList();
        }

        private IEnumerable<TValue> ExtractValues(object value)
        {
            if (value is IEnumerable<TValue> typedValues && value is not string)
            {
                return typedValues;
            }
            if (value is IEnumerable enumerable && value is not string)
            {
                return enumerable.Cast<object>()
                    .Where(x => x != null)
                    .Select(x => (TValue)TypeHelper.ChangeType(x, typeof(TValue)));
            }
            return Enumerable.Empty<TValue>();
        }

        string ISelectDropDownContext.LoadingText => LoadingText;

        string ISelectDropDownContext.EmptyText => ShouldShowNoMatch ? NoMatchText : NoDataText;

        bool ISelectDropDownContext.ShouldShowEmpty => ShouldShowEmpty;

        bool ISelectDropDownContext.ShouldShowNoMatch => ShouldShowNoMatch;

        Task ISelectDropDownContext.OnEndReachedAsync()
        {
            if (!OnEndReached.HasDelegate || Loading)
            {
                return Task.CompletedTask;
            }
            return OnEndReached.InvokeAsync();
        }

        private bool ShouldShowEmpty => !Loading && (OptionsRendered || ChildContent == null) && !FilteredOptions.Any();

        private bool ShouldShowNoMatch => !Loading && Options.Any() && Filterable && !Remote && !string.IsNullOrWhiteSpace(filterText) && !FilteredOptions.Any();

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            var enumValue = (TValue)value;
            if (nullable != null && value == null)
            {
                SelectedOption = null;
            }
            else
            {
                Label = dict?[enumValue];
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

        private static string GetSizeCssValue(InputSize size) => size switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };
    }
}
