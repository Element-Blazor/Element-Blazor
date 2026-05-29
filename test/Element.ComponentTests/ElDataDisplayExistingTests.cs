using Bunit;
using Element;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Element.ComponentTests
{
    public class ElDataDisplayExistingTests : BunitContext
    {
        public ElDataDisplayExistingTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void TagSupportsEffectRoundHitDisableTransitionsAndCloseAlias()
        {
            var closed = 0;
            var cut = Render<ElTag>(parameters => parameters
                .Add(x => x.Type, TagType.Warning)
                .Add(x => x.Effect, TagEffect.Plain)
                .Add(x => x.Round, true)
                .Add(x => x.Hit, true)
                .Add(x => x.DisableTransitions, true)
                .Add(x => x.Closable, true)
                .Add(x => x.OnClose, () => closed++)
                .AddChildContent("Alpha"));

            var tag = cut.Find(".el-tag");
            Assert.Contains("el-tag--plain", tag.ClassList);
            Assert.Contains("is-round", tag.ClassList);
            Assert.Contains("is-hit", tag.ClassList);
            Assert.Contains("el-tag--no-transition", tag.ClassList);

            cut.Find(".el-tag__close").Click();

            Assert.Equal(1, closed);
            Assert.Empty(cut.Markup.Trim());
        }

        [Fact]
        public async Task PaginationSupportsLayoutSizesAndJumper()
        {
            var current = 2;
            var pageSize = 10;
            var cut = Render<ElPagination>(parameters => parameters
                .Add(x => x.Total, 95)
                .Add(x => x.PageSize, pageSize)
                .Add(x => x.CurrentPage, current)
                .Add(x => x.Layout, "total, sizes, prev, pager, next, jumper")
                .Add(x => x.PageSizes, new[] { 10, 20, 50 })
                .Add(x => x.CurrentPageChanged, page =>
                {
                    current = page;
                    return Task.CompletedTask;
                })
                .Add(x => x.PageSizeChanged, size => pageSize = size));

            Assert.Equal("共 95 条", cut.Find(".el-pagination__total").TextContent.Trim());
            Assert.Equal("10", cut.Find(".el-pagination__sizes-select").GetAttribute("value"));
            Assert.NotEmpty(cut.FindAll(".el-pagination__jump"));

            cut.Find(".el-pagination__sizes-select").Change("20");

            Assert.Equal(20, pageSize);
            Assert.Equal(1, current);

            cut.Find(".el-pagination__editor").Change("4");

            Assert.Equal(4, current);
            Assert.Contains("active", cut.FindAll(".el-pager .number").First(x => x.TextContent.Trim() == "4").ClassList);
        }
    }
}
