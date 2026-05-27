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
    public partial class ElDateTimePicker
    {
        private static long dropDownIdSeed;
        private readonly string DropDownId = $"el-date-time-picker-dropdown-{Interlocked.Increment(ref dropDownIdSeed)}";
        private DropDownOption dropDownOption;
        private ElementReference pickerElement;
        private ElementReference inputElement;
        private bool effectiveDisabled;
        private InputSize effectiveSize = InputSize.Normal;
        private bool isFocus;
        private bool isHovering;
        private DateTime? pendingSingleValue;
        private DateTime? pendingRangeStart;
        private DateTime? pendingRangeEnd;

        [Inject]
        internal PopupService PopupService { get; set; }

        [Parameter]
        public DateTime? Value { get; set; }

        [Parameter]
        public DateTime? ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<DateTime?> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<DateTime?> ModelValueChanged { get; set; }

        [Parameter]
        public DateTime? Date
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<DateTime?> DateChanged { get; set; }

        [Parameter]
        public IList<DateTime?> RangeValue { get; set; } = new List<DateTime?>();

        [Parameter]
        public IList<DateTime?> ModelRangeValue
        {
            get => RangeValue;
            set => RangeValue = value;
        }

        [Parameter]
        public EventCallback<IList<DateTime?>> RangeValueChanged { get; set; }

        [Parameter]
        public EventCallback<IList<DateTime?>> ModelRangeValueChanged { get; set; }

        [Parameter]
        public string StringValue { get; set; }

        [Parameter]
        public EventCallback<string> StringValueChanged { get; set; }

        [Parameter]
        public IList<string> StringRangeValue { get; set; } = new List<string>();

        [Parameter]
        public EventCallback<IList<string>> StringRangeValueChanged { get; set; }

        [Parameter]
        public DateTimePickerType Type { get; set; } = DateTimePickerType.DateTime;

        [Parameter]
        public string Format { get; set; } = "yyyy-MM-dd HH:mm:ss";

        [Parameter]
        public string ValueFormat { get; set; }

        [Parameter]
        public DateTime? DefaultValue { get; set; }

        [Parameter]
        public TimeSpan? DefaultTime { get; set; }

        [Parameter]
        public TimeSpan? DefaultStartTime { get; set; }

        [Parameter]
        public TimeSpan? DefaultEndTime { get; set; }

        [Parameter]
        public string Placeholder { get; set; } = "请选择日期时间";

        [Parameter]
        public string StartPlaceholder { get; set; } = "开始日期时间";

        [Parameter]
        public string EndPlaceholder { get; set; } = "结束日期时间";

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
        public int PopperMaxHeight { get; set; } = 460;

        [Parameter]
        public bool SinglePanel { get; set; }

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public Func<DateTime, bool> DisabledDate { get; set; }

        [Parameter]
        public IEnumerable<DateTimePickerShortcut> Shortcuts { get; set; }

        [Parameter]
        public EventCallback<DateTime?> OnChange { get; set; }

        [Parameter]
        public EventCallback<IList<DateTime?>> OnRangeChange { get; set; }

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
                    Value = ConvertToDateTime(FormItem.OriginValue);
                }
                SetFieldValue(Value, false);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = ConvertToDateTime(value);
            ClearPendingValues();
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                _ = ModelValueChanged.InvokeAsync(Value);
            }
            if (DateChanged.HasDelegate)
            {
                _ = DateChanged.InvokeAsync(Value);
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
            if (!TryParseDateTime(text, Format, out var parsed))
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
            if (!TryParseDateTime(startText, Format, out var start) || !TryParseDateTime(endText, Format, out var end))
            {
                return;
            }

            await CommitRangeValueAsync(NormalizeRange(new DateTime?[] { start, end }), ValidateEvent, notifyChange: true, close: false);
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

            if (IsRange)
            {
                await CommitRangeValueAsync(new List<DateTime?>(), ValidateEvent, notifyChange: true, close: true);
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
                PopperClass = string.Join(" ", new[] { "el-date-picker__dropdown", "el-date-time-picker__dropdown", PopperClass }.Where(x => !string.IsNullOrWhiteSpace(x))),
                PopperStyle = PopperStyle,
                MaxHeight = PopperMaxHeight,
                FitInputWidth = false,
                AutoWidth = false,
                Width = IsRange && !SinglePanel ? 646 : 322,
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
            if (ShortcutList.Any())
            {
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "class", "el-picker-panel__sidebar");
                foreach (var shortcut in ShortcutList)
                {
                    builder.OpenElement(seq++, "button");
                    builder.AddAttribute(seq++, "type", "button");
                    builder.AddAttribute(seq++, "class", "el-picker-panel__shortcut");
                    builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => SelectShortcutAsync(shortcut)));
                    builder.AddContent(seq++, shortcut.Text);
                    builder.CloseElement();
                }
                builder.CloseElement();
            }

            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", ShortcutList.Any() ? "el-picker-panel__body" : "el-picker-panel__body-wrapper");
            if (IsRange && !SinglePanel)
            {
                BuildRangePanel(builder, ref seq, true);
                BuildRangePanel(builder, ref seq, false);
            }
            else
            {
                BuildSinglePanel(builder, ref seq);
            }
            builder.CloseElement();
            BuildFooter(builder, ref seq);
            builder.CloseElement();
            builder.CloseElement();
        }

        private void BuildSinglePanel(RenderTreeBuilder builder, ref int seq)
        {
            builder.OpenComponent<ElDatePickerPanel>(seq++);
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.Value), IsRange ? pendingRangeStart ?? RangeStart : pendingSingleValue ?? Value);
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.Type), DatePickerPanelType.Date);
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.DefaultValue), DefaultValue ?? pendingSingleValue ?? Value ?? RangeStart ?? DateTime.Today);
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.DisabledDate), DisabledDate);
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.Border), false);
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.ValidateEvent), false);
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.OnPick), EventCallback.Factory.Create<DatePickerPanelSelection>(this, OnPanelPickAsync));
            builder.CloseComponent();
        }

        private void BuildRangePanel(RenderTreeBuilder builder, ref int seq, bool left)
        {
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", left ? "el-date-range-picker__content is-left" : "el-date-range-picker__content is-right");
            builder.OpenComponent<ElDatePickerPanel>(seq++);
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.Value), left ? pendingRangeStart ?? RangeStart : pendingRangeEnd ?? RangeEnd);
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.Type), DatePickerPanelType.Date);
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.DefaultValue), GetRangePanelDefaultValue(left));
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.DisabledDate), DisabledDate);
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.Border), false);
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.ValidateEvent), false);
            builder.AddAttribute(seq++, nameof(ElDatePickerPanel.OnPick), EventCallback.Factory.Create<DatePickerPanelSelection>(this, OnPanelPickAsync));
            builder.CloseComponent();
            builder.CloseElement();
        }

        private void BuildFooter(RenderTreeBuilder builder, ref int seq)
        {
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "el-picker-panel__footer el-date-time-picker__footer");
            builder.AddAttribute(seq++, "style", "display:flex;align-items:center;justify-content:space-between;gap:8px;");
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "el-date-time-picker__time-wrapper");
            builder.AddAttribute(seq++, "style", "display:flex;align-items:center;gap:8px;flex-wrap:wrap;");
            if (IsRange)
            {
                BuildTimeInput(builder, ref seq, "开始", PendingStartTimeValue, "el-date-time-picker__start-time", OnRangeStartTimeChangedAsync);
                BuildTimeInput(builder, ref seq, "结束", PendingEndTimeValue, "el-date-time-picker__end-time", OnRangeEndTimeChangedAsync);
            }
            else
            {
                BuildTimeInput(builder, ref seq, "时间", PendingSingleTimeValue, "el-date-time-picker__time-input", OnSingleTimeChangedAsync);
            }
            builder.CloseElement();
            builder.OpenElement(seq++, "button");
            builder.AddAttribute(seq++, "type", "button");
            builder.AddAttribute(seq++, "class", "el-button el-button--text el-picker-panel__link-btn el-date-time-picker__confirm");
            builder.AddAttribute(seq++, "disabled", !CanConfirmPending);
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ConfirmPendingAsync));
            builder.OpenElement(seq++, "span");
            builder.AddContent(seq++, "确认");
            builder.CloseElement();
            builder.CloseElement();
            builder.CloseElement();
        }

        private void BuildTimeInput(RenderTreeBuilder builder, ref int seq, string label, string value, string cssClass, Func<ChangeEventArgs, Task> onChange)
        {
            builder.OpenElement(seq++, "label");
            builder.AddAttribute(seq++, "class", "el-date-time-picker__time-field");
            builder.AddAttribute(seq++, "style", "display:inline-flex;align-items:center;gap:6px;color:#606266;");
            builder.OpenElement(seq++, "span");
            builder.AddAttribute(seq++, "style", "font-size:12px;");
            builder.AddContent(seq++, label);
            builder.CloseElement();
            builder.OpenElement(seq++, "input");
            builder.AddAttribute(seq++, "class", $"el-input__inner {cssClass}");
            builder.AddAttribute(seq++, "style", "width:112px;height:28px;line-height:28px;padding:0 8px;");
            builder.AddAttribute(seq++, "type", "time");
            builder.AddAttribute(seq++, "step", "1");
            builder.AddAttribute(seq++, "value", value);
            builder.AddAttribute(seq++, "disabled", effectiveDisabled);
            builder.AddAttribute(seq++, "onchange", EventCallback.Factory.Create(this, onChange));
            builder.CloseElement();
            builder.CloseElement();
        }

        private Task OnPanelPickAsync(DatePickerPanelSelection selection)
        {
            if (selection == null)
            {
                return Task.CompletedTask;
            }

            if (!IsRange)
            {
                pendingSingleValue = CombineDateAndTime(selection.Value, pendingSingleValue?.TimeOfDay ?? Value?.TimeOfDay ?? DefaultTime ?? TimeSpan.Zero);
                StateHasChanged();
                return Task.CompletedTask;
            }

            var pickedDate = selection.Value.Date;
            if (!pendingRangeStart.HasValue || HasCompletePendingRange)
            {
                pendingRangeStart = CombineDateAndTime(pickedDate, pendingRangeStart?.TimeOfDay ?? RangeStart?.TimeOfDay ?? DefaultStartTime ?? TimeSpan.Zero);
                pendingRangeEnd = null;
                StateHasChanged();
                return Task.CompletedTask;
            }

            var startDate = pendingRangeStart.Value.Date;
            var startTime = pendingRangeStart.Value.TimeOfDay;
            var endTime = pendingRangeEnd?.TimeOfDay ?? RangeEnd?.TimeOfDay ?? DefaultEndTime ?? TimeSpan.Zero;
            if (pickedDate < startDate)
            {
                pendingRangeEnd = CombineDateAndTime(startDate, endTime);
                pendingRangeStart = CombineDateAndTime(pickedDate, startTime);
            }
            else
            {
                pendingRangeEnd = CombineDateAndTime(pickedDate, endTime);
            }
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task OnSingleTimeChangedAsync(ChangeEventArgs e)
        {
            if (TryParseTime(Convert.ToString(e.Value, CultureInfo.CurrentCulture), out var time))
            {
                pendingSingleValue = CombineDateAndTime((pendingSingleValue ?? Value ?? DefaultValue ?? DateTime.Today).Date, time);
                await RefreshPendingAsync();
            }
        }

        private async Task OnRangeStartTimeChangedAsync(ChangeEventArgs e)
        {
            if (TryParseTime(Convert.ToString(e.Value, CultureInfo.CurrentCulture), out var time))
            {
                pendingRangeStart = CombineDateAndTime((pendingRangeStart ?? RangeStart ?? DefaultValue ?? DateTime.Today).Date, time);
                await RefreshPendingAsync();
            }
        }

        private async Task OnRangeEndTimeChangedAsync(ChangeEventArgs e)
        {
            if (TryParseTime(Convert.ToString(e.Value, CultureInfo.CurrentCulture), out var time))
            {
                pendingRangeEnd = CombineDateAndTime((pendingRangeEnd ?? RangeEnd ?? DefaultValue ?? pendingRangeStart ?? DateTime.Today).Date, time);
                await RefreshPendingAsync();
            }
        }

        private Task RefreshPendingAsync()
        {
            dropDownOption?.RequestRender?.Invoke();
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task ConfirmPendingAsync()
        {
            if (IsRange)
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

        private async Task SelectShortcutAsync(DateTimePickerShortcut shortcut)
        {
            if (shortcut == null)
            {
                return;
            }

            if (IsRange)
            {
                await CommitRangeValueAsync(NormalizeRange(shortcut.ResolveRangeValue()), ValidateEvent, notifyChange: true, close: true);
                return;
            }

            await CommitSingleValueAsync(shortcut.ResolveValue(), ValidateEvent, notifyChange: true, close: true);
        }

        private async Task CommitSingleValueAsync(DateTime? value, bool validate, bool notifyChange, bool close)
        {
            Value = value;
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
            if (DateChanged.HasDelegate)
            {
                await DateChanged.InvokeAsync(Value);
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

        private async Task CommitRangeValueAsync(IList<DateTime?> range, bool validate, bool notifyChange, bool close)
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
            var strings = RangeValue.Select(FormatBindingValue).ToList();
            StringRangeValue = strings;
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
            pendingSingleValue = Value;
            pendingRangeStart = RangeStart;
            pendingRangeEnd = RangeEnd;
        }

        private void ClearPendingValues()
        {
            pendingSingleValue = null;
            pendingRangeStart = null;
            pendingRangeEnd = null;
        }

        private void SyncValueFromFormattedParameters()
        {
            if (!string.IsNullOrWhiteSpace(ValueFormat) && !Value.HasValue && !string.IsNullOrWhiteSpace(StringValue) && TryParseDateTime(StringValue, ValueFormat, out var parsed))
            {
                Value = parsed;
            }
            if (!string.IsNullOrWhiteSpace(ValueFormat) && IsRange && !HasAnyRangeValue && StringRangeValue?.Count >= 2)
            {
                var parsedValues = StringRangeValue.Take(2)
                    .Select(x => TryParseDateTime(x, ValueFormat, out var parsedDate) ? (DateTime?)parsedDate : null)
                    .ToList();
                if (parsedValues.Any(x => x.HasValue))
                {
                    RangeValue = NormalizeRange(parsedValues);
                }
            }
        }

        private IList<DateTime?> NormalizeRange(IEnumerable<DateTime?> values)
        {
            var range = values?.Take(2).ToList() ?? new List<DateTime?>();
            while (range.Count < 2)
            {
                range.Add(null);
            }
            if (range[0].HasValue && range[1].HasValue && range[0] > range[1])
            {
                return new List<DateTime?> { range[1], range[0] };
            }
            return range;
        }

        private DateTime? GetRangePanelDefaultValue(bool left)
        {
            var fallback = DefaultValue ?? pendingRangeStart ?? RangeStart ?? Value ?? DateTime.Today;
            if (left)
            {
                return fallback;
            }
            return fallback.AddMonths(1);
        }

        private string FormatDisplayValue(DateTime? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return FormatDateTime(value.Value, Format);
        }

        private string FormatBindingValue(DateTime? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return string.IsNullOrWhiteSpace(ValueFormat) ? null : FormatDateTime(value.Value, ValueFormat);
        }

        private string FormatDateTime(DateTime value, string format)
        {
            if (string.Equals(format, "x", StringComparison.Ordinal))
            {
                return new DateTimeOffset(value).ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture);
            }
            if (string.Equals(format, "X", StringComparison.Ordinal))
            {
                return new DateTimeOffset(value).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            }
            return value.ToString(ToDotNetDateFormat(format), CultureInfo.CurrentCulture);
        }

        private bool TryParseDateTime(string value, string format, out DateTime result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            if (string.Equals(format, "x", StringComparison.Ordinal) && long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var milliseconds))
            {
                result = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).LocalDateTime;
                return true;
            }
            if (string.Equals(format, "X", StringComparison.Ordinal) && long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var seconds))
            {
                result = DateTimeOffset.FromUnixTimeSeconds(seconds).LocalDateTime;
                return true;
            }

            return DateTime.TryParseExact(value, ToDotNetDateFormat(format), CultureInfo.CurrentCulture, DateTimeStyles.None, out result)
                || DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out result)
                || DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

        private static bool TryParseTime(string value, out TimeSpan result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            return TimeSpan.TryParse(value, CultureInfo.CurrentCulture, out result)
                || TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out result);
        }

        private static DateTime CombineDateAndTime(DateTime date, TimeSpan time)
        {
            return date.Date.Add(time);
        }

        private static DateTime? ConvertToDateTime(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is DateTime dateTime)
            {
                return dateTime;
            }
            if (DateTime.TryParse(Convert.ToString(value, CultureInfo.CurrentCulture), CultureInfo.CurrentCulture, DateTimeStyles.None, out var current))
            {
                return current;
            }
            if (DateTime.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.None, out var invariant))
            {
                return invariant;
            }
            return null;
        }

        private static string ToDotNetDateFormat(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return "yyyy-MM-dd HH:mm:ss";
            }
            return format
                .Replace("YYYY", "yyyy", StringComparison.Ordinal)
                .Replace("YY", "yy", StringComparison.Ordinal)
                .Replace("DD", "dd", StringComparison.Ordinal);
        }

        private bool IsRange => Type == DateTimePickerType.DateTimeRange;

        private bool HasAnyRangeValue => RangeValue?.Any(x => x.HasValue) == true;

        private bool HasCompletePendingRange => pendingRangeStart.HasValue && pendingRangeEnd.HasValue;

        private DateTime? RangeStart => RangeValue?.ElementAtOrDefault(0);

        private DateTime? RangeEnd => RangeValue?.ElementAtOrDefault(1);

        private string StartDisplayValue => FormatDisplayValue(RangeStart);

        private string EndDisplayValue => FormatDisplayValue(RangeEnd);

        private string SingleDisplayValue => FormatDisplayValue(Value);

        private string PendingSingleTimeValue => FormatTime(pendingSingleValue?.TimeOfDay ?? Value?.TimeOfDay ?? DefaultTime ?? TimeSpan.Zero);

        private string PendingStartTimeValue => FormatTime(pendingRangeStart?.TimeOfDay ?? RangeStart?.TimeOfDay ?? DefaultStartTime ?? TimeSpan.Zero);

        private string PendingEndTimeValue => FormatTime(pendingRangeEnd?.TimeOfDay ?? RangeEnd?.TimeOfDay ?? DefaultEndTime ?? TimeSpan.Zero);

        private static string FormatTime(TimeSpan value)
        {
            return value.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
        }

        private IReadOnlyList<DateTimePickerShortcut> ShortcutList => Shortcuts?.Where(x => x != null).ToList() ?? new List<DateTimePickerShortcut>();

        private string DropDownPanelClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add(IsRange && !SinglePanel ? "el-picker-panel el-date-range-picker el-date-time-range-picker" : "el-picker-panel el-date-picker el-date-time-picker")
            .AddIf(ShortcutList.Any(), "has-sidebar")
            .ToString();

        private string SingleWrapperClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-input", "el-date-editor", "el-input--prefix", "el-input--suffix", "el-date-editor--datetime", Cls)
            .AddIf(SizeCssValue != null, $"el-input--{SizeCssValue}")
            .AddIf(effectiveDisabled, "is-disabled")
            .AddIf(isFocus || IsDropDownOpen, "is-focus")
            .ToString();

        private string RangeWrapperClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-date-editor", "el-range-editor", "el-input__inner", "el-date-editor--datetimerange", Cls)
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

        private bool HasValue => IsRange ? HasAnyRangeValue : Value.HasValue;

        private bool IsDropDownOpen => dropDownOption != null && dropDownOption.IsShow;

        private bool IsInputReadonly => Readonly || !Editable;

        private bool CanConfirmPending => IsRange ? pendingRangeStart.HasValue && pendingRangeEnd.HasValue : pendingSingleValue.HasValue;
    }
}
