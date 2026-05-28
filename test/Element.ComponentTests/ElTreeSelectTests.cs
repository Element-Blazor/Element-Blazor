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
    public class ElTreeSelectTests : BunitContext
    {
        public ElTreeSelectTests()
        {
            Services.AddElementServices();
            JSInterop.SetupVoid("setDisabled", _ => true);
        }

        [Fact]
        public void TreeSelectRendersTreeDropdownAndSelectsNode()
        {
            int? value = null;
            var selectedNodes = new List<TreeItemBase>();
            var cut = Render<ElTreeSelect>(parameters => parameters
                .Add(x => x.Data, FlatTreeItems())
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.SelectedNodesChanged, next => selectedNodes = next));

            cut.Find(".el-select").Click();
            var dropdown = GetDropdown();
            var fragment = RenderTreeDropdown(dropdown);

            Assert.True(dropdown.IsTree);
            Assert.IsAssignableFrom<ElTreeSelect>(dropdown.Select);
            Assert.Contains("Root", fragment.Markup);
            Assert.Contains("Leaf", fragment.Markup);

            fragment.FindAll(".el-tree-node__content").Last().Click();

            Assert.Equal(2, value);
            Assert.Equal(new[] { 2 }, selectedNodes.Select(x => x.Id).ToArray());
            Assert.Equal("Root > Leaf", cut.Find("input").GetAttribute("value"));
        }

        [Fact]
        public void ValueParameterInitializesDisplayLabelFromTreePath()
        {
            var cut = Render<ValueHost>();

            Assert.Equal("Root > Leaf", cut.Find("input").GetAttribute("value"));

            cut.Find("button").Click();

            Assert.Equal("Root", cut.Find("input").GetAttribute("value"));
        }

        [Fact]
        public void TreeSelectKeepsInheritedSelectParameters()
        {
            var cut = Render<ElTreeSelect>(parameters => parameters
                .Add(x => x.Data, FlatTreeItems())
                .Add(x => x.Filterable, true)
                .Add(x => x.Clearable, true)
                .Add(x => x.Size, InputSize.Small)
                .Add(x => x.Disabled, true));

            var wrapper = cut.Find(".el-select");

            Assert.Contains("is-filterable", wrapper.ClassList);
            Assert.Contains("is-clearable", wrapper.ClassList);
            Assert.Contains("is-disabled", wrapper.ClassList);
            Assert.Contains("el-select--small", wrapper.ClassList);
            Assert.Equal("true", wrapper.GetAttribute("aria-disabled"));
        }

        [Fact]
        public void TreeSelectSupportsDeclarativeTreeContent()
        {
            int? value = null;
            var cut = Render<ElTreeSelect>(parameters => parameters
                .Add(x => x.TreeContent, DeclarativeTreeContent())
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find(".el-select").Click();
            var fragment = RenderTreeDropdown(GetDropdown());

            fragment.WaitForAssertion(() => Assert.Contains("Declarative leaf", fragment.Markup));

            fragment.FindAll(".el-tree-node__content").Last().Click();

            Assert.Equal(20, value);
            Assert.Equal("Declarative root > Declarative leaf", cut.Find("input").GetAttribute("value"));
        }

        [Fact]
        public void LegacyTreeSingleSelectUsesTreeSelectPipeline()
        {
            var cut = Render<ElTreeSingleSelect>(parameters => parameters
                .Add(x => x.Data, FlatTreeItems()));

            cut.Find(".el-select").Click();
            var dropdown = GetDropdown();
            var fragment = RenderTreeDropdown(dropdown);

            Assert.True(dropdown.IsTree);
            Assert.IsAssignableFrom<ElTreeSelect>(dropdown.Select);
            Assert.Contains("Leaf", fragment.Markup);
        }

        private DropDownOption GetDropdown()
        {
            return Services.GetRequiredService<PopupService>().SelectDropDownOptions.Single();
        }

        private IRenderedComponent<CascadingValue<ElTreeSelect>> RenderTreeDropdown(DropDownOption dropdown)
        {
            return Render<CascadingValue<ElTreeSelect>>(parameters => parameters
                .Add(x => x.Value, (ElTreeSelect)dropdown.Select)
                .Add(x => x.ChildContent, (RenderFragment)(content => content.AddContent(0, dropdown.OptionContent))));
        }

        private static List<TreeItemBase> FlatTreeItems()
        {
            return new List<TreeItemBase>
            {
                new TreeItemBase { Id = 1, ParentId = 0, Text = "Root" },
                new TreeItemBase { Id = 2, ParentId = 1, Text = "Leaf" }
            };
        }

        private static RenderFragment DeclarativeTreeContent()
        {
            return builder =>
            {
                builder.OpenComponent<ElTreeItem>(0);
                builder.AddAttribute(1, nameof(ElTreeItem.Id), 10);
                builder.AddAttribute(2, nameof(ElTreeItem.Text), "Declarative root");
                builder.AddAttribute(3, nameof(ElTreeItem.ChildContent), (RenderFragment)(child =>
                {
                    child.OpenComponent<ElTreeItem>(0);
                    child.AddAttribute(1, nameof(ElTreeItem.Id), 20);
                    child.AddAttribute(2, nameof(ElTreeItem.Text), "Declarative leaf");
                    child.CloseComponent();
                }));
                builder.CloseComponent();
            };
        }

        private class ValueHost : ComponentBase
        {
            private int? value = 2;

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<ElTreeSelect>(0);
                builder.AddAttribute(1, nameof(ElTreeSelect.Data), FlatTreeItems());
                builder.AddAttribute(2, nameof(ElTreeSelect.Value), value);
                builder.CloseComponent();

                builder.OpenElement(3, "button");
                builder.AddAttribute(4, "onclick", EventCallback.Factory.Create(this, () => value = 1));
                builder.AddContent(5, "Change");
                builder.CloseElement();
            }
        }
    }
}
