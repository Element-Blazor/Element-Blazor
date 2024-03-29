﻿using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Element.Test.RadioTests
{
    [TestName("Radio 单选框", "基础用法")]
    public class Test1 : IDemoTester
    {
        public async Task TestAsync(DemoCard demoCard)
        {
            var ps = await demoCard.Body.QuerySelectorAllAsync("p");
            await TestRadioList(ps[0], 0);
            await TestRadioList(ps[1], 2);
        }

        private async Task TestRadioList(IElementHandle p, int defaultIndex)
        {
            var radios = await p.QuerySelectorAllAsync("label.el-radio");
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

        async Task AssertRadiosAsync(IElementHandle[] radios, int checkedIndex)
        {
            for (int i = 0; i < radios.Length; i++)
            {
                var radio = radios[i];
                var cls = await radio.EvaluateFunctionAsync<string>("x=>x.className");
                IElementHandle el = null;
                if (i == checkedIndex)
                {
                    Assert.Equal("el-radio is-checked   el-radio-button--default", cls);
                    el = await radio.QuerySelectorAsync("span.el-radio__input.is-checked > span.el-radio__inner");
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
