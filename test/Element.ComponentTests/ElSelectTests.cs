using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Element.ComponentTests
{
    public class ElSelectTests : BunitContext
    {
        public ElSelectTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public async System.Threading.Tasks.Task MultipleSelectRendersTagsAndUpdatesValues()
        {
            IList<string> values = new List<string>();
            var cut = Render<SelectHost>(parameters => parameters
                .Add(x => x.Multiple, true)
                .Add(x => x.Values, values)
                .Add(x => x.ValuesChanged, next => values = (IList<string>)next));

            cut.Find(".el-select").Click();
            var dropdown = GetDropdown();
            var fragment = Render(builder =>
            {
                builder.OpenComponent<CascadingValue<DropDownOption>>(0);
                builder.AddAttribute(1, "Value", dropdown);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(content => content.AddContent(0, dropdown.OptionContent)));
                builder.CloseComponent();
            });
            Assert.Equal(2, fragment.FindAll(".el-select-dropdown__item").Count);

            await cut.InvokeAsync(() => ((ElSelect<string>)dropdown.Select).OnInternalSelectAsync(((ElSelect<string>)dropdown.Select).Options[0]));
            await cut.InvokeAsync(() => ((ElSelect<string>)dropdown.Select).OnInternalSelectAsync(((ElSelect<string>)dropdown.Select).Options[1]));

            Assert.Equal(new[] { "alpha", "beta" }, values);
            Assert.Equal(2, cut.FindAll(".el-select__tags .el-tag").Count);
        }

        [Fact]
        public void FilterableSelectFiltersVisibleOptions()
        {
            var cut = Render<SelectHost>(parameters => parameters.Add(x => x.Filterable, true));

            cut.Find(".el-select").Click();
            var dropdown = GetDropdown();
            Render(builder =>
            {
                builder.OpenComponent<CascadingValue<DropDownOption>>(0);
                builder.AddAttribute(1, "Value", dropdown);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(content => content.AddContent(0, dropdown.OptionContent)));
                builder.CloseComponent();
            });
            cut.Find("input").Input("Be");

            var select = (ElSelect<string>)dropdown.Select;
            cut.WaitForAssertion(() =>
            {
                Assert.False(select.IsOptionVisible(select.Options[0]));
                Assert.True(select.IsOptionVisible(select.Options[1]));
            });
        }

        [Fact]
        public void RemoteSelectInvokesRemoteMethod()
        {
            var filters = new List<string>();
            var cut = Render<SelectHost>(parameters => parameters
                .Add(x => x.Filterable, true)
                .Add(x => x.Remote, true)
                .Add(x => x.RemoteMethod, query => filters.Add(query))
                .Add(x => x.ChildContent, (RenderFragment)(builder => { })));

            cut.Find(".el-select").Click();
            cut.Find("input").Input("remote");

            Assert.Equal(new[] { "remote" }, filters);
        }

        [Fact]
        public void RemoteSelectDoesNotHideExistingOptionsWhileSearching()
        {
            var cut = Render<SelectHost>(parameters => parameters
                .Add(x => x.Filterable, true)
                .Add(x => x.Remote, true));

            cut.Find(".el-select").Click();
            var dropdown = GetDropdown();
            RenderDropdown(dropdown);

            cut.Find("input").Input("missing");
            var select = (ElSelect<string>)dropdown.Select;

            Assert.All(select.Options, option => Assert.True(select.IsOptionVisible(option)));
            Assert.Equal("true", cut.Find(".el-select").GetAttribute("aria-expanded"));
            Assert.Equal("false", cut.Find(".el-select").GetAttribute("aria-disabled"));
            Assert.Equal("false", cut.Find(".el-select").GetAttribute("aria-busy"));
            Assert.Equal("false", cut.Find(".el-select").GetAttribute("aria-multiselectable"));
        }

        [Fact]
        public async System.Threading.Tasks.Task OptionGroupsRenderHeadersAndPreserveSelection()
        {
            string value = null;
            var cut = Render<ElSelect<string>>(parameters => parameters
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.ChildContent, (RenderFragment)(content =>
                {
                    content.OpenComponent<ElOptionGroup<string>>(0);
                    content.AddAttribute(1, nameof(ElOptionGroup<string>.Label), "Backend");
                    content.AddAttribute(2, nameof(ElOptionGroup<string>.ChildContent), (RenderFragment)(group =>
                    {
                        group.OpenComponent<ElOption<string>>(0);
                        group.AddAttribute(1, nameof(ElOption<string>.Value), "api");
                        group.AddAttribute(2, nameof(ElOption<string>.Text), "API");
                        group.CloseComponent();
                    }));
                    content.CloseComponent();

                    content.OpenComponent<ElOptionGroup<string>>(3);
                    content.AddAttribute(4, nameof(ElOptionGroup<string>.Label), "Frontend");
                    content.AddAttribute(5, nameof(ElOptionGroup<string>.ChildContent), (RenderFragment)(group =>
                    {
                        group.OpenComponent<ElOption<string>>(0);
                        group.AddAttribute(1, nameof(ElOption<string>.Value), "ui");
                        group.AddAttribute(2, nameof(ElOption<string>.Text), "UI");
                        group.CloseComponent();
                    }));
                    content.CloseComponent();
                })));

            cut.Find(".el-select").Click();
            var dropdown = GetDropdown();
            var fragment = Render(builder =>
            {
                builder.OpenComponent<CascadingValue<DropDownOption>>(0);
                builder.AddAttribute(1, "Value", dropdown);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(content => content.AddContent(0, dropdown.OptionContent)));
                builder.CloseComponent();
            });

            var titles = fragment.FindAll(".el-select-group__title");
            Assert.Equal(new[] { "Backend", "Frontend" }, new[] { titles[0].TextContent.Trim(), titles[1].TextContent.Trim() });

            await cut.InvokeAsync(() => ((ElSelect<string>)dropdown.Select).OnInternalSelectAsync(((ElSelect<string>)dropdown.Select).Options.Last()));
            Assert.Equal("ui", value);
        }

        [Fact]
        public void OptionGroupKeepsStructureAndHidesFilteredChildren()
        {
            var cut = Render<GroupedSelectHost>();

            cut.Find(".el-select").Click();
            var dropdown = GetDropdown();
            var fragment = RenderDropdown(dropdown);

            Assert.Equal(2, fragment.FindAll(".el-select-group__title").Count);

            cut.Find("input").Input("AP");
            var select = (ElSelect<string>)dropdown.Select;

            var titles = fragment.FindAll(".el-select-group__title");

            Assert.Equal(new[] { "Backend", "Frontend" }, new[] { titles[0].TextContent.Trim(), titles[1].TextContent.Trim() });
            Assert.True(select.IsOptionVisible(select.Options[0]));
            Assert.False(select.IsOptionVisible(select.Options[1]));
        }

        [Fact]
        public void DisabledOptionGroupDisablesChildOptions()
        {
            string value = null;
            var cut = Render<ElSelect<string>>(parameters => parameters
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.ChildContent, (RenderFragment)(content =>
                {
                    content.OpenComponent<ElOptionGroup<string>>(0);
                    content.AddAttribute(1, nameof(ElOptionGroup<string>.Label), "Locked");
                    content.AddAttribute(2, nameof(ElOptionGroup<string>.Disabled), true);
                    content.AddAttribute(3, nameof(ElOptionGroup<string>.ChildContent), (RenderFragment)(group =>
                    {
                        group.OpenComponent<ElOption<string>>(0);
                        group.AddAttribute(1, nameof(ElOption<string>.Value), "locked");
                        group.AddAttribute(2, nameof(ElOption<string>.Text), "Locked option");
                        group.CloseComponent();
                    }));
                    content.CloseComponent();
                })));

            cut.Find(".el-select").Click();
            var dropdown = GetDropdown();
            var fragment = RenderDropdown(dropdown);
            var option = fragment.Find(".el-select-dropdown__item");

            Assert.Contains("is-disabled", option.ClassList);
            Assert.Equal("true", option.GetAttribute("aria-disabled"));
            Assert.Equal("-1", option.GetAttribute("tabindex"));

            option.Click();

            Assert.Null(value);
        }

        private DropDownOption GetDropdown()
        {
            return Services.GetRequiredService<PopupService>().SelectDropDownOptions.Single();
        }

        private IRenderedComponent<CascadingValue<DropDownOption>> RenderDropdown(DropDownOption dropdown)
        {
            return Render<CascadingValue<DropDownOption>>(parameters => parameters
                .Add(x => x.Value, dropdown)
                .Add(x => x.ChildContent, (RenderFragment)(content => content.AddContent(0, dropdown.OptionContent))));
        }

        private class SelectHost : ComponentBase
        {
            [Parameter]
            public bool Multiple { get; set; }

            [Parameter]
            public bool Filterable { get; set; }

            [Parameter]
            public bool Remote { get; set; }

            [Parameter]
            public IList<string> Values { get; set; } = new List<string>();

            [Parameter]
            public EventCallback<ICollection<string>> ValuesChanged { get; set; }

            [Parameter]
            public EventCallback<string> RemoteMethod { get; set; }

            [Parameter]
            public RenderFragment ChildContent { get; set; }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<ElSelect<string>>(0);
                builder.AddAttribute(1, nameof(ElSelect<string>.Multiple), Multiple);
                builder.AddAttribute(2, nameof(ElSelect<string>.Filterable), Filterable);
                builder.AddAttribute(3, nameof(ElSelect<string>.Remote), Remote);
                builder.AddAttribute(4, nameof(ElSelect<string>.Values), Values);
                builder.AddAttribute(5, nameof(ElSelect<string>.ValuesChanged), ValuesChanged);
                builder.AddAttribute(6, nameof(ElSelect<string>.RemoteMethod), RemoteMethod);
                builder.AddAttribute(7, nameof(ElSelect<string>.ChildContent), ChildContent ?? (RenderFragment)(content =>
                {
                    content.OpenComponent<ElOption<string>>(0);
                    content.AddAttribute(1, nameof(ElOption<string>.Value), "alpha");
                    content.AddAttribute(2, nameof(ElOption<string>.Text), "Alpha");
                    content.CloseComponent();

                    content.OpenComponent<ElOption<string>>(3);
                    content.AddAttribute(4, nameof(ElOption<string>.Value), "beta");
                    content.AddAttribute(5, nameof(ElOption<string>.Text), "Beta");
                    content.CloseComponent();
                }));
                builder.CloseComponent();
            }
        }

        private class GroupedSelectHost : ComponentBase
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<ElSelect<string>>(0);
                builder.AddAttribute(1, nameof(ElSelect<string>.Filterable), true);
                builder.AddAttribute(2, nameof(ElSelect<string>.ChildContent), (RenderFragment)(content =>
                {
                    content.OpenComponent<ElOptionGroup<string>>(0);
                    content.AddAttribute(1, nameof(ElOptionGroup<string>.Label), "Backend");
                    content.AddAttribute(2, nameof(ElOptionGroup<string>.ChildContent), (RenderFragment)(group =>
                    {
                        group.OpenComponent<ElOption<string>>(0);
                        group.AddAttribute(1, nameof(ElOption<string>.Value), "api");
                        group.AddAttribute(2, nameof(ElOption<string>.Text), "API");
                        group.CloseComponent();
                    }));
                    content.CloseComponent();

                    content.OpenComponent<ElOptionGroup<string>>(3);
                    content.AddAttribute(4, nameof(ElOptionGroup<string>.Label), "Frontend");
                    content.AddAttribute(5, nameof(ElOptionGroup<string>.ChildContent), (RenderFragment)(group =>
                    {
                        group.OpenComponent<ElOption<string>>(0);
                        group.AddAttribute(1, nameof(ElOption<string>.Value), "ui");
                        group.AddAttribute(2, nameof(ElOption<string>.Text), "UI");
                        group.CloseComponent();
                    }));
                    content.CloseComponent();
                }));
                builder.CloseComponent();
            }
        }
    }
}
