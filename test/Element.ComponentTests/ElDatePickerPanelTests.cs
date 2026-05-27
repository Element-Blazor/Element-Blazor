using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Element.ComponentTests
{
    public class ElDatePickerPanelTests : BunitContext
    {
        public ElDatePickerPanelTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void RendersSelectedDateAndCurrentMonth()
        {
            var cut = Render<ElDatePickerPanel>(parameters => parameters
                .Add(x => x.Value, new DateTime(2026, 5, 27)));

            Assert.Contains("2026 年", cut.Markup);
            Assert.Contains("5 月", cut.Markup);
            Assert.Single(cut.FindAll("td.current"));
            Assert.Equal("27", cut.Find("td.current span").TextContent.Trim());
        }

        [Fact]
        public void SelectingDateUpdatesBoundValueAndChangeEvent()
        {
            DateTime? value = new DateTime(2026, 5, 1);
            var changes = new List<DateTime?>();
            var picks = new List<DatePickerPanelSelection>();
            var cut = Render<ElDatePickerPanel>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnChange, next => changes.Add(next))
                .Add(x => x.OnPick, selection => picks.Add(selection)));

            var day = cut.FindAll("td.available")
                .First(x => x.QuerySelector("span")?.TextContent.Trim() == "15");
            day.Click();

            Assert.Equal(new DateTime(2026, 5, 15), value);
            Assert.Equal(new DateTime(2026, 5, 15), changes[0]);
            Assert.Equal(DatePickerPanelType.Date, picks[0].Type);
        }

        [Fact]
        public void DisabledDateCannotBeSelected()
        {
            DateTime? value = new DateTime(2026, 5, 1);
            var cut = Render<ElDatePickerPanel>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.DisabledDate, day => day.Day == 15));

            var day = cut.FindAll("td.disabled")
                .First(x => x.QuerySelector("span")?.TextContent.Trim() == "15");
            day.Click();

            Assert.Equal(new DateTime(2026, 5, 1), value);
        }

        [Fact]
        public void MonthPanelSelectsMonthValue()
        {
            DateTime? value = null;
            var cut = Render<ElDatePickerPanel>(parameters => parameters
                .Add(x => x.Type, DatePickerPanelType.Month)
                .Add(x => x.Value, new DateTime(2026, 5, 27))
                .Add(x => x.ValueChanged, next => value = next));

            cut.FindAll(".el-month-table td")[6].Click();

            Assert.Equal(new DateTime(2026, 7, 1), value);
        }

        [Fact]
        public void YearPanelSelectsYearValue()
        {
            DateTime? value = null;
            var cut = Render<ElDatePickerPanel>(parameters => parameters
                .Add(x => x.Type, DatePickerPanelType.Year)
                .Add(x => x.Value, new DateTime(2026, 5, 27))
                .Add(x => x.ValueChanged, next => value = next));

            cut.FindAll(".el-year-table td")
                .First(x => x.TextContent.Trim() == "2028")
                .Click();

            Assert.Equal(new DateTime(2028, 1, 1), value);
        }

        [Fact]
        public void RendersFooterAndBorderlessState()
        {
            RenderFragment footer = builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "class", "custom-date-footer");
                builder.AddContent(2, "Today");
                builder.CloseElement();
            };

            var cut = Render<ElDatePickerPanel>(parameters => parameters
                .Add(x => x.Border, false)
                .Add(x => x.FooterContent, footer));

            Assert.Contains("is-borderless", cut.Find(".el-date-picker-panel").ClassList);
            Assert.Contains("custom-date-footer", cut.Markup);
        }
    }
}
