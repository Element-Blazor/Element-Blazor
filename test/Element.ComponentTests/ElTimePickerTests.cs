using Bunit;
using Element;
using Element.ControlConfigs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Element.ComponentTests
{
    public class ElTimePickerTests : BunitContext
    {
        public ElTimePickerTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void RendersFormattedSingleValue()
        {
            var cut = Render<ElTimePicker>(parameters => parameters
                .Add(x => x.Value, new TimeSpan(14, 30, 45))
                .Add(x => x.Format, "HH:mm"));

            Assert.Equal("14:30", cut.Find("input").GetAttribute("value"));
            Assert.Contains("el-date-editor--time", cut.Find(".el-date-editor").ClassList);
        }

        [Fact]
        public void SelectsTimeFromPanelAndEmitsValueFormat()
        {
            TimeSpan? value = null;
            var stringValue = string.Empty;
            var changes = new List<TimeSpan?>();
            var cut = Render<ElTimePicker>(parameters => parameters
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.ValueFormat, "HH:mm:ss")
                .Add(x => x.StringValueChanged, next => stringValue = next)
                .Add(x => x.OnChange, next => changes.Add(next)));

            cut.Find(".el-date-editor").Click();
            SelectTimePart("single", "hour", 9);
            SelectTimePart("single", "minute", 45);
            SelectTimePart("single", "second", 30);
            Render(GetDropdown().OptionContent).Find(".el-time-panel__btn.confirm").Click();

            Assert.Equal(new TimeSpan(9, 45, 30), value);
            Assert.Equal("09:45:30", stringValue);
            Assert.Equal(new TimeSpan(9, 45, 30), changes[0]);
        }

        [Fact]
        public void EditableInputParsesFormattedValue()
        {
            TimeSpan? value = null;
            var cut = Render<ElTimePicker>(parameters => parameters
                .Add(x => x.Format, "HH:mm")
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find("input").Change("08:15");

            Assert.Equal(new TimeSpan(8, 15, 0), value);
        }

        [Fact]
        public void RangeSelectionOrdersValuesAndEmitsFormattedRange()
        {
            IList<TimeSpan?> range = new List<TimeSpan?>();
            IList<string> stringRange = new List<string>();
            var cut = Render<ElTimePicker>(parameters => parameters
                .Add(x => x.Type, TimePickerType.TimeRange)
                .Add(x => x.DefaultStartTime, new TimeSpan(20, 0, 0))
                .Add(x => x.DefaultEndTime, new TimeSpan(8, 0, 0))
                .Add(x => x.RangeValueChanged, next => range = next)
                .Add(x => x.ValueFormat, "HH:mm:ss")
                .Add(x => x.StringRangeValueChanged, next => stringRange = next));

            cut.Find(".el-date-editor").Click();
            SelectTimePart("start", "hour", 20);
            SelectTimePart("start", "minute", 30);
            SelectTimePart("end", "hour", 8);
            SelectTimePart("end", "minute", 15);
            Render(GetDropdown().OptionContent).Find(".el-time-panel__btn.confirm").Click();

            Assert.Equal(new TimeSpan(8, 15, 0), range[0]);
            Assert.Equal(new TimeSpan(20, 30, 0), range[1]);
            Assert.Equal(new[] { "08:15:00", "20:30:00" }, stringRange);
        }

        [Fact]
        public void ClearableClearsSingleValue()
        {
            TimeSpan? value = new TimeSpan(14, 30, 45);
            var cleared = false;
            var cut = Render<ElTimePicker>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnClear, () => cleared = true));

            cut.Find(".el-date-editor").MouseOver();
            cut.Find(".el-input__suffix .el-input__icon").Click();

            Assert.Null(value);
            Assert.True(cleared);
        }

        [Fact]
        public void GeneratedFormUsesTimePickerAndUpdatesModel()
        {
            var model = new GeneratedTimePickerModel
            {
                StartAt = new TimeSpan(9, 15, 0)
            };

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.EntityType, typeof(GeneratedTimePickerModel))
                .Add(x => x.Value, model));

            var wrapper = cut.Find(".el-date-editor");
            var input = cut.Find("input");

            Assert.Contains("el-date-editor--time", wrapper.ClassList);
            Assert.Contains("el-input--large", wrapper.ClassList);
            Assert.Equal("Start", input.GetAttribute("placeholder"));
            Assert.Equal("09:15", input.GetAttribute("value"));

            wrapper.Click();
            SelectTimePart("single", "hour", 10);
            SelectTimePart("single", "minute", 45);
            Render(GetDropdown().OptionContent).Find(".el-time-panel__btn.confirm").Click();

            Assert.Equal(new TimeSpan(10, 45, 0), model.StartAt);
        }

        private void SelectTimePart(string panel, string part, int value)
        {
            Render(GetDropdown().OptionContent)
                .Find($"li[data-time-panel='{panel}'][data-time-part='{part}'][data-time-value='{value}']")
                .Click();
        }

        private DropDownOption GetDropdown()
        {
            return Services.GetRequiredService<PopupService>().SelectDropDownOptions.Single();
        }

        private class GeneratedTimePickerModel
        {
            [TimePicker(Format = "HH:mm", Placeholder = "Start", Size = InputSize.Large)]
            public TimeSpan StartAt { get; set; }
        }
    }
}
