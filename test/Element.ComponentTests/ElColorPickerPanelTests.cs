using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Xunit;

namespace Element.ComponentTests
{
    public class ElColorPickerPanelTests : BunitContext
    {
        public ElColorPickerPanelTests()
        {
            Services.AddElementServices();
        }

        [Theory]
        [InlineData("#409EFF", "#409eff")]
        [InlineData("rgb(255, 120, 0)", "#ff7800")]
        [InlineData("rgba(255, 69, 0, 0.68)", "rgba(255, 69, 0, 0.68)")]
        [InlineData("hsv(51, 100, 98)", "#fad400")]
        [InlineData("hsla(209, 100%, 56%, 0.73)", "rgba(31, 147, 255, 0.73)")]
        [InlineData("#c7158577", "rgba(199, 21, 133, 0.467)")]
        public void ElementColorParsesSupportedFormats(string value, string expected)
        {
            var color = ElementColor.Parse(value);
            var actual = value.Contains("0.") || value.Length == 9
                ? color.ToCssString(ElementColorFormat.Rgb, true)
                : color.ToCssString(ElementColorFormat.Hex, false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RendersAlphaPredefineBorderAndFooter()
        {
            RenderFragment footer = builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "class", "custom-footer");
                builder.AddContent(2, "Apply");
                builder.CloseElement();
            };

            var cut = Render<ElColorPickerPanel>(parameters => parameters
                .Add(x => x.Value, "rgba(255, 69, 0, 0.68)")
                .Add(x => x.ShowAlpha, true)
                .Add(x => x.Border, false)
                .Add(x => x.Predefine, new[]
                {
                    "#ff4500",
                    "rgba(255, 69, 0, 0.68)"
                })
                .Add(x => x.FooterContent, footer));

            Assert.Contains("is-borderless", cut.Find(".el-color-picker-panel").ClassList);
            Assert.NotNull(cut.Find(".el-color-alpha-slider"));
            Assert.Equal(2, cut.FindAll(".el-color-predefine__color-selector").Count);
            Assert.Contains("selected", cut.FindAll(".el-color-predefine__color-selector")[1].ClassList);
            Assert.Contains("custom-footer", cut.Markup);
        }

        [Fact]
        public void HueInputUpdatesBoundValue()
        {
            var value = "#ff0000";
            var changes = new List<string>();
            var cut = Render<ElColorPickerPanel>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnChange, next => changes.Add(next)));

            cut.Find("input[aria-label='hue']").Input("120");

            Assert.Equal("#00ff00", value);
            Assert.Equal(new[] { "#00ff00" }, changes);
        }

        [Fact]
        public void TextInputRespectsColorFormat()
        {
            var value = "#409eff";
            var cut = Render<ElColorPickerPanel>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ColorFormat, "rgb")
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find("input[aria-label='color value']").Input("#ff0000");
            cut.Find("input[aria-label='color value']").Change("#ff0000");

            Assert.Equal("rgb(255, 0, 0)", value);
        }

        [Fact]
        public void DisabledPanelIgnoresInput()
        {
            var value = "#ff0000";
            var cut = Render<ElColorPickerPanel>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.Disabled, true)
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find("input[aria-label='hue']").Input("120");

            Assert.Equal("#ff0000", value);
            Assert.True(cut.Find("input[aria-label='hue']").HasAttribute("disabled"));
        }
    }
}
