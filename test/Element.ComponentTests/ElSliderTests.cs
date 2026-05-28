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
                .Add(x => x.RangeValue, range)
                .Add(x => x.Id, "range-slider"));

            var inputs = cut.FindAll(".el-slider__input--range");
            Assert.Equal(2, inputs.Count);
            Assert.Equal("slider", inputs[0].GetAttribute("role"));
            Assert.Equal("range-slider", inputs[0].Id);
            Assert.Equal("range-slider-2", inputs[1].Id);
            Assert.Equal("20", inputs[0].GetAttribute("aria-valuenow"));
            Assert.Equal("80", inputs[1].GetAttribute("aria-valuenow"));
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
            Assert.Contains("left:80", cut.Find(".el-slider__bar").GetAttribute("style"));
            Assert.Contains("width:10", cut.Find(".el-slider__bar").GetAttribute("style"));
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

        [Fact]
        public void SliderCanHideTooltip()
        {
            var cut = Render<ElSlider>(parameters => parameters
                .Add(x => x.Value, 30)
                .Add(x => x.ShowTooltip, false));

            Assert.Empty(cut.FindAll(".el-slider__tooltip"));
            Assert.Equal("30", cut.Find(".el-slider__input").GetAttribute("aria-valuetext"));
        }

        [Fact]
        public void RangeSliderFormatsBothTooltips()
        {
            var cut = Render<ElSlider>(parameters => parameters
                .Add(x => x.Range, true)
                .Add(x => x.RangeValue, new List<double> { 10, 40 })
                .Add(x => x.FormatTooltip, value => $"Level {value}"));

            var tooltips = cut.FindAll(".el-slider__tooltip");
            var inputs = cut.FindAll(".el-slider__input--range");

            Assert.Equal(2, tooltips.Count);
            Assert.Equal("Level 10", tooltips[0].TextContent);
            Assert.Equal("Level 40", tooltips[1].TextContent);
            Assert.Equal("Level 10", inputs[0].GetAttribute("aria-valuetext"));
            Assert.Equal("Level 40", inputs[1].GetAttribute("aria-valuetext"));
        }

        [Fact]
        public void FocusShowsTooltipForActiveThumb()
        {
            var cut = Render<ElSlider>(parameters => parameters
                .Add(x => x.Range, true)
                .Add(x => x.RangeValue, new List<double> { 10, 40 }));

            cut.FindAll(".el-slider__input--range")[1].Focus();

            Assert.DoesNotContain("is-hover", cut.FindAll(".el-slider__button-wrapper")[0].ClassList);
            Assert.Contains("is-hover", cut.FindAll(".el-slider__button-wrapper")[1].ClassList);

            cut.FindAll(".el-slider__input--range")[1].Blur();

            Assert.DoesNotContain("is-hover", cut.FindAll(".el-slider__button-wrapper")[1].ClassList);
        }

        [Fact]
        public void RangeSliderNormalizesBoundsAndStep()
        {
            var cut = Render<ElSlider>(parameters => parameters
                .Add(x => x.Range, true)
                .Add(x => x.Min, 10)
                .Add(x => x.Max, 50)
                .Add(x => x.Step, 5)
                .Add(x => x.RangeValue, new List<double> { 52, 11 }));

            var inputs = cut.FindAll(".el-slider__input--range");

            Assert.Equal("10", inputs[0].GetAttribute("value"));
            Assert.Equal("50", inputs[1].GetAttribute("value"));
            Assert.Contains("left:0", cut.Find(".el-slider__bar").GetAttribute("style"));
            Assert.Contains("width:100", cut.Find(".el-slider__bar").GetAttribute("style"));
        }

        [Fact]
        public void SliderRendersStopsMarksAndInput()
        {
            double value = 25;
            var cut = Render<ElSlider>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.Step, 25)
                .Add(x => x.ShowStops, true)
                .Add(x => x.ShowInput, true)
                .Add(x => x.Marks, new[]
                {
                    new SliderMark { Value = 0, Label = "Low" },
                    new SliderMark { Value = 100, Label = "High" }
                }));

            Assert.Equal(3, cut.FindAll(".el-slider__stop").Count);
            Assert.Contains("Low", cut.Markup);
            Assert.Contains("High", cut.Markup);
            Assert.Contains("el-slider--with-input", cut.Find(".el-slider").ClassList);

            cut.Find(".el-input-number input").Change("75");

            Assert.Equal(75, value);
            Assert.Equal("75", cut.Find(".el-slider__input").GetAttribute("value"));
        }

        [Fact]
        public void DisabledSliderDoesNotCommitInputs()
        {
            double value = 20;
            IList<double> range = new List<double> { 10, 40 };
            var single = Render<ElSlider>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.Disabled, true));
            var ranged = Render<ElSlider>(parameters => parameters
                .Add(x => x.Range, true)
                .Add(x => x.RangeValue, range)
                .Add(x => x.RangeValueChanged, next => range = next)
                .Add(x => x.Disabled, true));

            single.Find(".el-slider__input").Input(80);
            ranged.FindAll(".el-slider__input--range")[1].Input(80);

            Assert.Equal(20, value);
            Assert.Equal(new[] { 10d, 40d }, range);
            Assert.Equal("true", single.Find(".el-slider").GetAttribute("aria-disabled"));
            Assert.Equal("true", single.Find(".el-slider__input").GetAttribute("aria-disabled"));
        }
    }
}
