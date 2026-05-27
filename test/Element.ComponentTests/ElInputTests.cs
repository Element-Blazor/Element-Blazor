using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Element.ComponentTests
{
    public class ElInputTests : BunitContext
    {
        public ElInputTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void RendersPrefixAndSuffixSlots()
        {
            RenderFragment prefix = builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "custom-prefix");
                builder.AddContent(2, "https://");
                builder.CloseElement();
            };
            RenderFragment suffix = builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "custom-suffix");
                builder.AddContent(2, ".com");
                builder.CloseElement();
            };

            var cut = Render<ElInput<string>>(parameters => parameters
                .Add(x => x.Value, "element-plus")
                .Add(x => x.PrefixIcon, "el-icon-link")
                .Add(x => x.Prefix, prefix)
                .Add(x => x.SuffixIcon, "el-icon-search")
                .Add(x => x.Suffix, suffix));

            var wrapper = cut.Find(".el-input");

            Assert.Contains("el-input--prefix", wrapper.ClassList);
            Assert.Contains("el-input--suffix", wrapper.ClassList);
            Assert.Equal("https://", cut.Find(".custom-prefix").TextContent);
            Assert.Equal(".com", cut.Find(".custom-suffix").TextContent);
            Assert.Contains("el-icon-link", cut.Find(".el-input__prefix .el-input__icon").ClassList);
            Assert.Contains("el-icon-search", cut.Find(".el-input__suffix .el-input__icon").ClassList);
        }

        [Fact]
        public void FormatterAndParserControlDisplayedAndBoundValue()
        {
            decimal? value = 1234;
            decimal? inputValue = null;

            var cut = Render<ElInput<decimal?>>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.Formatter, next => next.HasValue ? $"$ {next.Value:N0}" : string.Empty)
                .Add(x => x.Parser, next => next.Replace("$", string.Empty).Replace(",", string.Empty).Trim())
                .Add(x => x.ValueChanged, next =>
                {
                    value = next;
                    inputValue = next;
                }));

            Assert.Equal("$ 1,234", cut.Find("input").GetAttribute("value"));

            cut.Find("input").Input("$ 2,500");

            Assert.Equal(2500, inputValue);
        }

        [Fact]
        public void ShowsWordLimitAndExceedState()
        {
            var cut = Render<ElInput<string>>(parameters => parameters
                .Add(x => x.Value, "Element")
                .Add(x => x.Maxlength, 5)
                .Add(x => x.ShowWordLimit, true));

            Assert.Contains("is-exceed", cut.Find(".el-input").ClassList);
            Assert.Equal("7 / 5", cut.Find(".el-input__count-inner").TextContent.Trim());
        }
    }
}
