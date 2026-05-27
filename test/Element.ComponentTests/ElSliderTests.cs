using Bunit;
using Element;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Element.ComponentTests
{
    public class ElSliderTests : BunitContext
    {
        public ElSliderTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void RangeSliderRendersTwoThumbsAndRangeBar()
        {
            var range = new List<double> { 20, 80 };
            var cut = Render<ElSlider>(parameters => parameters
                .Add(x => x.Range, true)
                .Add(x => x.RangeValue, range));

            Assert.Equal(2, cut.FindAll(".el-slider__input--range").Count);
            Assert.Contains("left:20", cut.Find(".el-slider__bar").GetAttribute("style"));
            Assert.Contains("width:60", cut.Find(".el-slider__bar").GetAttribute("style"));
        }

        [Fact]
        public void RangeSliderOrdersChangedValues()
        {
            IList<double> range = new List<double> { 20, 80 };
            var cut = Render<ElSlider>(parameters => parameters
                .Add(x => x.Range, true)
                .Add(x => x.RangeValue, range)
                .Add(x => x.RangeValueChanged, next => range = next));

            cut.FindAll(".el-slider__input--range").First().Input(90);

            Assert.Equal(new[] { 80d, 90d }, range);
        }

        [Fact]
        public void SliderFormatsTooltip()
        {
            var cut = Render<ElSlider>(parameters => parameters
                .Add(x => x.Value, 30)
                .Add(x => x.FormatTooltip, value => $"{value}%"));

            Assert.Contains("30%", cut.Markup);
            Assert.NotEmpty(cut.FindAll(".el-slider__tooltip"));
        }
    }
}
