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
        private DropDownOption dropDownOption;
        private ElementReference cascaderElement;
        private HtmlPropertyBuilder wrapperClsBuilder;
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
        public bool ExpandTriggerHover { get; set; }

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
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = NormalizeValue(value);
            SyncPathFromValue();
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

            OpenMenus();
            await OpenDropDownAsync();
        }

        private async Task OnInputClearAsync(MouseEventArgs e)
        {
            Value = new List<string>();
            activePath.Clear();
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
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
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
                await CommitSelectionAsync(option);
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
                        .AddIf(!option.HasChildren || CheckStrictly, "is-selectable");
                    builder.OpenElement(seq++, "li");
                    builder.SetKey(option);
                    builder.AddAttribute(seq++, "class", optionClsBuilder.ToString());
                    builder.AddAttribute(seq++, "role", "menuitem");
                    builder.AddAttribute(seq++, "aria-disabled", option.Disabled);
                    builder.AddAttribute(seq++, "aria-expanded", option.HasChildren ? IsInPath(option) : null);
                    builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => ExpandOptionAsync(option, index)));
                    if (ExpandTriggerHover)
                    {
                        builder.AddAttribute(seq++, "onmouseover", EventCallback.Factory.Create<MouseEventArgs>(this, () => ExpandOptionAsync(option, index)));
                    }
                    builder.OpenElement(seq++, "span");
                    builder.AddAttribute(seq++, "class", "el-cascader-node__label");
                    builder.AddContent(seq++, option.DisplayLabel);
                    builder.CloseElement();
                    if (option.HasChildren)
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
