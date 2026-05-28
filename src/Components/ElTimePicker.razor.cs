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
    public partial class ElTimePicker
    {
        private static long dropDownIdSeed;
        private readonly string DropDownId = $"el-time-picker-dropdown-{Interlocked.Increment(ref dropDownIdSeed)}";
        private DropDownOption dropDownOption;
        private ElementReference pickerElement;
        private ElementReference inputElement;
        private bool effectiveDisabled;
        private InputSize effectiveSize = InputSize.Normal;
        private bool isFocus;
        private bool isHovering;
        private TimeSpan? pendingSingleValue;
        private TimeSpan? pendingRangeStart;
        private TimeSpan? pendingRangeEnd;

        [Inject]
        internal PopupService PopupService { get; set; }

        [Parameter]
        public TimeSpan? Value { get; set; }

        [Parameter]
        public TimeSpan? ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<TimeSpan?> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<TimeSpan?> ModelValueChanged { get; set; }

        [Parameter]
        public TimeSpan? Time
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<TimeSpan?> TimeChanged { get; set; }

        [Parameter]
        public IList<TimeSpan?> RangeValue { get; set; } = new List<TimeSpan?>();

        [Parameter]
        public IList<TimeSpan?> ModelRangeValue
        {
            get => RangeValue;
            set => RangeValue = value;
        }

        [Parameter]
        public EventCallback<IList<TimeSpan?>> RangeValueChanged { get; set; }

        [Parameter]
        public EventCallback<IList<TimeSpan?>> ModelRangeValueChanged { get; set; }

        [Parameter]
        public string StringValue { get; set; }

        [Parameter]
        public EventCallback<string> StringValueChanged { get; set; }

        [Parameter]
        public IList<string> StringRangeValue { get; set; } = new List<string>();

        [Parameter]
        public EventCallback<IList<string>> StringRangeValueChanged { get; set; }

        [Parameter]
        public TimePickerType Type { get; set; } = TimePickerType.Time;

        [Parameter]
        public bool IsRange { get; set; }

        [Parameter]
        public string Format { get; set; } = "HH:mm:ss";

        [Parameter]
        public string ValueFormat { get; set; }

        [Parameter]
        public TimeSpan? DefaultValue { get; set; }

        [Parameter]
        public TimeSpan? DefaultStartTime { get; set; }

        [Parameter]
        public TimeSpan? DefaultEndTime { get; set; }

        [Parameter]
        public string Placeholder { get; set; } = "请选择时间";

        [Parameter]
        public string StartPlaceholder { get; set; } = "开始时间";

        [Parameter]
        public string EndPlaceholder { get; set; } = "结束时间";

        [Parameter]
        public string RangeSeparator { get; set; } = "-";

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        [Parameter]
        public bool Readonly { get; set; }

        [Parameter]
        public bool Editable { get; set; } = true;

        [Parameter]
        public bool Clearable { get; set; } = true;

        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        [Parameter]
        public string PrefixIcon { get; set; } = "el-icon-time";

        [Parameter]
        public string ClearIcon { get; set; } = "el-icon-circle-close";

        [Parameter]
        public string PopperClass { get; set; }

        [Parameter]
        public string PopperStyle { get; set; }

        [Parameter]
        public int PopperMaxHeight { get; set; } = 336;

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public EventCallback<TimeSpan?> OnChange { get; set; }

        [Parameter]
        public EventCallback<IList<TimeSpan?>> OnRangeChange { get; set; }

        [Parameter]
        public EventCallback OnClear { get; set; }

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
            SyncValueFromFormattedParameters();

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    Value = ConvertToTime(FormItem.OriginValue);
                }
                SetFieldValue(Value, false);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = ConvertToTime(value);
            ClearPendingValues();
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                _ = ModelValueChanged.InvokeAsync(Value);
            }
            if (TimeChanged.HasDelegate)
            {
                _ = TimeChanged.InvokeAsync(Value);
            }
            else
            {
                StateHasChanged();
            }
        }

        private async Task OnPickerClickAsync(MouseEventArgs e)
        {
            if (effectiveDisabled || Readonly)
            {
                return;
            }

            await OpenDropDownAsync();
        }

        private async Task OnInputFocusAsync(FocusEventArgs e)
        {
            isFocus = true;
            if (OnFocus.HasDelegate)
            {
                await OnFocus.InvokeAsync(e);
            }
            await OpenDropDownAsync();
        }

        private async Task OnInputBlurAsync(FocusEventArgs e)
        {
            isFocus = false;
            if (ValidateEvent)
            {
                SetFieldValue(Value, true);
            }
            if (OnBlur.HasDelegate)
            {
                await OnBlur.InvokeAsync(e);
            }
        }

        private async Task OnSingleInputChangedAsync(ChangeEventArgs e)
        {
            if (!Editable || effectiveDisabled || Readonly)
            {
                return;
            }

            var text = Convert.ToString(e.Value, CultureInfo.CurrentCulture);
            if (!TryParseTime(text, Format, out var parsed))
            {
                return;
            }

            await CommitSingleValueAsync(parsed, ValidateEvent, notifyChange: true, close: false);
        }

        private async Task OnRangeStartChangedAsync(ChangeEventArgs e)
        {
            await CommitRangeTextAsync(Convert.ToString(e.Value, CultureInfo.CurrentCulture), EndDisplayValue);
        }

        private async Task OnRangeEndChangedAsync(ChangeEventArgs e)
        {
            await CommitRangeTextAsync(StartDisplayValue, Convert.ToString(e.Value, CultureInfo.CurrentCulture));
        }

        private async Task CommitRangeTextAsync(string startText, string endText)
        {
            if (!Editable || effectiveDisabled || Readonly)
            {
                return;
            }
            if (!TryParseTime(startText, Format, out var start) || !TryParseTime(endText, Format, out var end))
            {
                return;
            }

            await CommitRangeValueAsync(NormalizeRange(new TimeSpan?[] { start, end }), ValidateEvent, notifyChange: true, close: false);
        }

        private async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
            {
                await CloseDropDownAsync();
            }
            else if (e.Key == "Enter" || e.Key == "ArrowDown")
            {
                await OpenDropDownAsync();
            }
        }

        private async Task OnClearClickAsync(MouseEventArgs e)
        {
            if (!Clearable || effectiveDisabled || Readonly)
            {
                return;
            }

            if (IsRangeMode)
            {
                await CommitRangeValueAsync(new List<TimeSpan?>(), ValidateEvent, notifyChange: true, close: true);
            }
            else
            {
                await CommitSingleValueAsync(null, ValidateEvent, notifyChange: true, close: true);
            }
            if (OnClear.HasDelegate)
            {
                await OnClear.InvokeAsync();
            }
        }

        private async Task OpenDropDownAsync()
        {
            if (effectiveDisabled || Readonly || dropDownOption != null)
            {
                return;
            }

            InitializePendingValues();
            dropDownOption = new DropDownOption
            {
                Target = pickerElement,
                OptionContent = BuildDropDownContent,
                PopperClass = string.Join(" ", new[] { "el-time-panel__popover", PopperClass }.Where(x => !string.IsNullOrWhiteSpace(x))),
                PopperStyle = PopperStyle,
                MaxHeight = PopperMaxHeight,
                FitInputWidth = false,
                AutoWidth = false,
                Width = IsRangeMode ? 430 : 220,
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
                ClearPendingValues();
            }

            if (OnVisibleChange.HasDelegate)
            {
                return OnVisibleChange.InvokeAsync(visible);
            }

            StateHasChanged();
            return Task.CompletedTask;
        }

        private void BuildDropDownContent(RenderTreeBuilder builder)
        {
            var seq = 0;
            builder.OpenElement(seq++, "li");
            builder.AddAttribute(seq++, "class", "el-date-picker__panel-item");
            builder.AddAttribute(seq++, "role", "presentation");
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", DropDownPanelClass);
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "el-time-panel__content");
            builder.AddAttribute(seq++, "style", IsRangeMode ? "display:flex;gap:12px;" : string.Empty);

            if (IsRangeMode)
            {
                BuildRangePanel(builder, ref seq, true);
                BuildRangePanel(builder, ref seq, false);
            }
            else
            {
                BuildTimePanel(builder, ref seq, "single", PendingSingleTimeValue, SetSinglePendingPartAsync);
            }

            builder.CloseElement();
            BuildFooter(builder, ref seq);
            builder.CloseElement();
            builder.CloseElement();
        }

        private void BuildRangePanel(RenderTreeBuilder builder, ref int seq, bool left)
        {
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", left ? "el-time-range-picker__content is-left" : "el-time-range-picker__content is-right");
            builder.AddAttribute(seq++, "style", "flex:1;");
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "el-time-range-picker__header");
            builder.AddAttribute(seq++, "style", "padding:8px 12px;color:#606266;font-size:12px;");
            builder.AddContent(seq++, left ? StartPlaceholder : EndPlaceholder);
            builder.CloseElement();
            BuildTimePanel(builder, ref seq, left ? "start" : "end", left ? PendingStartTimeValue : PendingEndTimeValue, left ? SetRangeStartPendingPartAsync : SetRangeEndPendingPartAsync);
            builder.CloseElement();
        }

        private void BuildTimePanel(RenderTreeBuilder builder, ref int seq, string panel, TimeSpan value, Func<TimePart, int, Task> onSelect)
        {
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "el-time-spinner");
            builder.AddAttribute(seq++, "role", "group");
            builder.AddAttribute(seq++, "aria-label", panel);
            builder.AddAttribute(seq++, "style", "display:flex;");
            BuildSpinnerColumn(builder, ref seq, panel, TimePart.Hour, 24, value.Hours, onSelect);
            if (ShowMinutes)
            {
                BuildSpinnerColumn(builder, ref seq, panel, TimePart.Minute, 60, value.Minutes, onSelect);
            }
            if (ShowSeconds)
            {
                BuildSpinnerColumn(builder, ref seq, panel, TimePart.Second, 60, value.Seconds, onSelect);
            }
            builder.CloseElement();
        }

        private void BuildSpinnerColumn(RenderTreeBuilder builder, ref int seq, string panel, TimePart part, int count, int selectedValue, Func<TimePart, int, Task> onSelect)
        {
            builder.OpenElement(seq++, "ul");
            builder.AddAttribute(seq++, "class", "el-time-spinner__wrapper el-scrollbar__wrap");
            builder.AddAttribute(seq++, "role", "listbox");
            builder.AddAttribute(seq++, "aria-label", part.ToString());
            builder.AddAttribute(seq++, "style", "list-style:none;margin:0;padding:0;max-height:192px;overflow:auto;flex:1;");
            for (var value = 0; value < count; value++)
            {
                var current = value;
                var itemClass = HtmlPropertyBuilder.CreateCssClassBuilder()
                    .Add("el-time-spinner__item")
                    .AddIf(current == selectedValue, "active")
                    .ToString();
                builder.OpenElement(seq++, "li");
                builder.AddAttribute(seq++, "class", itemClass);
                builder.AddAttribute(seq++, "role", "option");
                builder.AddAttribute(seq++, "aria-selected", current == selectedValue);
                builder.AddAttribute(seq++, "data-time-panel", panel);
                builder.AddAttribute(seq++, "data-time-part", GetPartName(part));
                builder.AddAttribute(seq++, "data-time-value", current);
                builder.AddAttribute(seq++, "style", "height:32px;line-height:32px;text-align:center;cursor:pointer;");
                builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => onSelect(part, current)));
                builder.AddContent(seq++, current.ToString("00", CultureInfo.InvariantCulture));
                builder.CloseElement();
            }
            builder.CloseElement();
        }

        private void BuildFooter(RenderTreeBuilder builder, ref int seq)
        {
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "el-time-panel__footer");
            builder.OpenElement(seq++, "button");
            builder.AddAttribute(seq++, "type", "button");
            builder.AddAttribute(seq++, "class", "el-time-panel__btn cancel");
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, CloseDropDownAsync));
            builder.AddContent(seq++, "取消");
            builder.CloseElement();
            builder.OpenElement(seq++, "button");
            builder.AddAttribute(seq++, "type", "button");
            builder.AddAttribute(seq++, "class", "el-time-panel__btn confirm");
            builder.AddAttribute(seq++, "disabled", !CanConfirmPending);
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ConfirmPendingAsync));
            builder.AddContent(seq++, "确认");
            builder.CloseElement();
            builder.CloseElement();
        }

        private Task SetSinglePendingPartAsync(TimePart part, int value)
        {
            pendingSingleValue = UpdatePart(PendingSingleTimeValue, part, value);
            return RefreshPendingAsync();
        }

        private Task SetRangeStartPendingPartAsync(TimePart part, int value)
        {
            pendingRangeStart = UpdatePart(PendingStartTimeValue, part, value);
            return RefreshPendingAsync();
        }

        private Task SetRangeEndPendingPartAsync(TimePart part, int value)
        {
            pendingRangeEnd = UpdatePart(PendingEndTimeValue, part, value);
            return RefreshPendingAsync();
        }

        private Task RefreshPendingAsync()
        {
            dropDownOption?.RequestRender?.Invoke();
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task ConfirmPendingAsync()
        {
            if (IsRangeMode)
            {
                if (!pendingRangeStart.HasValue || !pendingRangeEnd.HasValue)
                {
                    return;
                }

                await CommitRangeValueAsync(NormalizeRange(new[] { pendingRangeStart, pendingRangeEnd }), ValidateEvent, notifyChange: true, close: true);
                return;
            }

            if (!pendingSingleValue.HasValue)
            {
                return;
            }

            await CommitSingleValueAsync(pendingSingleValue, ValidateEvent, notifyChange: true, close: true);
        }

        private async Task CommitSingleValueAsync(TimeSpan? value, bool validate, bool notifyChange, bool close)
        {
            Value = NormalizeTime(value);
            StringValue = FormatBindingValue(Value);
            SetFieldValue(Value, validate);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
            if (TimeChanged.HasDelegate)
            {
                await TimeChanged.InvokeAsync(Value);
            }
            if (StringValueChanged.HasDelegate)
            {
                await StringValueChanged.InvokeAsync(StringValue);
            }
            if (notifyChange && OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
            if (close)
            {
                await CloseDropDownAsync();
            }
        }

        private async Task CommitRangeValueAsync(IList<TimeSpan?> range, bool validate, bool notifyChange, bool close)
        {
            RangeValue = NormalizeRange(range);
            await NotifyRangeValueChangedAsync(validate, notifyChange);
            if (close)
            {
                await CloseDropDownAsync();
            }
        }

        private async Task NotifyRangeValueChangedAsync(bool validate, bool notifyChange)
        {
            StringRangeValue = RangeValue.Select(FormatBindingValue).ToList();
            SetFieldValue(RangeStart, validate);
            if (RangeValueChanged.HasDelegate)
            {
                await RangeValueChanged.InvokeAsync(RangeValue);
            }
            if (ModelRangeValueChanged.HasDelegate)
            {
                await ModelRangeValueChanged.InvokeAsync(RangeValue);
            }
            if (StringRangeValueChanged.HasDelegate)
            {
                await StringRangeValueChanged.InvokeAsync(StringRangeValue);
            }
            if (notifyChange && OnRangeChange.HasDelegate)
            {
                await OnRangeChange.InvokeAsync(RangeValue);
            }
        }

        public ValueTask FocusAsync()
        {
            return inputElement.Dom(JSRuntime).FocusAsync();
        }

        public ValueTask BlurAsync()
        {
            return inputElement.Dom(JSRuntime).BlurAsync();
        }

        public Task ShowAsync()
        {
            return OpenDropDownAsync();
        }

        public Task HideAsync()
        {
            return CloseDropDownAsync();
        }

        public Task ClearAsync()
        {
            return OnClearClickAsync(null);
        }

        public override void Dispose()
        {
            if (dropDownOption != null)
            {
                PopupService.SelectDropDownOptions.Remove(dropDownOption);
            }
            base.Dispose();
        }

        private void InitializePendingValues()
        {
            pendingSingleValue = Value ?? DefaultValue ?? TimeSpan.Zero;
            pendingRangeStart = RangeStart ?? DefaultStartTime ?? DefaultValue ?? TimeSpan.Zero;
            pendingRangeEnd = RangeEnd ?? DefaultEndTime ?? DefaultValue ?? TimeSpan.Zero;
        }

        private void ClearPendingValues()
        {
            pendingSingleValue = null;
            pendingRangeStart = null;
            pendingRangeEnd = null;
        }

        private void SyncValueFromFormattedParameters()
        {
            if (!string.IsNullOrWhiteSpace(ValueFormat) && !Value.HasValue && !string.IsNullOrWhiteSpace(StringValue) && TryParseTime(StringValue, ValueFormat, out var parsed))
            {
                Value = parsed;
            }
            if (!string.IsNullOrWhiteSpace(ValueFormat) && IsRangeMode && !HasAnyRangeValue && StringRangeValue?.Count >= 2)
            {
                var parsedValues = StringRangeValue.Take(2)
                    .Select(x => TryParseTime(x, ValueFormat, out var parsedTime) ? (TimeSpan?)parsedTime : null)
                    .ToList();
                if (parsedValues.Any(x => x.HasValue))
                {
                    RangeValue = NormalizeRange(parsedValues);
                }
            }
        }

        private IList<TimeSpan?> NormalizeRange(IEnumerable<TimeSpan?> values)
        {
            var range = values?.Take(2).Select(NormalizeTime).ToList() ?? new List<TimeSpan?>();
            while (range.Count < 2)
            {
                range.Add(null);
            }
            if (range[0].HasValue && range[1].HasValue && range[0] > range[1])
            {
                return new List<TimeSpan?> { range[1], range[0] };
            }
            return range;
        }

        private static TimeSpan? NormalizeTime(TimeSpan? value)
        {
            if (!value.HasValue)
            {
                return null;
            }

            var ticks = value.Value.Ticks % TimeSpan.FromDays(1).Ticks;
            if (ticks < 0)
            {
                ticks += TimeSpan.FromDays(1).Ticks;
            }
            return TimeSpan.FromTicks(ticks);
        }

        private static TimeSpan UpdatePart(TimeSpan source, TimePart part, int value)
        {
            var hour = source.Hours;
            var minute = source.Minutes;
            var second = source.Seconds;
            switch (part)
            {
                case TimePart.Hour:
                    hour = value;
                    break;
                case TimePart.Minute:
                    minute = value;
                    break;
                case TimePart.Second:
                    second = value;
                    break;
            }

            return new TimeSpan(hour, minute, second);
        }

        private string FormatDisplayValue(TimeSpan? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return FormatTime(value.Value, Format);
        }

        private string FormatBindingValue(TimeSpan? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return string.IsNullOrWhiteSpace(ValueFormat) ? null : FormatTime(value.Value, ValueFormat);
        }

        private static string FormatTime(TimeSpan value, string format)
        {
            var normalized = NormalizeTime(value) ?? TimeSpan.Zero;
            return DateTime.Today.Add(normalized).ToString(ToDotNetTimeFormat(format), CultureInfo.CurrentCulture);
        }

        private static bool TryParseTime(string value, string format, out TimeSpan result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (DateTime.TryParseExact(value, ToDotNetTimeFormat(format), CultureInfo.CurrentCulture, DateTimeStyles.None, out var exact))
            {
                result = exact.TimeOfDay;
                return true;
            }
            if (TimeSpan.TryParse(value, CultureInfo.CurrentCulture, out result)
                || TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out result))
            {
                result = NormalizeTime(result) ?? TimeSpan.Zero;
                return true;
            }
            if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var current)
                || DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out current))
            {
                result = current.TimeOfDay;
                return true;
            }
            return false;
        }

        private static TimeSpan? ConvertToTime(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is TimeSpan time)
            {
                return NormalizeTime(time);
            }
            if (value is DateTime dateTime)
            {
                return dateTime.TimeOfDay;
            }
            return TryParseTime(Convert.ToString(value, CultureInfo.CurrentCulture), "HH:mm:ss", out var parsed)
                ? parsed
                : null;
        }

        private static string ToDotNetTimeFormat(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return "HH:mm:ss";
            }

            return format
                .Replace("A", "tt", StringComparison.Ordinal)
                .Replace("a", "tt", StringComparison.Ordinal);
        }

        private static string GetPartName(TimePart part) => part switch
        {
            TimePart.Hour => "hour",
            TimePart.Minute => "minute",
            _ => "second"
        };

        private bool IsRangeMode => IsRange || Type == TimePickerType.TimeRange;

        private bool HasAnyRangeValue => RangeValue?.Any(x => x.HasValue) == true;

        private TimeSpan? RangeStart => RangeValue?.ElementAtOrDefault(0);

        private TimeSpan? RangeEnd => RangeValue?.ElementAtOrDefault(1);

        private string StartDisplayValue => FormatDisplayValue(RangeStart);

        private string EndDisplayValue => FormatDisplayValue(RangeEnd);

        private string SingleDisplayValue => FormatDisplayValue(Value);

        private TimeSpan PendingSingleTimeValue => pendingSingleValue ?? Value ?? DefaultValue ?? TimeSpan.Zero;

        private TimeSpan PendingStartTimeValue => pendingRangeStart ?? RangeStart ?? DefaultStartTime ?? DefaultValue ?? TimeSpan.Zero;

        private TimeSpan PendingEndTimeValue => pendingRangeEnd ?? RangeEnd ?? DefaultEndTime ?? DefaultValue ?? TimeSpan.Zero;

        private bool ShowMinutes => FormatIncludes("m");

        private bool ShowSeconds => FormatIncludes("s");

        private bool FormatIncludes(string token)
        {
            return (!string.IsNullOrWhiteSpace(Format) && Format.Contains(token, StringComparison.Ordinal))
                || (!string.IsNullOrWhiteSpace(ValueFormat) && ValueFormat.Contains(token, StringComparison.Ordinal));
        }

        private string DropDownPanelClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add(IsRangeMode ? "el-time-panel el-time-range-picker" : "el-time-panel")
            .ToString();

        private string SingleWrapperClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-input", "el-date-editor", "el-input--prefix", "el-input--suffix", "el-date-editor--time", Cls)
            .AddIf(SizeCssValue != null, $"el-input--{SizeCssValue}")
            .AddIf(effectiveDisabled, "is-disabled")
            .AddIf(isFocus || IsDropDownOpen, "is-focus")
            .ToString();

        private string RangeWrapperClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-date-editor", "el-range-editor", "el-input__inner", "el-date-editor--timerange", Cls)
            .AddIf(SizeCssValue != null, $"el-range-editor--{SizeCssValue}")
            .AddIf(effectiveDisabled, "is-disabled")
            .AddIf(isFocus || IsDropDownOpen, "is-active")
            .ToString();

        private string SizeCssValue => effectiveSize switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };

        private string SuffixIconClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-input__icon")
            .Add(Clearable && HasValue && isHovering && !effectiveDisabled && !Readonly ? ClearIcon : "el-icon-arrow-up")
            .AddIf(IsDropDownOpen, "is-reverse")
            .ToString();

        private string RangeCloseIconClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-input__icon", "el-range__close-icon")
            .Add(Clearable && HasValue && isHovering && !effectiveDisabled && !Readonly ? ClearIcon : string.Empty)
            .ToString();

        private bool HasValue => IsRangeMode ? HasAnyRangeValue : Value.HasValue;

        private bool IsDropDownOpen => dropDownOption != null && dropDownOption.IsShow;

        private bool IsInputReadonly => Readonly || !Editable;

        private bool CanConfirmPending => IsRangeMode ? pendingRangeStart.HasValue && pendingRangeEnd.HasValue : pendingSingleValue.HasValue;

        private enum TimePart
        {
            Hour,
            Minute,
            Second
        }
    }
}
