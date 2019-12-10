using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blazui.Component.Test.RadioTests
{
    [TestName("Radio 单选框", "单选按钮组")]
    public class Test5 : IDemoTester
    {
        public async Task TestAsync(DemoCard demoCard)
        {
            await TestRadioList(await demoCard.Body.QuerySelectorAsync("div.el-radio-group"), 0);
        }

        private async Task TestRadioList(ElementHandle p, int defaultIndex)
        {
            var radios = await p.QuerySelectorAllAsync("label.el-radio-button");
            await AssertRadiosAsync(radios, defaultIndex);
            var count = 10;
            var random = new Random();
            while (count-- > 0)
            {
                var index = random.Next(3);
                await radios[index].ClickAsync();
                await Task.Delay(50);
                await AssertRadiosAsync(radios, index);
            }
        }

        async Task AssertRadiosAsync(ElementHandle[] radios, int checkedIndex)
        {
            for (int i = 0; i < radios.Length; i++)
            {
                var radio = radios[i];
                var cls = await radio.EvaluateFunctionAsync<string>("x=>x.className");
                if (i == checkedIndex)
                {
                    Assert.Equal("el-radio-button is-active  el-radio-button--default", cls);
                }
                else
                {
                    Assert.Equal("el-radio-button   el-radio-button--default", cls);
                }
                var el = await radio.QuerySelectorAsync("input[type='radio'].el-radio-button__orig-radio");
                Assert.NotNull(el);
                el = await radio.QuerySelectorAsync("span.el-radio-button__inner");
                Assert.NotNull(el);
            }
        }
    }
}
