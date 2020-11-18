using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blazui.Component.Test.CheckBoxTests
{
    [TestName("Checkbox 多选框", "复选框组")]
    public class Test2 : IDemoTester
    {
        public async Task TestAsync(DemoCard demoCard)
        {
            var checkAll = await demoCard.Body.QuerySelectorAsync("label.el-checkbox");
            var checkboxes = await demoCard.Body.QuerySelectorAllAsync("div.el-checkbox-group > label.el-checkbox");
            var checkedIndexes = new List<int>();
            checkedIndexes.Add(0);
            checkedIndexes.Add(2);
            await AssertCheckBoxesAsync(checkboxes, checkedIndexes.ToArray());
            var count = 20;
            var random = new Random();
            while (count-- > 0)
            {
                var index = random.Next(3);
                await checkboxes[index].ClickAsync();
                await Task.Delay(50);
                if (checkedIndexes.Contains(index))
                {
                    checkedIndexes.Remove(index);
                }
                else
                {
                    checkedIndexes.Add(index);
                }
                if (checkedIndexes.Count == 3)
                {
                    await AssertCheckBoxAsync(true, checkAll);
                }
                else if (checkedIndexes.Count == 0)
                {
                    await AssertCheckBoxAsync(false, checkAll);
                }
                await AssertCheckBoxesAsync(checkboxes, checkedIndexes.ToArray());
            }
            await checkAll.ClickAsync();
            await Task.Delay(50);
            checkedIndexes.Clear();
            checkedIndexes.Add(0);
            checkedIndexes.Add(1);
            checkedIndexes.Add(2);
            await AssertCheckBoxesAsync(checkboxes, checkedIndexes.ToArray());
            await checkAll.ClickAsync();
            await Task.Delay(50);
            checkedIndexes.Clear();
            await AssertCheckBoxesAsync(checkboxes, checkedIndexes.ToArray());
        }

        private async Task AssertCheckBoxesAsync(ElementHandle[] checkboxes, params int[] checkedIndexes)
        {
            for (int i = 0; i < checkboxes.Length; i++)
            {
                var checkbox = checkboxes[i];
                await AssertCheckBoxAsync(checkedIndexes.Contains(i), checkbox);
            }
        }

        private static async Task AssertCheckBoxAsync(bool isChecked, ElementHandle checkbox)
        {
            var cls = await checkbox.EvaluateFunctionAsync<string>("x=>x.className");
            ElementHandle el;
            if (isChecked)
            {
                Assert.Equal("el-checkbox is-checked ", cls);
                el = await checkbox.QuerySelectorAsync("span.el-checkbox__input.is-checked > span.el-checkbox__inner");
                Assert.NotNull(el);
            }
            else
            {
                Assert.Equal("el-checkbox  ", cls);
                el = await checkbox.QuerySelectorAsync("span.el-checkbox__input > span.el-checkbox__inner");
                Assert.NotNull(el);
            }
            el = await checkbox.QuerySelectorAsync("input[type='checkbox'].el-checkbox__original");
            Assert.NotNull(el);
        }
    }
}
