using Bunit;
using Element;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Element.ComponentTests
{
    public class ElDatePickerTests : BunitContext
    {
        public ElDatePickerTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void RendersFormattedSingleValue()
        {
            var cut = Render<ElDatePicker>(parameters => parameters
                .Add(x => x.Value, new DateTime(2026, 5, 27))
                .Add(x => x.Format, "YYYY/MM/DD"));

            Assert.Equal("2026/05/27", cut.Find("input").GetAttribute("value"));
            Assert.Contains("el-date-editor--date", cut.Find(".el-date-editor").ClassList);
        }

        [Fact]
        public void SelectsDateFromPopupAndEmitsValueFormat()
        {
            DateTime? value = new DateTime(2026, 5, 1);
            var stringValue = string.Empty;
            var changes = new List<DateTime?>();
            var cut = Render<ElDatePicker>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.ValueFormat, "YYYY-MM-DD")
                .Add(x => x.StringValueChanged, next => stringValue = next)
                .Add(x => x.OnChange, next => changes.Add(next)));

            cut.Find(".el-date-editor").Click();
            var fragment = Render(GetDropdown().OptionContent);
            fragment.FindAll("td.available")
                .First(x => x.QuerySelector("span")?.TextContent.Trim() == "15")
                .Click();

            Assert.Equal(new DateTime(2026, 5, 15), value);
            Assert.Equal("2026-05-15", stringValue);
            Assert.Equal(new DateTime(2026, 5, 15), changes[0]);
        }

        [Fact]
        public void DisabledDatePreventsSelection()
        {
            DateTime? value = new DateTime(2026, 5, 1);
            var cut = Render<ElDatePicker>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.DisabledDate, day => day.Day == 15));

            cut.Find(".el-date-editor").Click();
            var fragment = Render(GetDropdown().OptionContent);
            fragment.FindAll("td.disabled")
                .First(x => x.QuerySelector("span")?.TextContent.Trim() == "15")
                .Click();

            Assert.Equal(new DateTime(2026, 5, 1), value);
        }

        [Fact]
        public void ShortcutSelectsSingleValue()
        {
            DateTime? value = null;
            var cut = Render<ElDatePicker>(parameters => parameters
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.Shortcuts, new[]
                {
                    new DatePickerShortcut { Text = "Today", Value = new DateTime(2026, 5, 27) }
                }));

            cut.Find(".el-date-editor").Click();
            Render(GetDropdown().OptionContent).Find(".el-picker-panel__shortcut").Click();

            Assert.Equal(new DateTime(2026, 5, 27), value);
        }

        [Fact]
        public void RangeSelectionOrdersValuesAndEmitsFormattedRange()
        {
            IList<DateTime?> range = new List<DateTime?>();
            IList<string> stringRange = new List<string>();
            var cut = Render<ElDatePicker>(parameters => parameters
                .Add(x => x.Type, DatePickerType.DateRange)
                .Add(x => x.SinglePanel, true)
                .Add(x => x.DefaultValue, new DateTime(2026, 5, 1))
                .Add(x => x.RangeValueChanged, next => range = next)
                .Add(x => x.ValueFormat, "YYYY-MM-DD")
                .Add(x => x.StringRangeValueChanged, next => stringRange = next));

            cut.Find(".el-date-editor").Click();
            var fragment = Render(GetDropdown().OptionContent);
            fragment.FindAll("td.available").First(x => x.QuerySelector("span")?.TextContent.Trim() == "20").Click();
            fragment = Render(GetDropdown().OptionContent);
            fragment.FindAll("td.available").First(x => x.QuerySelector("span")?.TextContent.Trim() == "10").Click();

            Assert.Equal(new DateTime(2026, 5, 10), range[0]);
            Assert.Equal(new DateTime(2026, 5, 20), range[1]);
            Assert.Equal(new[] { "2026-05-10", "2026-05-20" }, stringRange);
        }

        [Fact]
        public void ParsesStringValueWithValueFormat()
        {
            var cut = Render<ElDatePicker>(parameters => parameters
                .Add(x => x.StringValue, "2026/05/27")
                .Add(x => x.ValueFormat, "YYYY/MM/DD")
                .Add(x => x.Format, "DD-MM-YYYY"));

            Assert.Equal("27-05-2026", cut.Find("input").GetAttribute("value"));
        }

        private DropDownOption GetDropdown()
        {
            return Services.GetRequiredService<PopupService>().SelectDropDownOptions.Single();
        }
    }
}
