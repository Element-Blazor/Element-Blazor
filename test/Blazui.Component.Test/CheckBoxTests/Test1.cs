using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blazui.Component.Test.CheckBoxTests
{
    [TestName("Checkbox 多选框", "动态渲染")]
    public class Test1 : IDemoTester
    {
        public async Task TestAsync(DemoCard demoCard)
        {
            var checkboxes = await demoCard.Body.QuerySelectorAllAsync("label.el-checkbox");
            var checkedIndexes = new List<int>();
            checkedIndexes.Add(0);
            checkedIndexes.Add(2);
            await AssertCheckBoxesAsync(checkboxes, checkedIndexes.ToArray());
            var count = 10;
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
                await AssertCheckBoxesAsync(checkboxes, checkedIndexes.ToArray());
            }
        }

        private async Task AssertCheckBoxesAsync(ElementHandle[] checkboxes, params int[] checkedIndexes)
        {
            for (int i = 0; i < checkboxes.Length; i++)
            {
                var checkbox = checkboxes[i];
                var cls = await checkbox.EvaluateFunctionAsync("x=>x.className");
                ElementHandle el;
                if (checkedIndexes.Contains(i))
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
}
