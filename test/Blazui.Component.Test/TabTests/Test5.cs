using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blazui.Component.Test.TabTests
{
    [TestName("Tabs 标签页", "可编辑的标签页")]
    public class Test5 : IDemoTester
    {
        public async Task TestAsync(DemoCard card)
        {
            var header = await card.Body.QuerySelectorAsync("div.el-tabs.el-tabs--card.el-tabs--top > div.el-tabs__header.is-top");
            var body = await card.Body.QuerySelectorAsync("div.el-tabs.el-tabs--card.el-tabs--top > div.el-tabs__content");
            Assert.NotNull(header);
            Assert.NotNull(body);
            var tabHeaders = await header.QuerySelectorAllAsync("div.el-tabs__nav-wrap.is-top > div.el-tabs__nav-scroll > div.el-tabs__nav.is-top > div.el-tabs__item.is-top.is-closable");
            var tasks = tabHeaders.Select(async x => new
            {
                Title = (await x.EvaluateFunctionAsync<string>("m=>m.innerText")).Trim(),
                Header = x
            });
            var headers = (await Task.WhenAll(tasks)).ToList();
            Assert.Equal(4, headers.Count);
            Assert.Equal("选项卡1", headers[0].Title);
            Assert.Equal("卡2", headers[1].Title);
            Assert.Equal("卡3", headers[2].Title);
            Assert.Equal("Component", headers[3].Title);

            await AssertHeaderAsync(tabHeaders, 0);
            await AssertBodyAsync(body, @"<!--!-->
<!--!-->
        <!--!--><!--!-->内容1<!--!-->
            ");
            await AssertHoverAsync(tabHeaders, 0);
            foreach (var tabHeader in headers.Skip(1))
            {
                await tabHeader.Header.ClickAsync();
                await Task.Delay(100);
                var index = headers.IndexOf(tabHeader);
                await AssertHoverAsync(tabHeaders, index);
                await AssertHeaderAsync(tabHeaders, index);
                await AssertBodyAsync(body, tabHeader.Title);
            }
        }

        private async Task AssertHoverAsync(ElementHandle[] tabHeaders, int activeIndex)
        {
            var activeHeader = tabHeaders[activeIndex];
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
            var bodyText = await body.EvaluateFunctionAsync<string>("x=>x.innerHTML");
            if (text == "卡2")
            {
                Assert.Equal(@$"<!--!-->{Environment.NewLine}<!--!-->{Environment.NewLine}        <!--!--><!--!-->内容2<!--!-->{Environment.NewLine}            ", bodyText);
            }
            else if (text == "卡3")
            {
                Assert.Equal(@$"<!--!-->{Environment.NewLine}<!--!-->{Environment.NewLine}        <!--!--><!--!-->内容3<!--!-->{Environment.NewLine}            ", bodyText);
            }
            else if (text == "Component")
            {
                Assert.Equal(@$"<!--!-->{Environment.NewLine}<!--!-->{Environment.NewLine}        <!--!--><!--!--><!--!-->    <label role=""checkbox"" aria-checked=""true"" class=""el-checkbox  ""><!--!-->{Environment.NewLine}        <span aria-checked=""mixed"" class=""el-checkbox__input   ""><!--!-->{Environment.NewLine}            <span class=""el-checkbox__inner""></span>
            <input type=""checkbox"" aria-hidden=""true"" class=""el-checkbox__original "" value=""""><!--!-->
        </span><!--!-->
        <span class=""el-checkbox__label""><!--!-->
            a<!--!-->
        </span><!--!-->
    </label><!--!-->
<!--!-->
            ", bodyText);
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
