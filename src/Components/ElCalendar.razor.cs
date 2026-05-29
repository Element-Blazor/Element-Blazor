using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElCalendar : ElementComponentBase
    {
        private static readonly string[] DefaultWeekDays = { "日", "一", "二", "三", "四", "五", "六" };

        [Parameter]
        public DateTime? Value { get; set; }

        [Parameter]
        public EventCallback<DateTime?> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<DateTime> OnPick { get; set; }

        [Parameter]
        public RenderFragment<DateTime> Header { get; set; }

        [Parameter]
        public RenderFragment<CalendarDateContext> DateCell { get; set; }

        [Parameter]
        public string[] WeekDays { get; set; } = DefaultWeekDays;

        [Parameter]
        public CultureInfo Culture { get; set; }

        protected DateTime ViewDate { get; set; }

        protected DateTime SelectedDate => (Value ?? DateTime.Today).Date;

        protected string Title => ViewDate.ToString("yyyy 年 M 月", Culture ?? CultureInfo.CurrentCulture);

        protected IReadOnlyList<IReadOnlyList<CalendarDateContext>> Weeks
        {
            get
            {
                var firstOfMonth = new DateTime(ViewDate.Year, ViewDate.Month, 1);
                var start = firstOfMonth.AddDays(-(int)firstOfMonth.DayOfWeek);
                var days = Enumerable.Range(0, 42)
                    .Select(i =>
                    {
                        var date = start.AddDays(i);
                        return new CalendarDateContext
                        {
                            Date = date,
                            IsCurrentMonth = date.Month == ViewDate.Month,
                            IsSelected = date.Date == SelectedDate,
                            IsToday = date.Date == DateTime.Today,
                            Type = date.Month == ViewDate.Month ? "current" : date < firstOfMonth ? "prev" : "next"
                        };
                    })
                    .ToList();

                return Enumerable.Range(0, 6)
                    .Select(i => (IReadOnlyList<CalendarDateContext>)days.Skip(i * 7).Take(7).ToList())
                    .ToList();
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            var value = Value ?? DateTime.Today;
            if (ViewDate == default)
            {
                ViewDate = new DateTime(value.Year, value.Month, 1);
            }
        }

        protected string CalendarClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-calendar", Cls)
            .ToString();

        protected string GetCellClass(CalendarDateContext context)
        {
            return HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add(context.Type)
                .AddIf(context.IsSelected, "is-selected")
                .AddIf(context.IsToday, "is-today")
                .ToString();
        }

        protected async Task SelectDateAsync(DateTime date)
        {
            Value = date.Date;
            ViewDate = new DateTime(date.Year, date.Month, 1);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            if (OnPick.HasDelegate)
            {
                await OnPick.InvokeAsync(date.Date);
            }
        }

        protected Task SelectPreviousMonthAsync()
        {
            ViewDate = ViewDate.AddMonths(-1);
            return Task.CompletedTask;
        }

        protected async Task SelectTodayAsync()
        {
            await SelectDateAsync(DateTime.Today);
        }

        protected Task SelectNextMonthAsync()
        {
            ViewDate = ViewDate.AddMonths(1);
            return Task.CompletedTask;
        }
    }
}
