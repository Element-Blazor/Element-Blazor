using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Element.Test.TabTests
{
    [TestName("Tabs 标签页", "在左边的标签页")]
    public class Test4 : IDemoTester
    {
        public async Task TestAsync(DemoCard card)
        {
            var header = await card.Body.QuerySelectorAsync("div.el-tabs.el-tabs--border-card.el-tabs--left > div.el-tabs__header.is-left");
            var body = await card.Body.QuerySelectorAsync("div.el-tabs.el-tabs--border-card.el-tabs--left > div.el-tabs__content");
            Assert.NotNull(header);
            Assert.NotNull(body);
            var tabHeaders = await header.QuerySelectorAllAsync("div.el-tabs__nav-wrap.is-left > div.el-tabs__nav-scroll > div.el-tabs__nav.is-left > div.el-tabs__item.is-left");
            var tasks = tabHeaders.Select(async x => new
            {
                Title = await x.EvaluateFunctionAsync<string>("m=>m.innerText"),
                Header = x
            });
            var headers = (await Task.WhenAll(tasks)).ToList();
            Assert.Equal(4, headers.Count);
            Assert.Equal("用户管理", headers[0].Title);
            Assert.Equal("角色管理", headers[1].Title);
            Assert.Equal("部门管理", headers[2].Title);
            Assert.Equal("人员管理", headers[3].Title);

            await AssertHeaderAsync(tabHeaders, 0);
            await AssertBodyAsync(body, "用户管理1");
            await AssertHoverAsync(tabHeaders[0]);
            foreach (var tabHeader in headers.Skip(1))
            {
                await tabHeader.Header.ClickAsync();
                await Task.Delay(100);
                var index = headers.IndexOf(tabHeader);
                await AssertHoverAsync(tabHeader.Header);
                await AssertHeaderAsync(tabHeaders, index);
                await AssertBodyAsync(body, $"{tabHeader.Title}1");
            }
        }

        private async Task AssertHoverAsync(ElementHandle tabHeader)
        {
            await tabHeader.HoverAsync();
            await Task.Delay(200);
            var textColor = await tabHeader.EvaluateFunctionAsync<string>("x=>window.getComputedStyle(x,null).color");
            Assert.Equal("rgb(64, 158, 255)", textColor);
        }

        private async Task AssertBodyAsync(ElementHandle body, string text)
        {
            var bodyText = await body.EvaluateFunctionAsync<string>("x=>x.innerText");
            Assert.Equal(text, bodyText?.Trim());
        }

        private async Task AssertHeaderAsync(ElementHandle[] tabHeaders, int index)
        {
            var activeTab = tabHeaders[index];
            var activeBoxModel = await activeTab.BoundingBoxAsync();
            Assert.NotNull(activeBoxModel);
            var cls = await activeTab.EvaluateFunctionAsync("x=>x.className");
            Assert.Equal("el-tabs__item is-left  is-active", cls);
            foreach (var tabHeader in tabHeaders)
            {
                if (activeTab == tabHeader)
                {
                    continue;
                }
                cls = await tabHeader.EvaluateFunctionAsync("x=>x.className");
                Assert.Equal("el-tabs__item is-left  ", cls);
            }
        }
    }
}
