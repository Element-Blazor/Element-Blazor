using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElTimeSelect : ElementFieldComponentBase<string>, ISelectDropDownContext
    {
        private static long dropDownIdSeed;
        private readonly string DropDownId = $"el-time-select-dropdown-{Interlocked.Increment(ref dropDownIdSeed)}";
        private DropDownOption dropDownOption;
        private ElementReference timeSelectElement;
        private HtmlPropertyBuilder wrapperClsBuilder;
        private InputSize effectiveSize = InputSize.Normal;
        private bool effectiveDisabled;
        private int hoveredIndex = -1;

        [Inject]
        internal PopupService PopupService { get; set; }

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public string ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<string> ModelValueChanged { get; set; }

        [Parameter]
        public string Start { get; set; } = "09:00";

        [Parameter]
        public string End { get; set; } = "18:00";

        [Parameter]
        public string Step { get; set; } = "00:30";

        [Parameter]
        public string MinTime { get; set; }

        [Parameter]
        public string MaxTime { get; set; }

        [Parameter]
        public string Format { get; set; } = @"hh\:mm";

        [Parameter]
        public string Placeholder { get; set; } = "请选择时间";

        [Parameter]
        public bool Clearable { get; set; } = true;

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
        public string PrefixIcon { get; set; } = "el-icon-time";

        [Parameter]
        public string PopperClass { get; set; }

        [Parameter]
        public string PopperStyle { get; set; }

        [Parameter]
        public int PopperMaxHeight { get; set; } = 274;

        [Parameter]
        public bool FitInputWidth { get; set; } = true;

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public EventCallback<string> OnChange { get; set; }

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
                .Add("el-time-select", Cls)
                .AddIf(effectiveDisabled, "is-disabled")
                .AddIf(IsDropDownOpen, "is-focus")
                .AddIf(sizeCssValue != null, $"el-time-select--{sizeCssValue}");

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    Value = Convert.ToString(FormItem.OriginValue);
                }
                SetFieldValue(Value, false);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = Convert.ToString(value);
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

        private async Task OnTimeSelectClickAsync(MouseEventArgs e)
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
            await OpenDropDownAsync();
        }

        private async Task OnInputClearAsync(MouseEventArgs e)
        {
            Value = string.Empty;
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

        private async Task SelectTimeAsync(string time)
        {
            if (string.IsNullOrWhiteSpace(time))
            {
                return;
            }
            Value = time;
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
                case "Home":
                    if (!IsDropDownOpen)
                    {
                        await OpenDropDownAsync();
                    }
                    SetHoverToEdge(first: true);
                    break;
                case "End":
                    if (!IsDropDownOpen)
                    {
                        await OpenDropDownAsync();
                    }
                    SetHoverToEdge(first: false);
                    break;
                case "Enter":
                    if (!IsDropDownOpen)
                    {
                        await OpenDropDownAsync();
                        return;
                    }
                    await SelectHoveredOptionAsync();
                    break;
                case " ":
                    if (!IsDropDownOpen)
                    {
                        await OpenDropDownAsync();
                    }
                    break;
                case "Escape":
                    await CloseDropDownAsync();
                    break;
            }
        }

        private async Task OpenDropDownAsync()
        {
            if (effectiveDisabled)
            {
                return;
            }
            if (dropDownOption != null)
            {
                RefreshDropDown();
                return;
            }

            hoveredIndex = FindInitialHoverIndex();
            dropDownOption = new DropDownOption
            {
                Select = this,
                Target = timeSelectElement,
                OptionContent = BuildTimeOptions,
                PopperClass = string.Join(" ", new[] { "el-time-select__dropdown", PopperClass }.Where(x => !string.IsNullOrWhiteSpace(x))),
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
                hoveredIndex = -1;
            }

            if (OnVisibleChange.HasDelegate)
            {
                return OnVisibleChange.InvokeAsync(visible);
            }

            StateHasChanged();
            return Task.CompletedTask;
        }

        private void BuildTimeOptions(RenderTreeBuilder builder)
        {
            var seq = 0;
            var options = Options;
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                var index = i;
                var itemClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                    .Add("el-select-dropdown__item")
                    .AddIf(option.Disabled, "is-disabled")
                    .AddIf(option.Value == Value, "selected")
                    .AddIf(index == hoveredIndex, "hover");
                builder.OpenElement(seq++, "li");
                builder.SetKey(option.Value);
                builder.AddAttribute(seq++, "id", GetOptionId(index));
                builder.AddAttribute(seq++, "class", itemClsBuilder.ToString());
                builder.AddAttribute(seq++, "role", "option");
                builder.AddAttribute(seq++, "aria-selected", option.Value == Value);
                builder.AddAttribute(seq++, "aria-disabled", option.Disabled);
                builder.AddAttribute(seq++, "tabindex", option.Disabled ? -1 : 0);
                builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => option.Disabled ? Task.CompletedTask : SelectTimeAsync(option.Value)));
                builder.AddContent(seq++, option.Value);
                builder.CloseElement();
            }
        }

        public override void Dispose()
        {
            if (dropDownOption != null)
            {
                PopupService.SelectDropDownOptions.Remove(dropDownOption);
            }
            base.Dispose();
        }

        private IReadOnlyList<TimeSelectOption> Options
        {
            get
            {
                var options = new List<TimeSelectOption>();
                var start = ParseTime(Start, TimeSpan.FromHours(9));
                var end = ParseTime(End, TimeSpan.FromHours(18));
                var step = ParseTime(Step, TimeSpan.FromMinutes(30));
                var minTime = string.IsNullOrWhiteSpace(MinTime) ? (TimeSpan?)null : ParseTime(MinTime, TimeSpan.Zero);
                var maxTime = string.IsNullOrWhiteSpace(MaxTime) ? (TimeSpan?)null : ParseTime(MaxTime, TimeSpan.FromDays(1));
                if (step <= TimeSpan.Zero)
                {
                    step = TimeSpan.FromMinutes(30);
                }
                for (var current = start; current <= end; current = current.Add(step))
                {
                    options.Add(new TimeSelectOption
                    {
                        Value = current.ToString(Format, CultureInfo.CurrentCulture),
                        Disabled = (minTime.HasValue && current < minTime.Value) || (maxTime.HasValue && current > maxTime.Value)
                    });
                }
                return options;
            }
        }

        private static TimeSpan ParseTime(string value, TimeSpan fallback)
        {
            if (TimeSpan.TryParse(value, CultureInfo.CurrentCulture, out var result))
            {
                return result;
            }
            if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return fallback;
        }

        private void RefreshDropDown()
        {
            dropDownOption?.RequestRender?.Invoke();
            StateHasChanged();
        }

        private void MoveHover(int step)
        {
            var options = Options;
            if (!options.Any(x => !x.Disabled))
            {
                hoveredIndex = -1;
                StateHasChanged();
                return;
            }

            var start = hoveredIndex < 0
                ? step > 0 ? 0 : options.Count - 1
                : hoveredIndex + step;
            hoveredIndex = FindNextEnabledIndex(options, start, step);
            RefreshDropDown();
        }

        private void SetHoverToEdge(bool first)
        {
            var options = Options;
            hoveredIndex = first
                ? FindNextEnabledIndex(options, 0, 1)
                : FindNextEnabledIndex(options, options.Count - 1, -1);
            RefreshDropDown();
        }

        private async Task SelectHoveredOptionAsync()
        {
            var options = Options;
            if (hoveredIndex < 0 || hoveredIndex >= options.Count)
            {
                return;
            }

            var option = options[hoveredIndex];
            if (option.Disabled)
            {
                return;
            }

            await SelectTimeAsync(option.Value);
        }

        private int FindInitialHoverIndex()
        {
            var options = Options;
            for (var i = 0; i < options.Count; i++)
            {
                if (!options[i].Disabled && string.Equals(options[i].Value, Value, StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return FindNextEnabledIndex(options, 0, 1);
        }

        private static int FindNextEnabledIndex(IReadOnlyList<TimeSelectOption> options, int start, int step)
        {
            if (options == null || options.Count == 0 || step == 0)
            {
                return -1;
            }

            var count = options.Count;
            for (var offset = 0; offset < count; offset++)
            {
                var index = ((start + offset * step) % count + count) % count;
                if (!options[index].Disabled)
                {
                    return index;
                }
            }

            return -1;
        }

        private string GetOptionId(int index)
        {
            return $"{DropDownId}-option-{index}";
        }

        private bool IsDropDownOpen => dropDownOption != null && dropDownOption.IsShow;

        private bool IsTimeSelectDisabled => effectiveDisabled;

        private InputSize EffectiveSize => effectiveSize;

        private string SuffixIconClass => $"el-icon-arrow-up el-select__caret{(IsDropDownOpen ? " is-reverse" : string.Empty)}";

        private string ActiveDescendantId => IsDropDownOpen && hoveredIndex >= 0 ? GetOptionId(hoveredIndex) : null;

        private string AriaExpanded => IsDropDownOpen ? "true" : "false";

        private string AriaDisabled => IsTimeSelectDisabled ? "true" : "false";

        bool ISelectDropDownContext.Loading => false;

        string ISelectDropDownContext.LoadingText => string.Empty;

        string ISelectDropDownContext.EmptyText => "无数据";

        bool ISelectDropDownContext.ShouldShowEmpty => !Options.Any();

        bool ISelectDropDownContext.ShouldShowNoMatch => false;

        Task ISelectDropDownContext.OnEndReachedAsync()
        {
            return Task.CompletedTask;
        }

        private static string GetSizeCssValue(InputSize size) => size switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };

        private sealed class TimeSelectOption
        {
            public string Value { get; set; }

            public bool Disabled { get; set; }
        }
    }
}
