using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blazui.Component.Test.RadioTests
{
    [TestName("Radio 单选框", "Change事件实现不允许切换状态")]
    public class Test3 : IDemoTester
    {
        public async Task TestAsync(DemoCard demoCard)
        {
            var ps = await demoCard.Body.QuerySelectorAllAsync("p");
            await TestRadioList(ps[0], 0);
        }

        private async Task TestRadioList(ElementHandle p, int defaultIndex)
        {
            var radios = await p.QuerySelectorAllAsync("label.el-radio");
            await AssertRadiosAsync(radios, defaultIndex, true);
            var count = 10;
            var random = new Random();
            var prevIndex = 0;
            while (count-- > 0)
            {
                var index = random.Next(3);
                await radios[index].ClickAsync();
                await Task.Delay(50);
                if (index == 0 || index == 1)
                {
                    index = prevIndex;
                }
                else
                {
                    prevIndex = index;
                }
                await AssertRadiosAsync(radios, index, false);
            }
        }

        async Task AssertRadiosAsync(ElementHandle[] radios, int checkedIndex, bool isInit)
        {
            for (int i = 0; i < radios.Length; i++)
            {
                var radio = radios[i];
                var cls = await radio.EvaluateFunctionAsync<string>("x=>x.className");
                ElementHandle el = null;
                if (i == checkedIndex)
                {
                    if (i == 0 || i == 1)
                    {
                        if (isInit)
                        {
                            Assert.Equal("el-radio is-checked   el-radio-button--default", cls);
                            el = await radio.QuerySelectorAsync("span.el-radio__input.is-checked > span.el-radio__inner");
                        }
                        else
                        {
                            Assert.Equal("el-radio  el-radio-button--default", cls);
                            el = await radio.QuerySelectorAsync("span.el-radio__input > span.el-radio__inner");
                        }
                    }
                    else
                    {
                        Assert.Equal("el-radio is-checked   el-radio-button--default", cls);
                        el = await radio.QuerySelectorAsync("span.el-radio__input.is-checked > span.el-radio__inner");
                    }
                }
                else
                {
                    Assert.Equal("el-radio    el-radio-button--default", cls);
                    el = await radio.QuerySelectorAsync("span.el-radio__input > span.el-radio__inner");
                }
                Assert.NotNull(el);
                el = await radio.QuerySelectorAsync("span.el-radio__label");
                Assert.NotNull(el);
            }
        }
    }
}
