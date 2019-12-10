using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blazui.Component.Test.TabTests
{
    [TestName("Tabs 标签页", "双向绑定实现可编辑的标签页")]
    public class Test6 : TestBase, IDemoTester
    {
        async Task<List<(string Title, ElementHandle Header)>> GetHeadersAsync(DemoCard card)
        {
            var header = await card.Body.QuerySelectorAsync("div.el-tabs.el-tabs--card.el-tabs--top > div.el-tabs__header.is-top");
            Assert.NotNull(header);
            var tabHeaders = await header.QuerySelectorAllAsync("div.el-tabs__nav-wrap.is-top > div.el-tabs__nav-scroll > div.el-tabs__nav.is-top > div.el-tabs__item.is-top.is-closable");
            var tasks = tabHeaders.Select(async x => ((await x.EvaluateFunctionAsync<string>("m=>m.innerText")).Trim(), x));
            List<(string Title, ElementHandle Header)> headers = (await Task.WhenAll(tasks)).ToList();
            return headers;
        }
        async Task<List<(string Title, ElementHandle Header)>> AssertTabAsync(DemoCard card, List<string> tabs, int activeIndex)
        {
            var body = await card.Body.QuerySelectorAsync("div.el-tabs.el-tabs--card.el-tabs--top > div.el-tabs__content");
            Assert.NotNull(body);
            var headers = await GetHeadersAsync(card);
            var tabHeaders = headers.Select(x => x.Header).ToArray();
            Assert.Equal(tabs.Count, headers.Count);

            for (int i = 0; i < tabs.Count; i++)
            {
                Assert.Equal(tabs[i], headers[i].Title);
            }

            if (!tabs.Any())
            {
                return headers;
            }
            await AssertHeaderAsync(tabHeaders, activeIndex);
            await AssertBodyAsync(body, tabs[activeIndex]);
            await AssertHoverAsync(tabHeaders, activeIndex);
            foreach (var tabHeader in headers)
            {
                await tabHeader.Header.ClickAsync();
                await Task.Delay(100);
                var index = headers.IndexOf(tabHeader);
                await AssertHoverAsync(tabHeaders, index);
                await AssertHeaderAsync(tabHeaders, index);
                await AssertBodyAsync(body, tabHeader.Title);
            }
            return headers;
        }
        public async Task TestAsync(DemoCard card)
        {
            var tabs = new List<string>();
            tabs.Add("选项卡1");
            tabs.Add("卡2");
            tabs.Add("卡3");
            tabs.Add("Component");
            var originTabs = tabs.ToList();
            await Task.Delay(200);
            var headers = await GetHeadersAsync(card);
            await headers[0].Header.ClickAsync();
            await Task.Delay(200);
            ElementHandle closeIcon = null;
            while (true)
            {
                var headerItem = headers.FirstOrDefault();
                if (headerItem.Header == null)
                {
                    break;
                }
                await headerItem.Header.ClickAsync();
                await Task.Delay(200);
                closeIcon = await headerItem.Header.QuerySelectorAsync("span.el-icon-close");
                Assert.NotNull(closeIcon);
                await closeIcon.ClickAsync();
                await Task.Delay(200);
                tabs.RemoveAt(0);
                headers = await GetHeadersAsync(card);
            }
            await AssertEmptyBody(card);
            var newTab = await card.Body.QuerySelectorAsync("button");
            tabs.Add("标题0");
            tabs.Add("标题1");
            tabs.Add("标题2");
            tabs.Add("标题3");
            for (int i = 0; i < 4; i++)
            {
                await newTab.ClickAsync();
                await Task.Delay(200);
            }
            headers = await GetHeadersAsync(card);
            while (true)
            {
                var headerItem = headers.LastOrDefault();
                if (headerItem.Header == null)
                {
                    break;
                }
                await headerItem.Header.ClickAsync();
                await Task.Delay(200);
                closeIcon = await headerItem.Header.QuerySelectorAsync("span.el-icon-close");
                Assert.NotNull(closeIcon);
                await closeIcon.ClickAsync();
                await Task.Delay(200);
                tabs.RemoveAt(tabs.Count - 1);
                headers = await AssertTabAsync(card, tabs, tabs.Count - 1);
            }
            await AssertEmptyBody(card);
            var count = 5;
            await newTab.ClickAsync();
            await Task.Delay(200);
            while (count-- > 0)
            {
                await newTab.ClickAsync();
                await Task.Delay(200);
                headers = await GetHeadersAsync(card);
                Assert.Equal(2, headers.Count);
                tabs = headers.Select(x => x.Title).ToList();
                var tabHeaders = headers.Select(x => x.Header).ToArray();
                await headers[0].Header.ClickAsync();
                await Task.Delay(200);
                await AssertHeaderAsync(tabHeaders, 0);
                closeIcon = await headers[0].Header.QuerySelectorAsync("span.el-icon-close");
                await closeIcon.ClickAsync();
                tabs.RemoveAt(0);
                await Task.Delay(200);
                headers = await GetHeadersAsync(card);
                Assert.Single(headers);
                var body = await card.Body.QuerySelectorAsync("div.el-tabs.el-tabs--card.el-tabs--top > div.el-tabs__content");
                await AssertBodyAsync(body, headers[0].Title);
            }
        }

        private static async Task AssertEmptyBody(DemoCard card)
        {
            var body = await card.Body.QuerySelectorAsync("div.el-tabs.el-tabs--card.el-tabs--top > div.el-tabs__content");
            var bodyContent = await body.EvaluateFunctionAsync<string>("x=>x.innerText");
            Assert.True(string.IsNullOrWhiteSpace(bodyContent));
        }

        private async Task AssertHoverAsync(ElementHandle[] tabHeaders, int activeIndex)
        {
            foreach (var header in tabHeaders)
            {
                await header.HoverAsync();
                await Task.Delay(500);
                var textColor = await header.EvaluateFunctionAsync<string>("x=>window.getComputedStyle(x,null).color");
                Assert.Equal("rgb(64, 158, 255)", textColor);
                var closeIcon = await header.QuerySelectorAsync("span.el-icon-close");
                Assert.NotNull(closeIcon);
            }
        }

        private async Task AssertBodyAsync(ElementHandle body, string text)
        {
            if (text != "Component")
            {

                var bodyText = (await body.EvaluateFunctionAsync<string>("x=>x.innerText")).Trim();
                if (text == "选项1")
                {
                    Assert.Equal("内容1", bodyText);
                }
                else if (text == "卡2")
                {
                    Assert.Equal("内容2", bodyText);
                }
                else if (text == "卡3")
                {
                    Assert.Equal("内容3", bodyText);
                }
                else if (text == "标题0")
                {
                    Assert.Equal("内容0", bodyText);
                }
                else if (text == "标题1")
                {
                    Assert.Equal("内容1", bodyText);
                }
                else if (text == "标题2")
                {
                    Assert.Equal("内容2", bodyText);
                }
                else if (text == "标题3")
                {
                    Assert.Equal("内容3", bodyText);
                }
                else if (text == "标题3")
                {
                    Assert.Equal("内容3", bodyText);
                }
                else if (text == "标题4")
                {
                    Assert.Equal("内容4", bodyText);
                }
                else if (text == "标题5")
                {
                    Assert.Equal("内容5", bodyText);
                }
            }
            else
            {
                var input = await body.QuerySelectorAsync("label.el-checkbox > span.el-checkbox__input > input");
                Assert.NotNull(input);
            }
        }

        private async Task AssertHeaderAsync(ElementHandle[] tabHeaders, int index)
        {
            var activeTab = tabHeaders[index];
            var activeBoxModel = await activeTab.BoundingBoxAsync();
            Assert.NotNull(activeBoxModel);
            var closeIcon = await activeTab.QuerySelectorAsync("span.el-icon-close");
            Assert.NotNull(closeIcon);
            var cls = await activeTab.EvaluateFunctionAsync<string>("x=>x.className");
            Assert.Equal("el-tabs__item is-top is-closable is-active", cls);
            foreach (var tabHeader in tabHeaders)
            {
                closeIcon = await activeTab.QuerySelectorAsync("span.el-icon-close");
                Assert.NotNull(closeIcon);
                cls = await tabHeader.EvaluateFunctionAsync<string>("x=>x.className");
                if (activeTab == tabHeader)
                {
                    Assert.Equal("el-tabs__item is-top is-closable is-active", cls);
                    continue;
                }
                Assert.Equal("el-tabs__item is-top is-closable ", cls);
            }
        }
    }
}
