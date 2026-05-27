using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElDatePickerPanel : ElementFieldComponentBase<DateTime?>
    {
        private static readonly string[] WeekHeaders = new[] { "日", "一", "二", "三", "四", "五", "六" };
        private static readonly string[] MonthLabels = new[] { "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" };
        private DateTime displayDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        private DatePickerPanelType currentView;
        private bool effectiveDisabled;

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
        public DatePickerPanelType Type { get; set; } = DatePickerPanelType.Date;

        [Parameter]
        public DateTime? DefaultValue { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        [Parameter]
        public bool Border { get; set; } = true;

        [Parameter]
        public Func<DateTime, bool> DisabledDate { get; set; }

        [Parameter]
        public RenderFragment FooterContent { get; set; }

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public EventCallback<DateTime?> OnChange { get; set; }

        [Parameter]
        public EventCallback<DateTime> OnPanelChange { get; set; }

        [Parameter]
        public EventCallback<DatePickerPanelSelection> OnPick { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            effectiveDisabled = Disabled || (FormItem?.Form?.Disabled ?? false);
            currentView = currentView == default ? Type : currentView;

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    Value = ConvertToDateTime(FormItem.OriginValue);
                }
                SetFieldValue(Value, false);
            }

            displayDate = NormalizeDisplayDate(Value ?? DefaultValue ?? displayDate);
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = ConvertToDateTime(value);
            displayDate = NormalizeDisplayDate(Value ?? DateTime.Today);
            currentView = Type;
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

        private Task GoPrevMonthAsync()
        {
            return SetDisplayDateAsync(displayDate.AddMonths(-1));
        }

        private Task GoNextMonthAsync()
        {
            return SetDisplayDateAsync(displayDate.AddMonths(1));
        }

        private Task GoPrevYearAsync()
        {
            var years = CurrentView == DatePickerPanelType.Year ? -10 : -1;
            return SetDisplayDateAsync(displayDate.AddYears(years));
        }

        private Task GoNextYearAsync()
        {
            var years = CurrentView == DatePickerPanelType.Year ? 10 : 1;
            return SetDisplayDateAsync(displayDate.AddYears(years));
        }

        private void ShowYearView()
        {
            if (!effectiveDisabled)
            {
                currentView = DatePickerPanelType.Year;
            }
        }

        private void ShowMonthView()
        {
            if (!effectiveDisabled)
            {
                currentView = DatePickerPanelType.Month;
            }
        }

        private async Task SelectDateAsync(DateTime value)
        {
            if (effectiveDisabled || IsDisabledDate(value))
            {
                return;
            }

            await CommitValueAsync(value.Date, DatePickerPanelType.Date);
        }

        private async Task SelectMonthAsync(int month)
        {
            if (effectiveDisabled)
            {
                return;
            }

            var next = new DateTime(displayDate.Year, month, 1);
            if (Type == DatePickerPanelType.Month)
            {
                await CommitValueAsync(next, DatePickerPanelType.Month);
                return;
            }

            displayDate = next;
            currentView = DatePickerPanelType.Date;
            await NotifyPanelChangeAsync();
        }

        private async Task SelectYearAsync(int year)
        {
            if (effectiveDisabled)
            {
                return;
            }

            var next = new DateTime(year, displayDate.Month, 1);
            if (Type == DatePickerPanelType.Year)
            {
                await CommitValueAsync(next, DatePickerPanelType.Year);
                return;
            }

            displayDate = next;
            currentView = DatePickerPanelType.Month;
            await NotifyPanelChangeAsync();
        }

        private async Task CommitValueAsync(DateTime value, DatePickerPanelType selectionType)
        {
            Value = NormalizeValue(value, selectionType);
            displayDate = NormalizeDisplayDate(Value.Value);
            SetFieldValue(Value, ValidateEvent);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
            if (OnPick.HasDelegate)
            {
                await OnPick.InvokeAsync(new DatePickerPanelSelection(Value.Value, selectionType));
            }
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
        }

        private async Task SetDisplayDateAsync(DateTime value)
        {
            if (effectiveDisabled)
            {
                return;
            }

            displayDate = NormalizeDisplayDate(value);
            await NotifyPanelChangeAsync();
        }

        private async Task NotifyPanelChangeAsync()
        {
            if (OnPanelChange.HasDelegate)
            {
                await OnPanelChange.InvokeAsync(displayDate);
            }
        }

        private RenderFragment BuildDateTable()
        {
            return builder =>
            {
                var seq = 0;
                var firstDay = new DateTime(displayDate.Year, displayDate.Month, 1);
                var gridStart = firstDay.AddDays(-(int)firstDay.DayOfWeek);

                builder.OpenElement(seq++, "table");
                builder.AddAttribute(seq++, "cellspacing", "0");
                builder.AddAttribute(seq++, "cellpadding", "0");
                builder.AddAttribute(seq++, "class", "el-date-table");
                builder.OpenElement(seq++, "tbody");
                builder.OpenElement(seq++, "tr");
                foreach (var header in WeekHeaders)
                {
                    builder.OpenElement(seq++, "th");
                    builder.AddContent(seq++, header);
                    builder.CloseElement();
                }
                builder.CloseElement();

                for (var week = 0; week < 6; week++)
                {
                    builder.OpenElement(seq++, "tr");
                    builder.AddAttribute(seq++, "class", "el-date-table__row");
                    for (var day = 0; day < 7; day++)
                    {
                        var current = gridStart.AddDays(week * 7 + day);
                        builder.OpenElement(seq++, "td");
                        builder.AddAttribute(seq++, "class", GetDayClass(current));
                        builder.AddAttribute(seq++, "role", "gridcell");
                        builder.AddAttribute(seq++, "aria-selected", IsSelectedDate(current));
                        builder.AddAttribute(seq++, "aria-disabled", IsDisabledDate(current));
                        builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => SelectDateAsync(current)));
                        builder.OpenElement(seq++, "div");
                        builder.OpenElement(seq++, "span");
                        builder.AddContent(seq++, current.Day);
                        builder.CloseElement();
                        builder.CloseElement();
                        builder.CloseElement();
                    }
                    builder.CloseElement();
                }

                builder.CloseElement();
                builder.CloseElement();
            };
        }

        private RenderFragment BuildMonthTable()
        {
            return builder =>
            {
                var seq = 0;
                builder.OpenElement(seq++, "table");
                builder.AddAttribute(seq++, "class", "el-month-table");
                builder.OpenElement(seq++, "tbody");
                for (var row = 0; row < 3; row++)
                {
                    builder.OpenElement(seq++, "tr");
                    for (var col = 0; col < 4; col++)
                    {
                        var month = row * 4 + col + 1;
                        var current = new DateTime(displayDate.Year, month, 1);
                        builder.OpenElement(seq++, "td");
                        builder.AddAttribute(seq++, "class", GetMonthClass(month));
                        builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => SelectMonthAsync(month)));
                        builder.OpenElement(seq++, "div");
                        builder.OpenElement(seq++, "a");
                        builder.AddAttribute(seq++, "class", "cell");
                        builder.AddContent(seq++, MonthLabels[month - 1]);
                        builder.CloseElement();
                        builder.CloseElement();
                        builder.CloseElement();
                    }
                    builder.CloseElement();
                }
                builder.CloseElement();
                builder.CloseElement();
            };
        }

        private RenderFragment BuildYearTable()
        {
            return builder =>
            {
                var seq = 0;
                var start = YearRangeStart;
                builder.OpenElement(seq++, "table");
                builder.AddAttribute(seq++, "class", "el-year-table");
                builder.OpenElement(seq++, "tbody");
                for (var row = 0; row < 3; row++)
                {
                    builder.OpenElement(seq++, "tr");
                    for (var col = 0; col < 4; col++)
                    {
                        var index = row * 4 + col;
                        if (index >= 10)
                        {
                            builder.OpenElement(seq++, "td");
                            builder.CloseElement();
                            continue;
                        }

                        var year = start + index;
                        builder.OpenElement(seq++, "td");
                        builder.AddAttribute(seq++, "class", GetYearClass(year));
                        builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => SelectYearAsync(year)));
                        builder.OpenElement(seq++, "a");
                        builder.AddAttribute(seq++, "class", "cell");
                        builder.AddContent(seq++, year);
                        builder.CloseElement();
                        builder.CloseElement();
                    }
                    builder.CloseElement();
                }
                builder.CloseElement();
                builder.CloseElement();
            };
        }

        private string GetDayClass(DateTime value)
        {
            return HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add(value.Month == displayDate.Month ? "available" : value < new DateTime(displayDate.Year, displayDate.Month, 1) ? "prev-month" : "available next-month")
                .AddIf(value.Date == DateTime.Today, "today")
                .AddIf(IsSelectedDate(value), "current")
                .AddIf(IsDisabledDate(value), "disabled")
                .ToString();
        }

        private string GetMonthClass(int month)
        {
            return HtmlPropertyBuilder.CreateCssClassBuilder()
                .AddIf(Value.HasValue && Value.Value.Year == displayDate.Year && Value.Value.Month == month, "current")
                .AddIf(DateTime.Today.Year == displayDate.Year && DateTime.Today.Month == month, "today")
                .ToString();
        }

        private string GetYearClass(int year)
        {
            return HtmlPropertyBuilder.CreateCssClassBuilder()
                .AddIf(Value.HasValue && Value.Value.Year == year, "current")
                .AddIf(DateTime.Today.Year == year, "today")
                .ToString();
        }

        private bool IsSelectedDate(DateTime value)
        {
            return Value.HasValue && Value.Value.Date == value.Date;
        }

        private bool IsDisabledDate(DateTime value)
        {
            return DisabledDate?.Invoke(value.Date) == true;
        }

        private DateTime NormalizeValue(DateTime value, DatePickerPanelType selectionType)
        {
            return selectionType switch
            {
                DatePickerPanelType.Year => new DateTime(value.Year, 1, 1),
                DatePickerPanelType.Month => new DateTime(value.Year, value.Month, 1),
                _ => value.Date
            };
        }

        private DateTime NormalizeDisplayDate(DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
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

        private string PanelClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-picker-panel", "el-date-picker", "el-date-picker-panel", Cls)
            .AddIf(!Border, "is-borderless")
            .AddIf(effectiveDisabled, "is-disabled")
            .ToString();

        private string HeaderClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-date-picker__header")
            .AddIf(CurrentView != DatePickerPanelType.Date, "el-date-picker__header--bordered")
            .ToString();

        private DatePickerPanelType CurrentView => currentView;

        private int DisplayedYear => displayDate.Year;

        private int DisplayedMonth => displayDate.Month;

        private int YearRangeStart => displayDate.Year / 10 * 10;

        private string YearRangeText => $"{YearRangeStart} - {YearRangeStart + 9}";

        private bool IsDatePickerPanelDisabled => effectiveDisabled;
    }
}
