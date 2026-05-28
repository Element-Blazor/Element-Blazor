using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Element.ComponentTests
{
    public class ElTransferTests : BunitContext
    {
        public ElTransferTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void FiltersItemsWithBuiltInAndCustomFilter()
        {
            var cut = Render<ElTransfer>(parameters => parameters
                .Add(x => x.Filterable, true)
                .Add(x => x.NoMatchText, "No result")
                .Add(x => x.List1, Items()));

            cut.Find(".el-transfer-panel__filter input").Input("alp");

            Assert.Contains("Alpha", cut.Markup);
            Assert.DoesNotContain("Beta", cut.Markup);

            var custom = Render<ElTransfer>(parameters => parameters
                .Add(x => x.Filterable, true)
                .Add(x => x.NoMatchText, "No result")
                .Add(x => x.List1, Items())
                .Add(x => x.FilterMethod, (keyword, item) => item.Id == keyword));
            custom.Find(".el-transfer-panel__filter input").Input("b");

            Assert.DoesNotContain("Alpha", custom.Markup);
            Assert.Contains("Beta", custom.Markup);

            custom.Find(".el-transfer-panel__filter input").Input("missing");

            Assert.Contains("No result", custom.Markup);
        }

        [Fact]
        public void RendersItemAndPanelSlotsWithContext()
        {
            var cut = Render<ElTransfer>(parameters => parameters
                .Add(x => x.LeftTitle, "Available")
                .Add(x => x.List1, Items())
                .Add(x => x.ItemContent, item => builder =>
                {
                    builder.OpenElement(0, "span");
                    builder.AddAttribute(1, "class", "custom-item");
                    builder.AddContent(2, $"{item.Id}:{item.Label}");
                    builder.CloseElement();
                })
                .Add(x => x.LeftHeaderContent, context => builder =>
                {
                    builder.OpenElement(0, "strong");
                    builder.AddAttribute(1, "class", "custom-header");
                    builder.AddContent(2, $"{context.Title}:{context.VisibleCount}/{context.TotalCount}");
                    builder.CloseElement();
                })
                .Add(x => x.LeftFooterContent, context => builder =>
                {
                    builder.OpenElement(0, "span");
                    builder.AddAttribute(1, "class", "custom-footer");
                    builder.AddContent(2, context.Direction);
                    builder.CloseElement();
                }));

            Assert.Equal("Available:2/2", cut.Find(".custom-header").TextContent.Trim());
            Assert.Equal(new[] { "a:Alpha", "b:Beta" }, cut.FindAll(".custom-item").Select(x => x.TextContent.Trim()).ToArray());
            Assert.Equal("left", cut.Find(".custom-footer").TextContent.Trim());
        }

        [Fact]
        public void DirectionButtonTextsRenderAndButtonsDisableWhenNothingChecked()
        {
            var cut = Render<ElTransfer>(parameters => parameters
                .Add(x => x.List1, Items())
                .Add(x => x.ButtonTexts, new[] { "Back", "Add" }));

            var buttons = cut.FindAll(".el-transfer__button");

            Assert.Contains("Back", buttons[0].TextContent);
            Assert.Contains("Add", buttons[1].TextContent);
            Assert.True(buttons[0].HasAttribute("disabled"));
            Assert.True(buttons[1].HasAttribute("disabled"));
        }

        [Fact]
        public void ItemCheckboxMovesSingleEnabledItem()
        {
            List<string> value = null;
            var cut = Render<ElTransfer>(parameters => parameters
                .Add(x => x.List1, Items())
                .Add(x => x.ValueChanged, next => value = next));

            cut.FindAll(".el-transfer-panel__item input")[0].Change(true);

            var buttons = cut.FindAll(".el-transfer__button");
            Assert.False(buttons[1].HasAttribute("disabled"));

            buttons[1].Click();

            Assert.Equal(new[] { "a" }, value);
            Assert.Contains("Alpha", cut.FindAll(".el-transfer-panel")[1].TextContent);
            Assert.Contains("Beta", cut.FindAll(".el-transfer-panel")[0].TextContent);
        }

        [Fact]
        public void ValueParameterInitializesTargetPanel()
        {
            var cut = Render<ElTransfer>(parameters => parameters
                .Add(x => x.List1, Items())
                .Add(x => x.Value, new List<string> { "b" }));

            var panels = cut.FindAll(".el-transfer-panel");

            Assert.DoesNotContain("Beta", panels[0].TextContent);
            Assert.Contains("Beta", panels[1].TextContent);
        }

        [Fact]
        public void SelectAllUsesVisibleEnabledItemsOnly()
        {
            List<string> value = null;
            var items = new List<TransferItem>
            {
                new TransferItem { Id = "a", Label = "Alpha" },
                new TransferItem { Id = "b", Label = "Beta", IsDisabled = true },
                new TransferItem { Id = "g", Label = "Gamma" }
            };
            var cut = Render<ElTransfer>(parameters => parameters
                .Add(x => x.Filterable, true)
                .Add(x => x.List1, items)
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find(".el-transfer-panel__filter input").Input("a");
            cut.Find(".el-transfer-panel__header input").Change(true);

            Assert.Equal("true", cut.Find(".el-transfer-panel__header label").GetAttribute("aria-checked"));

            cut.FindAll(".el-transfer__button")[1].Click();

            Assert.Equal(new[] { "a", "g" }, value);
            Assert.DoesNotContain("Beta", cut.FindAll(".el-transfer-panel")[1].TextContent);
            Assert.Contains("Beta", cut.FindAll(".el-transfer-panel")[0].TextContent);
        }

        [Fact]
        public void MovingBackUpdatesValueAndKeepsDisabledItems()
        {
            List<string> value = null;
            var cut = Render<ElTransfer>(parameters => parameters
                .Add(x => x.List1, new List<TransferItem>())
                .Add(x => x.List2, new List<TransferItem>
                {
                    new TransferItem { Id = "a", Label = "Alpha" },
                    new TransferItem { Id = "b", Label = "Beta", IsDisabled = true }
                })
                .Add(x => x.ValueChanged, next => value = next));

            var rightHeaderInput = cut.FindAll(".el-transfer-panel__header input")[1];
            rightHeaderInput.Change(true);
            cut.FindAll(".el-transfer__button")[0].Click();

            Assert.Equal(new[] { "b" }, value);
            Assert.Contains("Alpha", cut.FindAll(".el-transfer-panel")[0].TextContent);
            Assert.Contains("Beta", cut.FindAll(".el-transfer-panel")[1].TextContent);
        }

        private static List<TransferItem> Items()
        {
            return new List<TransferItem>
            {
                new TransferItem { Id = "a", Label = "Alpha" },
                new TransferItem { Id = "b", Label = "Beta" }
            };
        }
    }
}
