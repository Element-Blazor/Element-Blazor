using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Element.ComponentTests
{
    public class ElRateTests : BunitContext
    {
        public ElRateTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void RendersCustomIconTemplate()
        {
            RenderFragment<RateIconContext> template = context => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", context.Active ? "custom-rate active" : "custom-rate");
                builder.AddAttribute(2, "data-index", context.Index);
                builder.AddAttribute(3, "data-max", context.Max);
                builder.AddAttribute(4, "data-value", context.Value?.ToString("0.0"));
                builder.AddAttribute(5, "data-current", context.CurrentValue?.ToString("0.0"));
                builder.AddAttribute(6, "data-color", context.Color);
                builder.AddAttribute(7, "data-icon", context.Icon);
                builder.AddAttribute(8, "style", context.IconStyle);
                builder.AddContent(9, context.Index);
                builder.CloseElement();
            };

            var cut = Render<ElRate>(parameters => parameters
                .Add(x => x.Value, 2)
                .Add(x => x.Colors, new[] { "red", "orange", "green" })
                .Add(x => x.IconTemplate, template));

            Assert.Equal(5, cut.FindAll(".custom-rate").Count);
            Assert.Equal(2, cut.FindAll(".custom-rate.active").Count);
            Assert.Equal("5", cut.Find(".custom-rate").GetAttribute("data-max"));
            Assert.Equal("2.0", cut.Find(".custom-rate").GetAttribute("data-value"));
            Assert.Equal("2.0", cut.Find(".custom-rate").GetAttribute("data-current"));
            Assert.Equal("red", cut.Find(".custom-rate").GetAttribute("data-color"));
            Assert.Equal("el-icon-star-on", cut.Find(".custom-rate").GetAttribute("data-icon"));
            Assert.Equal("color:red", cut.Find(".custom-rate").GetAttribute("style"));
        }

        [Fact]
        public void IconTemplateReceivesHalfHoverContext()
        {
            var cut = Render<RateTemplateHost>();

            cut.FindAll(".el-rate__item")[2].MouseMove(new MouseEventArgs { OffsetX = 4 });

            cut.WaitForAssertion(() =>
            {
                var icons = cut.FindAll(".template-rate");
                Assert.Equal("true", icons[2].GetAttribute("data-half"));
                Assert.Equal("2.5", icons[2].GetAttribute("data-current"));
                Assert.Equal("2.5", icons[2].GetAttribute("data-hover"));
                Assert.Equal("false", icons[2].GetAttribute("data-disabled"));
                Assert.Equal("false", icons[2].GetAttribute("data-empty"));
                Assert.Equal("gray", icons[4].GetAttribute("data-color"));
                Assert.Equal("el-rate__icon void-icon", icons[4].GetAttribute("data-class"));
            });
        }

        [Fact]
        public void IconTemplateReceivesDisabledContext()
        {
            var cut = Render<DisabledRateTemplateHost>();

            var icons = cut.FindAll(".template-rate");

            Assert.Equal("true", cut.Find(".el-rate").GetAttribute("aria-disabled"));
            Assert.All(icons, icon => Assert.Equal("true", icon.GetAttribute("data-disabled")));
            Assert.Equal("disabled-gray", icons[3].GetAttribute("data-color"));
            Assert.Equal("el-rate__icon void-icon", icons[3].GetAttribute("data-class"));
        }

        [Fact]
        public void ItemTemplateAliasRendersCustomIcons()
        {
            RenderFragment<RateIconContext> template = context => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", context.Empty ? "alias-rate empty" : "alias-rate");
                builder.AddContent(2, context.Index);
                builder.CloseElement();
            };

            var cut = Render<ElRate>(parameters => parameters
                .Add(x => x.Value, 3)
                .Add(x => x.ItemTemplate, template));

            Assert.Equal(5, cut.FindAll(".alias-rate").Count);
            Assert.Equal(2, cut.FindAll(".alias-rate.empty").Count);
        }

        [Fact]
        public void CustomIconTemplateStillCommitsSelection()
        {
            double? value = 1;
            double? changed = null;
            RenderFragment<RateIconContext> template = context => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "selectable-rate");
                builder.AddContent(2, context.Index);
                builder.CloseElement();
            };

            var cut = Render<ElRate>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnChange, next => changed = next)
                .Add(x => x.IconTemplate, template));

            cut.FindAll(".el-rate__item")[3].Click();

            Assert.Equal(4, value);
            Assert.Equal(4, changed);
        }

        private class RateTemplateHost : ComponentBase
        {
            private double? value = 2;

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<ElRate>(0);
                builder.AddAttribute(1, nameof(ElRate.Value), value);
                builder.AddAttribute(2, nameof(ElRate.AllowHalf), true);
                builder.AddAttribute(3, nameof(ElRate.VoidColor), "gray");
                builder.AddAttribute(4, nameof(ElRate.Colors), new[] { "red", "orange", "gold" });
                builder.AddAttribute(5, nameof(ElRate.DisabledVoidColor), "disabled-gray");
                builder.AddAttribute(6, nameof(ElRate.Icon), "active-icon");
                builder.AddAttribute(7, nameof(ElRate.VoidIcon), "void-icon");
                builder.AddAttribute(8, nameof(ElRate.IconTemplate), (RenderFragment<RateIconContext>)(context => templateBuilder =>
                {
                    templateBuilder.OpenElement(0, "span");
                    templateBuilder.AddAttribute(1, "class", context.Half ? "template-rate half" : "template-rate");
                    templateBuilder.AddAttribute(2, "data-half", context.Half ? "true" : "false");
                    templateBuilder.AddAttribute(3, "data-current", context.CurrentValue?.ToString("0.0"));
                    templateBuilder.AddAttribute(4, "data-hover", context.HoverValue?.ToString("0.0"));
                    templateBuilder.AddAttribute(5, "data-disabled", context.Disabled ? "true" : "false");
                    templateBuilder.AddAttribute(6, "data-empty", context.Empty ? "true" : "false");
                    templateBuilder.AddAttribute(7, "data-color", context.Color);
                    templateBuilder.AddAttribute(8, "data-class", context.IconClass);
                    templateBuilder.CloseElement();
                }));
                builder.CloseComponent();
            }
        }

        private class DisabledRateTemplateHost : ComponentBase
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<ElRate>(0);
                builder.AddAttribute(1, nameof(ElRate.Value), 2d);
                builder.AddAttribute(2, nameof(ElRate.Disabled), true);
                builder.AddAttribute(3, nameof(ElRate.DisabledVoidColor), "disabled-gray");
                builder.AddAttribute(4, nameof(ElRate.Icon), "active-icon");
                builder.AddAttribute(5, nameof(ElRate.VoidIcon), "void-icon");
                builder.AddAttribute(6, nameof(ElRate.IconTemplate), (RenderFragment<RateIconContext>)(context => templateBuilder =>
                {
                    templateBuilder.OpenElement(0, "span");
                    templateBuilder.AddAttribute(1, "class", "template-rate");
                    templateBuilder.AddAttribute(2, "data-disabled", context.Disabled ? "true" : "false");
                    templateBuilder.AddAttribute(3, "data-color", context.Color);
                    templateBuilder.AddAttribute(4, "data-class", context.IconClass);
                    templateBuilder.CloseElement();
                }));
                builder.CloseComponent();
            }
        }
    }
}
