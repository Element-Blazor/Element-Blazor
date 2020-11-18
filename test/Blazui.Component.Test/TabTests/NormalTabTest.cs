using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blazui.Component.Test.TabTests
{
    [TestName("Tabs 标签页", "基础的、简洁的标签页")]
    public class NormalTabTest : IDemoTester
    {
        public async Task TestAsync(DemoCard card)
        {
            var header = await card.Body.QuerySelectorAsync("div.el-tabs.el-tabs--top > div.el-tabs__header.is-top");
            var body = await card.Body.QuerySelectorAsync("div.el-tabs.el-tabs--top > div.el-tabs__content");
            Assert.NotNull(header);
            Assert.NotNull(body);
            var navElement = await header.QuerySelectorAsync("div.el-tabs__nav-wrap > div.el-tabs__nav-scroll > div.el-tabs__nav");
            Assert.NotNull(navElement);
            var tabHeaders = await header.QuerySelectorAllAsync("div.el-tabs__nav-wrap.is-top > div.el-tabs__nav-scroll > div.el-tabs__nav.is-top > div.el-tabs__item.is-top");
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

            await AssertHeaderAsync(navElement, tabHeaders, 0);
            await AssertBodyAsync(body, "用户管理1");
            await AssertHoverAsync(tabHeaders[0]);
            foreach (var tabHeader in headers.Skip(1))
            {
                await tabHeader.Header.ClickAsync();
                await Task.Delay(100);
                var index = headers.IndexOf(tabHeader);
                await AssertHoverAsync(tabHeader.Header);
                await AssertHeaderAsync(navElement, tabHeaders, index);
                await AssertBodyAsync(body, $"{tabHeader.Title}1");
            }
        }

        private static async Task AssertHoverAsync(ElementHandle tabHeader)
        {
            await tabHeader.HoverAsync();
            var textColor = await tabHeader.EvaluateFunctionAsync<string>("x=>window.getComputedStyle(x,null).color");
            Assert.Equal("rgb(64, 158, 255)", textColor);
        }

        private static async Task AssertBodyAsync(ElementHandle body, string text)
        {
            var bodyText = await body.EvaluateFunctionAsync<string>("x=>x.innerText");
            Assert.Equal(text, bodyText?.Trim());
        }

        private static async Task AssertHeaderAsync(ElementHandle navElement, ElementHandle[] tabHeaders, int index)
        {
            var shadow = await navElement.QuerySelectorAsync("div.el-tabs__active-bar");
            Assert.NotNull(shadow);
            var shadowBoxModel = await shadow.BoundingBoxAsync();
            Assert.NotNull(shadowBoxModel);
            var activeTab = tabHeaders[index];
            var activeBoxModel = await activeTab.BoundingBoxAsync();
            Assert.NotNull(activeBoxModel);
            var barOffsetLeft = await activeTab.EvaluateFunctionAsync<float>("x=>x.offsetLeft+parseFloat(window.getComputedStyle(x).paddingLeft)");
            var shadowBoxWidth = Math.Round(shadowBoxModel.Width + 20 + ((index == 0 || index == tabHeaders.Length - 1) ? 0 : 20), 2);
            Assert.Equal(activeBoxModel.Width, shadowBoxWidth);
            var styleHandle = await shadow.GetPropertyAsync("style");
            var width = await styleHandle.EvaluateFunctionAsync<string>("s=>s.width");
            Assert.Equal($"{shadowBoxModel.Width.ToString("G29")}px", width);
            var transform = await styleHandle.EvaluateFunctionAsync<string>("x=>x.transform");
            Assert.Equal($"translateX({barOffsetLeft}px)", transform);
        }
    }
}
