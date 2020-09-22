using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Element.Test.CheckBoxTests
{
    [TestName("Checkbox 多选框", "硬编码")]
    public class Test1 : IDemoTester
    {
        public async Task TestAsync(DemoCard demoCard)
        {
            var checkboxes = await demoCard.Body.QuerySelectorAllAsync("label.el-checkbox");
            var checkedIndexes = new List<int>();
            var disabledIndexes = new List<int>();
            disabledIndexes.Add(1);
            await AssertCheckBoxesAsync(checkboxes, checkedIndexes.ToArray(), disabledIndexes.ToArray());
            await checkboxes[0].ClickAsync();
            await Task.Delay(50);
            checkedIndexes.Add(0);
            await AssertCheckBoxesAsync(checkboxes, checkedIndexes.ToArray(), disabledIndexes.ToArray());
            await checkboxes[1].ClickAsync();
            await Task.Delay(50);
            await AssertCheckBoxesAsync(checkboxes, checkedIndexes.ToArray(), disabledIndexes.ToArray());
        }

        private async Task AssertCheckBoxesAsync(ElementHandle[] checkboxes, int[] checkedIndexes, int[] disabledIndexes)
        {
            for (int i = 0; i < checkboxes.Length; i++)
            {
                var checkbox = checkboxes[i];
                var cls = await checkbox.EvaluateFunctionAsync<string>("x=>x.className");
                ElementHandle el;
                if (checkedIndexes.Contains(i))
                {
                    if (disabledIndexes.Contains(i))
                    {
                        Assert.Equal("el-checkbox is-checked is-disabled", cls);
                        el = await checkbox.QuerySelectorAsync("span.el-checkbox__input.is-disabled > span.el-checkbox__inner");
                        Assert.NotNull(el);
                        el = await checkbox.QuerySelectorAsync("input[type='checkbox'].el-checkbox__original.is-disabled");
                        Assert.NotNull(el);
                    }
                    else
                    {
                        Assert.Equal("el-checkbox is-checked ", cls);
                        el = await checkbox.QuerySelectorAsync("span.el-checkbox__input.is-checked > span.el-checkbox__inner");
                        Assert.NotNull(el);
                        el = await checkbox.QuerySelectorAsync("input[type='checkbox'].el-checkbox__original");
                        Assert.NotNull(el);
                    }
                }
                else
                {
                    if (disabledIndexes.Contains(i))
                    {
                        Assert.Equal("el-checkbox  is-disabled", cls);
                        el = await checkbox.QuerySelectorAsync("span.el-checkbox__input.is-disabled > span.el-checkbox__inner");
                        Assert.NotNull(el);
                        el = await checkbox.QuerySelectorAsync("input[type='checkbox'].el-checkbox__original.is-disabled");
                        Assert.NotNull(el);
                    }
                    else
                    {
                        Assert.Equal("el-checkbox  ", cls);
                        el = await checkbox.QuerySelectorAsync("span.el-checkbox__input > span.el-checkbox__inner");
                        Assert.NotNull(el);
                        el = await checkbox.QuerySelectorAsync("input[type='checkbox'].el-checkbox__original");
                        Assert.NotNull(el);
                    }
                }
                el = await checkbox.QuerySelectorAsync("span.el-checkbox__label");
                Assert.NotNull(el);
            }
        }
    }
}
