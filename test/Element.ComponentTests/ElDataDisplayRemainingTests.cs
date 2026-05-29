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
    public class ElDataDisplayRemainingTests : BunitContext
    {
        public ElDataDisplayRemainingTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void InfiniteScrollRaisesLoadOnScrollAndShowsStates()
        {
            var loads = new List<int>();
            var cut = Render<ElInfiniteScroll>(parameters => parameters
                .Add(x => x.Height, "120")
                .Add(x => x.Delay, 0)
                .Add(x => x.OnLoad, args => loads.Add(args.LoadCount))
                .AddChildContent("<div>Rows</div>"));

            Assert.Contains("height:120px", cut.Find(".el-infinite-scroll").GetAttribute("style"));
            cut.Find(".el-infinite-scroll").TriggerEvent("onscroll", EventArgs.Empty);
            cut.Find(".el-infinite-scroll").TriggerEvent("onscroll", EventArgs.Empty);

            Assert.Equal(new[] { 1, 2 }, loads);

            cut = Render<ElInfiniteScroll>(parameters => parameters
                .Add(x => x.Loading, true)
                .Add(x => x.Disabled, true)
                .AddChildContent("<div>Rows</div>"));

            Assert.Contains("is-loading", cut.Find(".el-infinite-scroll").ClassList);
            Assert.Equal("加载中", cut.Find(".el-infinite-scroll__loading").TextContent.Trim());
            Assert.Equal("没有更多了", cut.Find(".el-infinite-scroll__disabled").TextContent.Trim());
        }

        [Fact]
        public void TourStepsNavigateAndClose()
        {
            var open = true;
            var current = 0;
            var closed = 0;
            var changes = new List<int>();
            var steps = new List<TourStep>
            {
                new TourStep { Title = "Step 1", Description = "First", Placement = "right" },
                new TourStep { Title = "Step 2", Description = "Second" }
            };

            var cut = Render<ElTour>(parameters => parameters
                .Add(x => x.ModelValue, open)
                .Add(x => x.ModelValueChanged, next => open = next)
                .Add(x => x.Current, current)
                .Add(x => x.CurrentChanged, next => current = next)
                .Add(x => x.OnChange, next => changes.Add(next))
                .Add(x => x.OnClose, () => closed++)
                .Add(x => x.Steps, steps));

            Assert.Equal("Step 1", cut.Find(".el-tour__title").TextContent.Trim());
            Assert.Contains("is-right", cut.Find(".el-tour__content").ClassList);

            cut.FindAll(".el-tour__actions button")[1].Click();

            Assert.Equal(1, current);
            Assert.Equal(1, changes.Single());
            Assert.Equal("Step 2", cut.Find(".el-tour__title").TextContent.Trim());

            cut.FindAll(".el-tour__actions button")[1].Click();

            Assert.False(open);
            Assert.Equal(1, closed);
            Assert.Empty(cut.FindAll(".el-tour"));
        }

        [Fact]
        public void TreeFiltersTemplatesChecksLazyLoadsAndDrags()
        {
            var checkedNodes = new List<TreeItemBase>();
            TreeDragEventArgs dropped = null;
            var data = new List<TreeItemBase>
            {
                new TreeItemBase { Id = 1, ParentId = 0, Text = "Root", Expanded = true },
                new TreeItemBase { Id = 2, ParentId = 1, Text = "Alpha" },
                new TreeItemBase { Id = 3, ParentId = 1, Text = "Beta", HasChildren = true }
            };

            var cut = Render<ElTree>(parameters => parameters
                .Add(x => x.DataSource, data)
                .Add(x => x.FilterText, "Alpha")
                .Add(x => x.ShowCheckbox, true)
                .Add(x => x.NodeTemplate, node => builder =>
                {
                    builder.OpenElement(0, "strong");
                    builder.AddContent(1, node.Text);
                    builder.CloseElement();
                })
                .Add(x => x.CheckedNodesChanged, next => checkedNodes = next)
                .Add(x => x.Draggable, true)
                .Add(x => x.OnNodeDrop, args => dropped = args));

            Assert.Contains("Root", cut.Markup);
            Assert.Contains("Alpha", cut.Markup);
            Assert.DoesNotContain("Beta", cut.Markup);
            Assert.NotEmpty(cut.FindAll(".el-tree-node__label strong"));

            cut.FindAll(".el-checkbox__original")[0].Change(true);

            Assert.Contains(1, checkedNodes.Select(x => x.Id));
            Assert.Contains(2, checkedNodes.Select(x => x.Id));

            cut.FindAll(".el-tree-node")[1].DragStart(new Microsoft.AspNetCore.Components.Web.DragEventArgs());
            cut.FindAll(".el-tree-node")[0].Drop(new Microsoft.AspNetCore.Components.Web.DragEventArgs());

            Assert.NotNull(dropped);
            Assert.Equal(2, dropped.DraggingNode.Id);
            Assert.Equal(1, dropped.DropNode.Id);

            var loaded = false;
            var lazyData = new List<TreeItemBase>
            {
                new TreeItemBase { Id = 10, ParentId = 0, Text = "Lazy Root", Expanded = true },
                new TreeItemBase { Id = 11, ParentId = 10, Text = "Lazy Parent", HasChildren = true }
            };

            cut = Render<ElTree>(parameters => parameters
                .Add(x => x.DataSource, lazyData)
                .Add(x => x.FilterText, null)
                .Add(x => x.Lazy, true)
                .Add(x => x.LazyLoad, node =>
                {
                    loaded = true;
                    return Task.FromResult<IEnumerable<TreeItemBase>>(new[]
                    {
                        new TreeItemBase { Id = 4, Text = "Loaded" }
                    });
                }));

            cut.FindAll(".el-tree-node")
                .First(x => x.TextContent.Contains("Lazy Parent"))
                .QuerySelector(".el-tree-node__expand-icon")
                .Click();

            Assert.True(loaded);
            Assert.Contains("Loaded", cut.Markup);
        }

        [Fact]
        public void TableV2VirtualizesExpandsAndSummarizesRows()
        {
            var rows = Enumerable.Range(1, 8)
                .Select(i => new ProductRow { Name = $"Item {i}", Amount = i })
                .ToList();
            var expanded = new HashSet<object>();

            var cut = Render<ElTableV2>(parameters => parameters
                .Add(x => x.DataSource, rows)
                .Add(x => x.StartIndex, 2)
                .Add(x => x.ItemCount, 3)
                .Add(x => x.ShowSummary, true)
                .Add(x => x.ExpandedRows, expanded)
                .Add(x => x.ExpandContent, row => builder =>
                {
                    builder.OpenElement(0, "span");
                    builder.AddAttribute(1, "class", "expanded-product");
                    builder.AddContent(2, ((ProductRow)row).Name);
                    builder.CloseElement();
                }));

            Assert.DoesNotContain("Item 1", cut.Markup);
            Assert.Contains("Item 3", cut.Markup);
            Assert.Contains("Item 5", cut.Markup);
            Assert.Contains("36", cut.Find(".el-table-v2__summary-row").TextContent);

            cut.Find(".el-table__expand-icon").Click();

            Assert.Contains("expanded-product", cut.Markup);
            Assert.Contains("el-table__expand-icon--expanded", cut.Find(".el-table__expand-icon").ClassList);
        }

        [Fact]
        public void TreeV2UsesTreePipeline()
        {
            var cut = Render<ElTreeV2>(parameters => parameters
                .Add(x => x.DataSource, new List<TreeItemBase>
                {
                    new TreeItemBase { Id = 1, ParentId = 0, Text = "Root" }
                }));

            Assert.Contains("el-tree", cut.Find("[role='tree']").ClassList);
            Assert.Contains("Root", cut.Markup);
        }

        private class ProductRow
        {
            public string Name { get; set; }

            public int Amount { get; set; }
        }
    }
}
