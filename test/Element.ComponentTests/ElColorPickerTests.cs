using Bunit;
using Element;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Element.ComponentTests
{
    public class ElColorPickerTests : BunitContext
    {
        public ElColorPickerTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void RendersTriggerWithColorAndSize()
        {
            var cut = Render<ElColorPicker>(parameters => parameters
                .Add(x => x.Value, "rgba(255, 69, 0, 0.68)")
                .Add(x => x.ShowAlpha, true)
                .Add(x => x.Size, InputSize.Small));

            Assert.Contains("el-color-picker--small", cut.Find(".el-color-picker").ClassList);
            Assert.Contains("is-alpha", cut.Find(".el-color-picker__color").ClassList);
            Assert.Contains("rgba(255, 69, 0, 0.68)", cut.Find(".el-color-picker__color-inner").GetAttribute("style"));
        }

        [Fact]
        public void OpensPanelWithAlphaAndPredefinedColors()
        {
            var cut = Render<ElColorPicker>(parameters => parameters
                .Add(x => x.Value, "#ff0000")
                .Add(x => x.ShowAlpha, true)
                .Add(x => x.Predefine, new[] { "#ff0000", "#00ff00" }));

            cut.Find(".el-color-picker").Click();
            var dropdown = GetDropdown();
            var fragment = Render(dropdown.OptionContent);

            Assert.Contains("el-color-picker__dropdown", dropdown.PopperClass);
            Assert.Equal(300, dropdown.Width);
            Assert.NotNull(fragment.Find(".el-color-alpha-slider"));
            Assert.Equal(2, fragment.FindAll(".el-color-predefine__color-selector").Count);
        }

        [Fact]
        public void PanelChangeUpdatesBoundValueAndChangeEvent()
        {
            var value = "#ff0000";
            var changes = new List<string>();
            var cut = Render<ElColorPicker>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnChange, next => changes.Add(next)));

            cut.Find(".el-color-picker").Click();
            var fragment = Render(GetDropdown().OptionContent);
            fragment.Find("input[aria-label='hue']").Input("120");

            Assert.Equal("#00ff00", value);
            Assert.Equal(new[] { "#00ff00" }, changes);
        }

        [Fact]
        public void ClearableTriggerClearsValue()
        {
            var value = "#ff0000";
            var cleared = false;
            var cut = Render<ElColorPicker>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.Clearable, true)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnClear, _ => cleared = true));

            cut.Find(".el-color-picker").MouseOver();
            cut.Find(".el-color-picker__icon").Click();

            Assert.Equal(string.Empty, value);
            Assert.True(cleared);
        }

        [Fact]
        public void DisabledPickerDoesNotOpen()
        {
            var cut = Render<ElColorPicker>(parameters => parameters
                .Add(x => x.Value, "#ff0000")
                .Add(x => x.Disabled, true));

            cut.Find(".el-color-picker").Click();

            Assert.Empty(Services.GetRequiredService<PopupService>().SelectDropDownOptions);
            Assert.Contains("is-disabled", cut.Find(".el-color-picker").ClassList);
        }

        private DropDownOption GetDropdown()
        {
            return Services.GetRequiredService<PopupService>().SelectDropDownOptions.Single();
        }
    }
}
