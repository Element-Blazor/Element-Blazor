using Bunit;
using Element;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace Element.ComponentTests
{
    public class ElSelectV2Tests : BunitContext
    {
        public ElSelectV2Tests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void RendersVirtualizedOptionsAndSelectsValue()
        {
            int value = 0;
            var cut = Render<ElSelectV2<int>>(parameters => parameters
                .Add(x => x.Options, Enumerable.Range(1, 1000).Select(i => new SelectV2Option
                {
                    Label = $"Option {i}",
                    Value = i
                }))
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find(".el-select-v2").Click();
            cut.FindAll(".el-select-dropdown__item").First().Click();

            Assert.Equal(1, value);
        }

        [Fact]
        public void FiltersVirtualizedOptions()
        {
            var cut = Render<ElSelectV2<int>>(parameters => parameters
                .Add(x => x.Filterable, true)
                .Add(x => x.Options, Enumerable.Range(1, 30).Select(i => new SelectV2Option
                {
                    Label = $"Option {i}",
                    Value = i
                })));

            cut.Find(".el-select-v2").Click();
            cut.Find("input").Input("Option 29");

            var items = cut.FindAll(".el-select-dropdown__item");
            Assert.Single(items);
            Assert.Contains("Option 29", items[0].TextContent);
        }

        [Fact]
        public void DisabledOptionDoesNotChangeValue()
        {
            int value = 0;
            var cut = Render<ElSelectV2<int>>(parameters => parameters
                .Add(x => x.Options, new[]
                {
                    new SelectV2Option { Label = "Locked", Value = 1, Disabled = true }
                })
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find(".el-select-v2").Click();
            cut.Find(".el-select-dropdown__item").Click();

            Assert.Equal(0, value);
        }

        [Fact]
        public void ChangingCanCancelSelection()
        {
            int value = 0;
            var cut = Render<ElSelectV2<int>>(parameters => parameters
                .Add(x => x.Options, new[]
                {
                    new SelectV2Option { Label = "Option 1", Value = 1 }
                })
                .Add(x => x.OnChanging, args => args.DisallowChange = true)
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find(".el-select-v2").Click();
            cut.Find(".el-select-dropdown__item").Click();

            Assert.Equal(0, value);
        }

        [Fact]
        public void ClearableClearsSelectedValue()
        {
            int? value = 1;
            var cut = Render<ElSelectV2<int?>>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.Options, new[]
                {
                    new SelectV2Option { Label = "Option 1", Value = 1 }
                })
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find(".el-select-v2__clear").Click();

            Assert.Null(value);
        }
    }
}
