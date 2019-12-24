using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blazui.Component.Test.FormTests
{
    [TestName("Form 表单", "基础用法")]
    public class Test1 : IDemoTester
    {
        public async Task TestAsync(DemoCard demoCard)
        {
            var form = await demoCard.Body.QuerySelectorAsync("form.el-form--leaf-left.el-form");
            Assert.NotNull(form);
            var formItems = await form.QuerySelectorAllAsync("div.el-form-item.is-required");
            Assert.Equal(8, formItems.Length);
            foreach (var formItem in formItems)
            {
                var index = Array.IndexOf(formItems, formItem);
                var label = await formItem.QuerySelectorAsync("label[for='name'].el-form-item__label");
                Assert.NotNull(label);
                var content = await formItem.QuerySelectorAsync("div.el-form-item__content");
                Assert.NotNull(content);
                var contentMarginLeft = await content.EvaluateFunctionAsync<string>("x=>x.style.marginLeft");
                Assert.Equal("100px", contentMarginLeft);
                var labelWidth = await label.EvaluateFunctionAsync<string>("x=>x.style.width");
                Assert.Equal("100px", labelWidth);
                var labelText = await label.EvaluateFunctionAsync<string>("x=>x.innerText");
                if (index == 0)
                {
                    Assert.Equal("活动名称", labelText?.Trim());
                    var input = content.QuerySelectorAsync("div.el-input > input[type='text'][name='Name'][placeholder='请输入内容'].el-input__inner");
                    Assert.NotNull(input);
                    continue;
                }

                if (index == 1)
                {
                    Assert.Equal("活动区域", labelText?.Trim());
                    var input = content.QuerySelectorAsync("div.el-select > div.el-input.el-input-suffix > input[type='text'][placeholder='请选择活动区域'].el-input__inner");
                    Assert.NotNull(input);
                    var icon = content.QuerySelectorAsync("div.el-select > div.el-input.el-input-suffix > span.el-input__suffix > span.el-input__suffix-inner > i.el-input__icon.el-select__caret.el-icon-arrow-up");
                    Assert.NotNull(icon);
                    continue;
                }

                if (index == 2)
                {
                    continue;
                }

                if (index == 3)
                {
                    continue;
                }

                if (index == 4)
                {
                    continue;
                }

                if (index == 5)
                {
                    continue;
                }

                if (index == 6)
                {
                    continue;
                }

                if (index == 7)
                {
                    continue;
                }

                throw new Exception(index.ToString());
            }
        }
    }
}
