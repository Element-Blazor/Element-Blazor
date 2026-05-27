using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElCascader : ElementFieldComponentBase<IList<string>>, ISelectDropDownContext
    {
        private static long dropDownIdSeed;
        private readonly string DropDownId = $"el-cascader-dropdown-{Interlocked.Increment(ref dropDownIdSeed)}";
        private readonly List<IList<CascaderOption>> menus = new List<IList<CascaderOption>>();
        private readonly List<CascaderOption> activePath = new List<CascaderOption>();
        private readonly List<IList<string>> checkedValues = new List<IList<string>>();
        private readonly List<CascaderSuggestion> suggestions = new List<CascaderSuggestion>();
        private DropDownOption dropDownOption;
        private ElementReference cascaderElement;
        private HtmlPropertyBuilder wrapperClsBuilder;
        private string filterText;
        private InputSize effectiveSize = InputSize.Normal;
        private bool effectiveDisabled;

        [Inject]
        internal PopupService PopupService { get; set; }

        [Parameter]
        public IList<string> Value { get; set; } = new List<string>();

        [Parameter]
        public IList<string> ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<IList<string>> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<IList<string>> ModelValueChanged { get; set; }

        [Parameter]
        public IList<IList<string>> Values { get; set; } = new List<IList<string>>();

        [Parameter]
        public IList<IList<string>> ModelValues
        {
            get => Values;
            set => Values = value;
        }

        [Parameter]
        public EventCallback<IList<IList<string>>> ValuesChanged { get; set; }

        [Parameter]
        public EventCallback<IList<IList<string>>> ModelValuesChanged { get; set; }

        [Parameter]
        public IEnumerable<CascaderOption> Options { get; set; }

        [Parameter]
        public string Placeholder { get; set; } = "请选择";

        [Parameter]
        public string Separator { get; set; } = " / ";

        [Parameter]
        public bool Clearable { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        [Parameter]
        public bool CheckStrictly { get; set; }

        [Parameter]
        public bool Multiple { get; set; }

        [Parameter]
        public bool Lazy { get; set; }

        [Parameter]
        public Func<CascaderOption, Task<IEnumerable<CascaderOption>>> LazyLoad { get; set; }

        [Parameter]
        public bool Filterable { get; set; }

        [Parameter]
        public Func<CascaderOption, string, bool> FilterMethod { get; set; }

        [Parameter]
        public bool ExpandTriggerHover { get; set; }

        [Parameter]
        public RenderFragment<CascaderOption> NodeTemplate { get; set; }

        [Parameter]
        public RenderFragment<CascaderSuggestion> SuggestionItemTemplate { get; set; }

        [Parameter]
        public string PrefixIcon { get; set; }

        [Parameter]
        public string PopperClass { get; set; }

        [Parameter]
        public string PopperStyle { get; set; }

        [Parameter]
        public int PopperMaxHeight { get; set; } = 204;

        [Parameter]
        public bool FitInputWidth { get; set; }

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public EventCallback<IList<string>> OnChange { get; set; }

        [Parameter]
        public EventCallback<IList<IList<string>>> OnValuesChange { get; set; }

        [Parameter]
        public EventCallback<CascaderOption> OnExpandChange { get; set; }

        [Parameter]
        public EventCallback<bool> OnVisibleChange { get; set; }

        [Parameter]
        public EventCallback<FocusEventArgs> OnFocus { get; set; }

        [Parameter]
        public EventCallback<FocusEventArgs> OnBlur { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            effectiveDisabled = Disabled || (FormItem?.Form?.Disabled ?? false);
            effectiveSize = Size == InputSize.Normal
                ? FormItem?.Size ?? FormItem?.Form?.EffectiveSize ?? ResolveInputSize(InputSize.Normal)
                : Size;
            var sizeCssValue = GetSizeCssValue(effectiveSize);
            wrapperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-cascader", Cls)
                .AddIf(effectiveDisabled, "is-disabled")
                .AddIf(IsDropDownOpen, "is-focus")
                .AddIf(sizeCssValue != null, $"el-cascader--{sizeCssValue}");

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    Value = NormalizeValue(FormItem.OriginValue);
                }
                SetFieldValue(Value, false);
            }

            SyncPathFromValue();
            SyncCheckedValuesFromValues();
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = NormalizeValue(value);
            SyncPathFromValue();
            SyncCheckedValuesFromValues();
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

        private async Task OnCascaderClickAsync(MouseEventArgs e)
        {
            if (effectiveDisabled)
            {
                return;
            }

            if (IsDropDownOpen)
            {
                await CloseDropDownAsync();
                return;
            }

            filterText = null;
            suggestions.Clear();
            OpenMenus();
            await OpenDropDownAsync();
        }

        private async Task OnInputClearAsync(MouseEventArgs e)
        {
            Value = new List<string>();
            Values = new List<IList<string>>();
            activePath.Clear();
            checkedValues.Clear();
            filterText = null;
            suggestions.Clear();
            OpenMenus();
            SetFieldValue(Value, ValidateEvent);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
            if (ValuesChanged.HasDelegate)
            {
                await ValuesChanged.InvokeAsync(Values);
            }
            if (ModelValuesChanged.HasDelegate)
            {
                await ModelValuesChanged.InvokeAsync(Values);
            }
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
            if (OnValuesChange.HasDelegate)
            {
                await OnValuesChange.InvokeAsync(Values);
            }
            await CloseDropDownAsync();
        }

        private async Task OnInputFocusAsync(FocusEventArgs e)
        {
            if (OnFocus.HasDelegate)
            {
                await OnFocus.InvokeAsync(e);
            }
        }

        private async Task OnInputBlurAsync(FocusEventArgs e)
        {
            if (ValidateEvent)
            {
                SetFieldValue(Value, true);
            }
            if (OnBlur.HasDelegate)
            {
                await OnBlur.InvokeAsync(e);
            }
        }

        private async Task ExpandOptionAsync(CascaderOption option, int menuIndex)
        {
            if (option == null || option.Disabled)
            {
                return;
            }

            TrimActivePath(menuIndex);
            activePath.Add(option);
            TrimMenus(menuIndex + 1);

            if (Lazy)
            {
                await EnsureLazyLoadedAsync(option);
            }

            if (option.HasChildren)
            {
                menus.Add(option.Children);
                if (OnExpandChange.HasDelegate)
                {
                    await OnExpandChange.InvokeAsync(option);
                }
            }

            if (CheckStrictly || option.Leaf || !option.HasChildren)
            {
                if (Multiple)
                {
                    await ToggleMultipleSelectionAsync(activePath);
                }
                else
                {
                    await CommitSelectionAsync(option);
                }
            }

            RefreshDropDown();
        }

        private async Task OnInputValueChangedAsync(string value)
        {
            if (!Filterable)
            {
                return;
            }

            filterText = value;
            BuildSuggestions();
            if (!IsDropDownOpen)
            {
                await OpenDropDownAsync();
            }
            RefreshDropDown();
        }

        private async Task CommitSelectionAsync(CascaderOption option)
        {
            Value = activePath.Select(x => x.Value).Where(x => x != null).ToList();
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

            if (!option.HasChildren || !CheckStrictly)
            {
                await CloseDropDownAsync();
            }
        }

        private async Task ToggleMultipleSelectionAsync(IEnumerable<CascaderOption> path)
        {
            var values = path.Select(x => x.Value).Where(x => x != null).ToList();
            if (!values.Any())
            {
                return;
            }

            var existing = checkedValues.FindIndex(x => SamePath(x, values));
            if (existing >= 0)
            {
                checkedValues.RemoveAt(existing);
            }
            else
            {
                checkedValues.Add(values);
            }

            Values = checkedValues.Select(x => (IList<string>)x.ToList()).ToList();
            Value = Values.LastOrDefault()?.ToList() ?? new List<string>();
            SetFieldValue(Value, ValidateEvent);

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
            if (ValuesChanged.HasDelegate)
            {
                await ValuesChanged.InvokeAsync(Values);
            }
            if (ModelValuesChanged.HasDelegate)
            {
                await ModelValuesChanged.InvokeAsync(Values);
            }
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
            if (OnValuesChange.HasDelegate)
            {
                await OnValuesChange.InvokeAsync(Values);
            }
        }

        private async Task SelectSuggestionAsync(CascaderSuggestion suggestion)
        {
            if (suggestion?.Path == null || !suggestion.Path.Any())
            {
                return;
            }

            activePath.Clear();
            activePath.AddRange(suggestion.Path);
            Value = suggestion.Values.ToList();
            OpenMenus();

            if (Multiple)
            {
                await ToggleMultipleSelectionAsync(suggestion.Path);
                return;
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
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
            await CloseDropDownAsync();
        }

        private void OpenMenus()
        {
            menus.Clear();
            menus.Add(RootOptions);
            var currentOptions = RootOptions;
            var nextPath = new List<CascaderOption>();

            foreach (var value in Value ?? Enumerable.Empty<string>())
            {
                var node = currentOptions.FirstOrDefault(x => x.Value == value);
                if (node == null)
                {
                    break;
                }

                nextPath.Add(node);
                if (!node.HasChildren)
                {
                    break;
                }

                currentOptions = node.Children;
                menus.Add(currentOptions);
            }

            activePath.Clear();
            activePath.AddRange(nextPath);
        }

        private void SyncPathFromValue()
        {
            activePath.Clear();
            var currentOptions = RootOptions;
            foreach (var value in Value ?? Enumerable.Empty<string>())
            {
                var option = currentOptions.FirstOrDefault(x => x.Value == value);
                if (option == null)
                {
                    break;
                }

                activePath.Add(option);
                currentOptions = option.Children ?? new List<CascaderOption>();
            }
        }

        private void SyncCheckedValuesFromValues()
        {
            checkedValues.Clear();
            foreach (var value in Values ?? Enumerable.Empty<IList<string>>())
            {
                var normalized = NormalizeValue(value);
                if (normalized.Any())
                {
                    checkedValues.Add(normalized);
                }
            }
        }

        private async Task OpenDropDownAsync()
        {
            if (dropDownOption != null)
            {
                RefreshDropDown();
                return;
            }

            dropDownOption = new DropDownOption
            {
                Select = this,
                Target = cascaderElement,
                OptionContent = BuildPanelContent,
                PopperClass = string.Join(" ", new[] { "el-cascader__dropdown", PopperClass }.Where(x => !string.IsNullOrWhiteSpace(x))),
                PopperStyle = PopperStyle,
                MaxHeight = PopperMaxHeight,
                FitInputWidth = FitInputWidth,
                DropDownId = DropDownId,
                Refresh = () => InvokeAsync(StateHasChanged),
                OnClosed = () => NotifyVisibleChangeAsync(false),
                IsShow = true
            };
            PopupService.SelectDropDownOptions.Add(dropDownOption);
            await NotifyVisibleChangeAsync(true);
        }

        private async Task CloseDropDownAsync()
        {
            if (dropDownOption?.Instance != null)
            {
                await dropDownOption.Instance.CloseDropDownAsync(dropDownOption);
                return;
            }

            if (dropDownOption != null)
            {
                PopupService.SelectDropDownOptions.Remove(dropDownOption);
                dropDownOption = null;
                await NotifyVisibleChangeAsync(false);
            }
        }

        private Task NotifyVisibleChangeAsync(bool visible)
        {
            if (!visible)
            {
                dropDownOption = null;
                filterText = null;
                suggestions.Clear();
            }

            if (OnVisibleChange.HasDelegate)
            {
                return OnVisibleChange.InvokeAsync(visible);
            }

            StateHasChanged();
            return Task.CompletedTask;
        }

        private void BuildPanelContent(RenderTreeBuilder builder)
        {
            var seq = 0;
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "el-cascader-panel");
            builder.AddAttribute(seq++, "role", "menu");

            if (Filterable && !string.IsNullOrWhiteSpace(filterText))
            {
                BuildSuggestionContent(builder, ref seq);
                builder.CloseElement();
                return;
            }

            for (var menuIndex = 0; menuIndex < menus.Count; menuIndex++)
            {
                var index = menuIndex;
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "class", "el-cascader-menu");
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "class", "el-cascader-menu__wrap el-scrollbar__wrap");
                builder.OpenElement(seq++, "ul");
                builder.AddAttribute(seq++, "class", "el-cascader-menu__list");
                foreach (var option in menus[index])
                {
                    var optionClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                        .Add("el-cascader-node")
                        .AddIf(option.Disabled, "is-disabled")
                        .AddIf(IsInPath(option), "in-active-path")
                        .AddIf(IsSelected(option), "is-active")
                        .AddIf(IsChecked(option), "is-checked")
                        .AddIf(!option.HasChildren || CheckStrictly, "is-selectable");
                    builder.OpenElement(seq++, "li");
                    builder.SetKey(option);
                    builder.AddAttribute(seq++, "class", optionClsBuilder.ToString());
                    builder.AddAttribute(seq++, "role", "menuitem");
                    builder.AddAttribute(seq++, "aria-disabled", option.Disabled);
                    builder.AddAttribute(seq++, "aria-expanded", option.HasChildren ? IsInPath(option) : null);
                    builder.AddAttribute(seq++, "aria-checked", Multiple ? IsChecked(option) : null);
                    builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => ExpandOptionAsync(option, index)));
                    if (ExpandTriggerHover)
                    {
                        builder.AddAttribute(seq++, "onmouseover", EventCallback.Factory.Create<MouseEventArgs>(this, () => ExpandOptionAsync(option, index)));
                    }
                    if (Multiple)
                    {
                        builder.OpenElement(seq++, "span");
                        builder.AddAttribute(seq++, "class", "el-checkbox__input" + (IsChecked(option) ? " is-checked" : string.Empty));
                        builder.OpenElement(seq++, "span");
                        builder.AddAttribute(seq++, "class", "el-checkbox__inner");
                        builder.CloseElement();
                        builder.CloseElement();
                    }
                    builder.OpenElement(seq++, "span");
                    builder.AddAttribute(seq++, "class", "el-cascader-node__label");
                    if (NodeTemplate != null)
                    {
                        builder.AddContent(seq++, NodeTemplate(option));
                    }
                    else
                    {
                        builder.AddContent(seq++, option.DisplayLabel);
                    }
                    builder.CloseElement();
                    if (option.Loading)
                    {
                        builder.OpenElement(seq++, "i");
                        builder.AddAttribute(seq++, "class", "el-icon-loading el-cascader-node__postfix");
                        builder.CloseElement();
                    }
                    else if (option.HasChildren || (Lazy && !option.Leaf))
                    {
                        builder.OpenElement(seq++, "i");
                        builder.AddAttribute(seq++, "class", "el-icon-arrow-right el-cascader-node__postfix");
                        builder.CloseElement();
                    }
                    else if (IsSelected(option))
                    {
                        builder.OpenElement(seq++, "i");
                        builder.AddAttribute(seq++, "class", "el-icon-check el-cascader-node__postfix");
                        builder.CloseElement();
                    }
                    builder.CloseElement();
                }
                builder.CloseElement();
                builder.CloseElement();
                builder.CloseElement();
            }
            builder.CloseElement();
        }

        private void BuildSuggestionContent(RenderTreeBuilder builder, ref int seq)
        {
            if (!suggestions.Any())
            {
                builder.OpenElement(seq++, "p");
                builder.AddAttribute(seq++, "class", "el-cascader__empty-text");
                builder.AddContent(seq++, "无匹配数据");
                builder.CloseElement();
                return;
            }

            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "el-cascader-menu");
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "el-cascader-menu__wrap el-scrollbar__wrap");
            builder.OpenElement(seq++, "ul");
            builder.AddAttribute(seq++, "class", "el-cascader-menu__list");
            foreach (var suggestion in suggestions)
            {
                builder.OpenElement(seq++, "li");
                builder.SetKey(suggestion);
                builder.AddAttribute(seq++, "class", "el-cascader-node is-selectable");
                builder.AddAttribute(seq++, "role", "menuitem");
                builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => SelectSuggestionAsync(suggestion)));
                builder.OpenElement(seq++, "span");
                builder.AddAttribute(seq++, "class", "el-cascader-node__label");
                if (SuggestionItemTemplate != null)
                {
                    builder.AddContent(seq++, SuggestionItemTemplate(suggestion));
                }
                else
                {
                    builder.AddContent(seq++, suggestion.Text);
                }
                builder.CloseElement();
                builder.CloseElement();
            }
            builder.CloseElement();
            builder.CloseElement();
            builder.CloseElement();
        }

        public override void Dispose()
        {
            if (dropDownOption != null)
            {
                PopupService.SelectDropDownOptions.Remove(dropDownOption);
            }
            base.Dispose();
        }

        private IList<CascaderOption> RootOptions => Options?.ToList() ?? new List<CascaderOption>();

        private bool IsDropDownOpen => dropDownOption != null && dropDownOption.IsShow;

        private bool IsCascaderDisabled => effectiveDisabled;

        private InputSize EffectiveSize => effectiveSize;

        private string SuffixIconClass => $"el-icon-arrow-up el-cascader__caret{(IsDropDownOpen ? " is-reverse" : string.Empty)}";

        private string DisplayValue => activePath.Any()
            ? string.Join(Separator, activePath.Select(x => x.DisplayLabel))
            : string.Empty;

        private string InputValue => Filterable && IsDropDownOpen && !string.IsNullOrWhiteSpace(filterText)
            ? filterText
            : DisplayValue;

        bool ISelectDropDownContext.Loading => false;

        string ISelectDropDownContext.LoadingText => string.Empty;

        string ISelectDropDownContext.EmptyText => "无数据";

        bool ISelectDropDownContext.ShouldShowEmpty => !RootOptions.Any();

        bool ISelectDropDownContext.ShouldShowNoMatch => false;

        Task ISelectDropDownContext.OnEndReachedAsync()
        {
            return Task.CompletedTask;
        }

        private void RefreshDropDown()
        {
            dropDownOption?.RequestRender?.Invoke();
            StateHasChanged();
        }

        private bool IsInPath(CascaderOption option)
        {
            return activePath.Contains(option);
        }

        private bool IsSelected(CascaderOption option)
        {
            return activePath.LastOrDefault() == option && Value?.LastOrDefault() == option.Value;
        }

        private bool IsChecked(CascaderOption option)
        {
            return checkedValues.Any(path => path.LastOrDefault() == option.Value);
        }

        private async Task EnsureLazyLoadedAsync(CascaderOption option)
        {
            if (!Lazy || LazyLoad == null || option == null || option.Leaf || option.Loaded || option.HasChildren)
            {
                return;
            }

            option.Loading = true;
            RefreshDropDown();
            try
            {
                var children = await LazyLoad(option);
                option.Children = children?.Where(x => x != null).ToList() ?? new List<CascaderOption>();
                option.Loaded = true;
            }
            finally
            {
                option.Loading = false;
            }
        }

        private void BuildSuggestions()
        {
            suggestions.Clear();
            if (string.IsNullOrWhiteSpace(filterText))
            {
                return;
            }

            foreach (var path in EnumeratePaths(RootOptions, new List<CascaderOption>()))
            {
                var option = path.LastOrDefault();
                if (option == null || option.Disabled || (option.HasChildren && !CheckStrictly))
                {
                    continue;
                }

                var matches = FilterMethod != null
                    ? FilterMethod(option, filterText)
                    : path.Any(x => x.DisplayLabel?.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);
                if (!matches)
                {
                    continue;
                }

                suggestions.Add(new CascaderSuggestion
                {
                    Path = path.ToList(),
                    Text = string.Join(Separator, path.Select(x => x.DisplayLabel))
                });
            }
        }

        private IEnumerable<IList<CascaderOption>> EnumeratePaths(IEnumerable<CascaderOption> options, IList<CascaderOption> prefix)
        {
            foreach (var option in options ?? Enumerable.Empty<CascaderOption>())
            {
                var path = prefix.Concat(new[] { option }).ToList();
                if (option.HasChildren)
                {
                    foreach (var childPath in EnumeratePaths(option.Children, path))
                    {
                        yield return childPath;
                    }
                }

                if (!option.HasChildren || CheckStrictly)
                {
                    yield return path;
                }
            }
        }

        private static bool SamePath(IList<string> first, IList<string> second)
        {
            return first != null && second != null && first.SequenceEqual(second);
        }

        private void TrimMenus(int count)
        {
            while (menus.Count > count)
            {
                menus.RemoveAt(menus.Count - 1);
            }
        }

        private void TrimActivePath(int count)
        {
            while (activePath.Count > count)
            {
                activePath.RemoveAt(activePath.Count - 1);
            }
        }

        private static IList<string> NormalizeValue(object value)
        {
            if (value == null)
            {
                return new List<string>();
            }
            if (value is IList<string> stringList)
            {
                return stringList;
            }
            if (value is IEnumerable<string> stringEnumerable)
            {
                return stringEnumerable.ToList();
            }
            return new List<string> { Convert.ToString(value) };
        }

        private static string GetSizeCssValue(InputSize size) => size switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };
    }
}
