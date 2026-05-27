using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Element.ComponentTests
{
    public class ElCascaderTests : BunitContext
    {
        public ElCascaderTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public async Task MultipleSelectionTracksSelectedPaths()
        {
            IList<IList<string>> selected = new List<IList<string>>();
            var cut = Render<ElCascader>(parameters => parameters
                .Add(x => x.Multiple, true)
                .Add(x => x.Options, BuildOptions())
                .Add(x => x.ValuesChanged, paths => selected = paths));

            await OpenAsync(cut);
            var dropdown = GetDropdown();
            Render(dropdown.OptionContent).FindAll("li")[0].Click();
            Render(dropdown.OptionContent).FindAll("li")[1].Click();

            Assert.Collection(selected,
                path => Assert.Equal(new[] { "guide", "install" }, path));
        }

        [Fact]
        public async Task LazyLoadAddsChildrenWhenNodeExpands()
        {
            var root = new CascaderOption { Label = "Remote", Value = "remote" };
            var loadCount = 0;
            var cut = Render<ElCascader>(parameters => parameters
                .Add(x => x.Lazy, true)
                .Add(x => x.Options, new[] { root })
                .Add(x => x.LazyLoad, option =>
                {
                    loadCount++;
                    return Task.FromResult<IEnumerable<CascaderOption>>(new[]
                    {
                        new CascaderOption { Label = "Loaded", Value = "loaded", Leaf = true }
                    });
                }));

            await OpenAsync(cut);
            var dropdown = GetDropdown();
            Render(dropdown.OptionContent).Find("li").Click();

            cut.WaitForAssertion(() =>
            {
                Assert.Equal(1, loadCount);
                Assert.Single(root.Children);
                Assert.Equal("loaded", root.Children[0].Value);
            });
        }

        [Fact]
        public async Task FilterableRendersSuggestionTemplate()
        {
            RenderFragment<CascaderSuggestion> template = suggestion => builder =>
            {
                builder.OpenElement(0, "strong");
                builder.AddAttribute(1, "class", "custom-cascader-suggestion");
                builder.AddContent(2, suggestion.Text);
                builder.CloseElement();
            };
            var cut = Render<ElCascader>(parameters => parameters
                .Add(x => x.Filterable, true)
                .Add(x => x.Options, BuildOptions())
                .Add(x => x.SuggestionItemTemplate, template));

            await OpenAsync(cut);
            cut.Find("input").Input("Install");

            var fragment = Render(GetDropdown().OptionContent);
            Assert.Contains("custom-cascader-suggestion", fragment.Markup);
            Assert.Contains("Guide / Install", fragment.Markup);
        }

        [Fact]
        public async Task RendersNodeTemplate()
        {
            RenderFragment<CascaderOption> template = option => builder =>
            {
                builder.OpenElement(0, "strong");
                builder.AddAttribute(1, "class", "custom-cascader-node");
                builder.AddContent(2, option.Label);
                builder.CloseElement();
            };
            var cut = Render<ElCascader>(parameters => parameters
                .Add(x => x.Options, BuildOptions())
                .Add(x => x.NodeTemplate, template));

            await OpenAsync(cut);
            var fragment = Render(GetDropdown().OptionContent);

            Assert.Contains("custom-cascader-node", fragment.Markup);
            Assert.Contains("Guide", fragment.Markup);
        }

        private async Task OpenAsync(IRenderedComponent<ElCascader> cut)
        {
            await cut.InvokeAsync(() => cut.Find(".el-cascader").Click());
        }

        private DropDownOption GetDropdown()
        {
            return Services.GetRequiredService<PopupService>().SelectDropDownOptions.Single();
        }

        private static IEnumerable<CascaderOption> BuildOptions()
        {
            return new[]
            {
                new CascaderOption
                {
                    Label = "Guide",
                    Value = "guide",
                    Children = new List<CascaderOption>
                    {
                        new CascaderOption { Label = "Install", Value = "install", Leaf = true },
                        new CascaderOption { Label = "Upgrade", Value = "upgrade", Leaf = true }
                    }
                }
            };
        }
    }
}
