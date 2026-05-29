using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Element.ComponentTests
{
    public class ElDataDisplayMoreTests : BunitContext
    {
        public ElDataDisplayMoreTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void CollapseTogglesItemsAndHonorsAccordionMode()
        {
            string active = "profile";
            var cut = Render(builder =>
            {
                builder.OpenComponent<ElCollapse>(0);
                builder.AddAttribute(1, nameof(ElCollapse.Value), active);
                builder.AddAttribute(2, nameof(ElCollapse.ValueChanged), EventCallback.Factory.Create<string>(this, next => active = next));
                builder.AddAttribute(3, nameof(ElCollapse.Accordion), true);
                builder.AddAttribute(4, nameof(ElCollapse.ChildContent), (RenderFragment)(child =>
                {
                    child.OpenComponent<ElCollapseItem>(0);
                    child.AddAttribute(1, nameof(ElCollapseItem.Name), "profile");
                    child.AddAttribute(2, nameof(ElCollapseItem.Title), "Profile");
                    child.AddAttribute(3, nameof(ElCollapseItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Profile body")));
                    child.CloseComponent();
                    child.OpenComponent<ElCollapseItem>(4);
                    child.AddAttribute(5, nameof(ElCollapseItem.Name), "billing");
                    child.AddAttribute(6, nameof(ElCollapseItem.Title), "Billing");
                    child.AddAttribute(7, nameof(ElCollapseItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Billing body")));
                    child.CloseComponent();
                }));
                builder.CloseComponent();
            });

            Assert.Contains("is-active", cut.FindAll(".el-collapse-item")[0].ClassList);
            Assert.DoesNotContain("display:none", cut.FindAll(".el-collapse-item__wrap")[0].GetAttribute("style") ?? string.Empty);

            cut.FindAll(".el-collapse-item__header")[1].Click();

            Assert.Equal("billing", active);
            Assert.Contains("is-active", cut.FindAll(".el-collapse-item")[1].ClassList);
            Assert.Contains("display:none", cut.FindAll(".el-collapse-item__wrap")[0].GetAttribute("style"));
        }

        [Fact]
        public void SegmentedSelectsEnabledOptionAndSkipsDisabledOption()
        {
            var value = "daily";
            var changes = new List<string>();
            var options = new[]
            {
                new SegmentedOption { Label = "Daily", Value = "daily" },
                new SegmentedOption { Label = "Weekly", Value = "weekly" },
                new SegmentedOption { Label = "Monthly", Value = "monthly", Disabled = true }
            };

            var cut = Render<ElSegmented<string>>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnChange, next => changes.Add(next))
                .Add(x => x.Options, options)
                .Add(x => x.Block, true));

            Assert.Contains("is-block", cut.Find(".el-segmented").ClassList);
            Assert.Contains("is-selected", cut.FindAll(".el-segmented__item")[0].ClassList);

            cut.FindAll("input")[1].Change("weekly");

            Assert.Equal("weekly", value);
            Assert.Equal("weekly", changes.Single());
            Assert.Contains("is-selected", cut.FindAll(".el-segmented__item")[1].ClassList);

            cut.FindAll("input")[2].Change("monthly");

            Assert.Equal("weekly", value);
            Assert.Contains("is-disabled", cut.FindAll(".el-segmented__item")[2].ClassList);
        }

        [Fact]
        public void ImageRendersFitLazyErrorAndPreview()
        {
            var errors = 0;
            var cut = Render<ElImage>(parameters => parameters
                .Add(x => x.Src, "/photo.png")
                .Add(x => x.Alt, "Photo")
                .Add(x => x.Fit, "cover")
                .Add(x => x.Lazy, true)
                .Add(x => x.Width, "120")
                .Add(x => x.Height, "80")
                .Add(x => x.PreviewSrcList, new List<string> { "/photo.png", "/next.png" })
                .Add(x => x.OnError, () => errors++));

            var image = cut.Find(".el-image__inner");
            Assert.Equal("lazy", image.GetAttribute("loading"));
            Assert.Contains("object-fit:cover", image.GetAttribute("style"));
            Assert.Contains("width:120px", cut.Find(".el-image").GetAttribute("style"));

            image.Click();

            Assert.NotEmpty(cut.FindAll(".el-image-viewer__wrapper"));
            Assert.Equal("/photo.png", cut.Find(".el-image-viewer__canvas img").GetAttribute("src"));

            cut.Find(".el-image-viewer__next").Click();

            Assert.Equal("/next.png", cut.Find(".el-image-viewer__canvas img").GetAttribute("src"));

            cut.Find(".el-image-viewer__close").Click();
            Assert.Empty(cut.FindAll(".el-image-viewer__wrapper"));

            cut.Find(".el-image__inner").TriggerEvent("onerror", EventArgs.Empty);

            Assert.Equal(1, errors);
            Assert.Equal("加载失败", cut.Find(".el-image__error").TextContent.Trim());
        }

        [Fact]
        public void CalendarRendersCurrentMonthAndSelectsDate()
        {
            DateTime? value = new DateTime(2026, 5, 12);
            DateTime? picked = null;
            var cut = Render<ElCalendar>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnPick, next => picked = next)
                .Add(x => x.DateCell, context => builder =>
                {
                    builder.OpenElement(0, "span");
                    builder.AddAttribute(1, "class", "day-cell");
                    builder.AddContent(2, context.Date.Day);
                    builder.CloseElement();
                }));

            Assert.Contains("2026 年 5 月", cut.Find(".el-calendar__title").TextContent.Trim());
            Assert.Single(cut.FindAll("td.is-selected"));
            Assert.NotEmpty(cut.FindAll(".day-cell"));

            cut.FindAll("td.current")
                .First(x => x.TextContent.Trim() == "20")
                .Click();

            Assert.Equal(new DateTime(2026, 5, 20), value);
            Assert.Equal(new DateTime(2026, 5, 20), picked);

            cut.FindAll("button")[0].Click();

            Assert.Contains("2026 年 4 月", cut.Find(".el-calendar__title").TextContent.Trim());
        }

        [Fact]
        public void CarouselSwitchesSlidesWithArrowsAndIndicators()
        {
            var changes = new List<int>();
            var cut = Render(builder =>
            {
                builder.OpenComponent<ElCarousel>(0);
                builder.AddAttribute(1, nameof(ElCarousel.Autoplay), false);
                builder.AddAttribute(2, nameof(ElCarousel.Height), "180");
                builder.AddAttribute(3, nameof(ElCarousel.Arrow), CarouselArrow.Always);
                builder.AddAttribute(4, nameof(ElCarousel.OnChange), EventCallback.Factory.Create<int>(this, index => changes.Add(index)));
                builder.AddAttribute(5, nameof(ElCarousel.ChildContent), (RenderFragment)(child =>
                {
                    child.OpenComponent<ElCarouselItem>(0);
                    child.AddAttribute(1, nameof(ElCarouselItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "First")));
                    child.CloseComponent();
                    child.OpenComponent<ElCarouselItem>(2);
                    child.AddAttribute(3, nameof(ElCarouselItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Second")));
                    child.CloseComponent();
                }));
                builder.CloseComponent();
            });

            Assert.Contains("height:180px", cut.Find(".el-carousel__container").GetAttribute("style"));
            Assert.Contains("is-active", cut.FindAll(".el-carousel__item")[0].ClassList);
            Assert.Contains("display:none", cut.FindAll(".el-carousel__item")[1].GetAttribute("style"));

            cut.Find(".el-carousel__arrow--right").Click();

            Assert.Equal(1, changes.Single());
            Assert.Contains("is-active", cut.FindAll(".el-carousel__item")[1].ClassList);
            Assert.Contains("display:none", cut.FindAll(".el-carousel__item")[0].GetAttribute("style"));

            cut.FindAll(".el-carousel__indicator button")[0].Click();

            Assert.Equal(new[] { 1, 0 }, changes);
            Assert.Contains("is-active", cut.FindAll(".el-carousel__indicator")[0].ClassList);
        }
    }
}
