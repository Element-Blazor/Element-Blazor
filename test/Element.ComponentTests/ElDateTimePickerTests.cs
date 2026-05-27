using Bunit;
using Element;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Element.ComponentTests
{
    public class ElDateTimePickerTests : BunitContext
    {
        public ElDateTimePickerTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void RendersFormattedSingleValue()
        {
            var cut = Render<ElDateTimePicker>(parameters => parameters
                .Add(x => x.Value, new DateTime(2026, 5, 27, 14, 30, 45))
                .Add(x => x.Format, "YYYY/MM/DD HH:mm:ss"));

            Assert.Equal("2026/05/27 14:30:45", cut.Find("input").GetAttribute("value"));
            Assert.Contains("el-date-editor--datetime", cut.Find(".el-date-editor").ClassList);
        }

        [Fact]
        public void SelectsDateTimeFromPopupAndEmitsValueFormat()
        {
            DateTime? value = new DateTime(2026, 5, 1, 8, 0, 0);
            var stringValue = string.Empty;
            var changes = new List<DateTime?>();
            var cut = Render<ElDateTimePicker>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.ValueFormat, "YYYY-MM-DD HH:mm:ss")
                .Add(x => x.StringValueChanged, next => stringValue = next)
                .Add(x => x.OnChange, next => changes.Add(next)));

            cut.Find(".el-date-editor").Click();
            var fragment = Render(GetDropdown().OptionContent);
            fragment.FindAll("td.available")
                .First(x => x.QuerySelector("span")?.TextContent.Trim() == "15")
                .Click();
            fragment = Render(GetDropdown().OptionContent);
            fragment.Find(".el-date-time-picker__time-input").Change("09:45:00");
            fragment = Render(GetDropdown().OptionContent);
            fragment.Find(".el-date-time-picker__confirm").Click();

            Assert.Equal(new DateTime(2026, 5, 15, 9, 45, 0), value);
            Assert.Equal("2026-05-15 09:45:00", stringValue);
            Assert.Equal(new DateTime(2026, 5, 15, 9, 45, 0), changes[0]);
        }

        [Fact]
        public void ShortcutSelectsSingleValue()
        {
            DateTime? value = null;
            var cut = Render<ElDateTimePicker>(parameters => parameters
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.Shortcuts, new[]
                {
                    new DateTimePickerShortcut { Text = "Morning", Value = new DateTime(2026, 5, 27, 8, 30, 0) }
                }));

            cut.Find(".el-date-editor").Click();
            Render(GetDropdown().OptionContent).Find(".el-picker-panel__shortcut").Click();

            Assert.Equal(new DateTime(2026, 5, 27, 8, 30, 0), value);
        }

        [Fact]
        public void DisabledDatePreventsSelection()
        {
            DateTime? value = new DateTime(2026, 5, 1, 8, 0, 0);
            var cut = Render<ElDateTimePicker>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.DisabledDate, day => day.Day == 15));

            cut.Find(".el-date-editor").Click();
            var fragment = Render(GetDropdown().OptionContent);
            fragment.FindAll("td.disabled")
                .First(x => x.QuerySelector("span")?.TextContent.Trim() == "15")
                .Click();
            fragment = Render(GetDropdown().OptionContent);
            fragment.Find(".el-date-time-picker__confirm").Click();

            Assert.Equal(new DateTime(2026, 5, 1, 8, 0, 0), value);
        }

        [Fact]
        public void RangeSelectionOrdersValuesAndEmitsFormattedRange()
        {
            IList<DateTime?> range = new List<DateTime?>();
            IList<string> stringRange = new List<string>();
            var cut = Render<ElDateTimePicker>(parameters => parameters
                .Add(x => x.Type, DateTimePickerType.DateTimeRange)
                .Add(x => x.SinglePanel, true)
                .Add(x => x.DefaultValue, new DateTime(2026, 5, 1))
                .Add(x => x.DefaultStartTime, new TimeSpan(8, 0, 0))
                .Add(x => x.DefaultEndTime, new TimeSpan(18, 0, 0))
                .Add(x => x.RangeValueChanged, next => range = next)
                .Add(x => x.ValueFormat, "YYYY-MM-DD HH:mm:ss")
                .Add(x => x.StringRangeValueChanged, next => stringRange = next));

            cut.Find(".el-date-editor").Click();
            var fragment = Render(GetDropdown().OptionContent);
            fragment.FindAll("td.available").First(x => x.QuerySelector("span")?.TextContent.Trim() == "20").Click();
            fragment = Render(GetDropdown().OptionContent);
            fragment.Find(".el-date-time-picker__start-time").Change("09:00:00");
            fragment = Render(GetDropdown().OptionContent);
            fragment.FindAll("td.available").First(x => x.QuerySelector("span")?.TextContent.Trim() == "10").Click();
            fragment = Render(GetDropdown().OptionContent);
            fragment.Find(".el-date-time-picker__end-time").Change("17:30:00");
            fragment = Render(GetDropdown().OptionContent);
            fragment.Find(".el-date-time-picker__confirm").Click();

            Assert.Equal(new DateTime(2026, 5, 10, 9, 0, 0), range[0]);
            Assert.Equal(new DateTime(2026, 5, 20, 17, 30, 0), range[1]);
            Assert.Equal(new[] { "2026-05-10 09:00:00", "2026-05-20 17:30:00" }, stringRange);
        }

        [Fact]
        public void ClearableClearsSingleValue()
        {
            DateTime? value = new DateTime(2026, 5, 27, 14, 30, 45);
            var cleared = false;
            var cut = Render<ElDateTimePicker>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnClear, _ => cleared = true));

            cut.Find(".el-date-editor").MouseOver();
            cut.Find(".el-input__suffix .el-input__icon").Click();

            Assert.Null(value);
            Assert.True(cleared);
        }

        private DropDownOption GetDropdown()
        {
            return Services.GetRequiredService<PopupService>().SelectDropDownOptions.Single();
        }
    }
}
