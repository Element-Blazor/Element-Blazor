using System;
using System.Collections.Generic;
using System.Linq;
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
            var form = await demoCard.Body.QuerySelectorAsync("form.el-form--label-left.el-form");
            Assert.NotNull(form);
            var formItems = await form.QuerySelectorAllAsync("div.el-form-item");
            Assert.Equal(9, formItems.Length);
            foreach (var formItem in formItems)
            {
                var index = Array.IndexOf(formItems, formItem);
                var label = await formItem.QuerySelectorAsync("label[for='name'].el-form-item__label");
                var content = await formItem.QuerySelectorAsync("div.el-form-item__content");
                Assert.NotNull(content);
                var contentMarginLeft = await content.EvaluateFunctionAsync<string>("x=>x.style.marginLeft");
                var classList = await formItem.EvaluateFunctionAsync<string>("x=>x.classList.toString()");
                var clsList = classList.Split(' ').Select(x => x.Trim()).ToArray() ?? new string[0];
                string labelWidth = string.Empty;
                string labelText = string.Empty;
                if (index < 8)
                {
                    Assert.NotNull(label);
                    labelWidth = await label.EvaluateFunctionAsync<string>("x=>x.style.width");
                    labelText = await label.EvaluateFunctionAsync<string>("x=>x.innerText");
                    Assert.Equal("100px", contentMarginLeft);
                    Assert.Equal("100px", labelWidth);
                    if (index == 2)
                    {
                        Assert.DoesNotContain("is-required", clsList);
                    }
                    else
                    {
                        Assert.Contains("is-required", clsList);
                    }
                }
                else
                {
                    Assert.Null(label);
                    Assert.True(string.IsNullOrWhiteSpace(contentMarginLeft));
                    Assert.True(string.IsNullOrWhiteSpace(labelWidth));
                    Assert.DoesNotContain("is-required", clsList);
                }
                if (index == 0)
                {
                    Assert.Equal("活动名称", labelText?.Trim());
                    var input = await content.QuerySelectorAsync("div.el-input > input[type='text'][name='Name'][placeholder='请输入内容'].el-input__inner");
                    Assert.NotNull(input);
                    var inputValue = await input.EvaluateFunctionAsync<string>("x=>x.value");
                    Assert.Equal(string.Empty, inputValue);
                    continue;
                }

                if (index == 1)
                {
                    Assert.Equal("活动区域", labelText?.Trim());
                    var input = await content.QuerySelectorAsync("div.el-select > div.el-input.el-input--suffix > input[type='text'][placeholder='请选择活动区域'].el-input__inner");
                    Assert.NotNull(input);
                    var inputValue = await input.EvaluateFunctionAsync<string>("x=>x.value");
                    Assert.Equal("北京", inputValue);
                    var icon = await content.QuerySelectorAsync("div.el-select > div.el-input.el-input--suffix > span.el-input__suffix > span.el-input__suffix-inner > i.el-input__icon.el-select__caret.el-icon-arrow-up");
                    Assert.NotNull(icon);
                    continue;
                }

                if (index == 2)
                {
                    Assert.Equal("活动区域2", labelText?.Trim());
                    var input = await content.QuerySelectorAsync("div.el-select > div.el-input.el-input--suffix > input[type='text'][placeholder='请选择活动区域'].el-input__inner");
                    Assert.NotNull(input);
                    var inputValue = await input.EvaluateFunctionAsync<string>("x=>x.value");
                    Assert.Equal(string.Empty, inputValue);
                    var icon = await content.QuerySelectorAsync("div.el-select > div.el-input.el-input--suffix > span.el-input__suffix > span.el-input__suffix-inner > i.el-input__icon.el-select__caret.el-icon-arrow-up");
                    Assert.NotNull(icon);
                    continue;
                }

                if (index == 3)
                {
                    Assert.Equal("活动时间", labelText?.Trim());
                    var input = await content.QuerySelectorAsync("div.el-input.el-date-editor.el-input--prefix.el-input--suffix.el-date-editor--date > input[type='text'][placeholder='请选择日期'][name='Time'].el-input__inner");
                    Assert.NotNull(input);
                    var inputValue = await input.EvaluateFunctionAsync<string>("x=>x.value");
                    Assert.Equal(string.Empty, inputValue);
                    var icon = await content.QuerySelectorAsync("div.el-input.el-date-editor.el-input--prefix.el-input--suffix.el-date-editor--date > span.el-input__prefix > i.el-input__icon.el-icon-date");
                    Assert.NotNull(icon);
                    continue;
                }

                if (index == 4)
                {
                    Assert.Equal("即时配送", labelText?.Trim());
                    var input = await content.QuerySelectorAsync("div.el-switch > input[type='checkbox'].el-switch__input");
                    Assert.NotNull(input);
                    var icon = await content.QuerySelectorAsync("div.el-switch > span.el-switch__core");
                    Assert.NotNull(icon);
                    var borderColor = await icon.EvaluateFunctionAsync<string>("x=>x.style.borderColor");
                    Assert.Equal("rgb(19, 206, 102)", borderColor);
                    var backgroundColor = await icon.EvaluateFunctionAsync<string>("x=>x.style.backgroundColor");
                    Assert.Equal("rgb(192, 204, 218)", backgroundColor);
                    continue;
                }

                if (index == 5)
                {
                    Assert.Equal("活动性质", labelText?.Trim());
                    var checkboxes = await content.QuerySelectorAllAsync("div.el-checkbox-group > label.el-checkbox");
                    Assert.Equal(2, checkboxes.Length);
                    continue;
                }

                if (index == 6)
                {
                    Assert.Equal("特殊资源", labelText?.Trim());
                    var radios = await content.QuerySelectorAllAsync("label.el-radio.el-radio-button--default");
                    Assert.Equal(2, radios.Length);
                    continue;
                }

                if (index == 7)
                {
                    Assert.Equal("活动形式", labelText?.Trim());
                    var input = await content.QuerySelectorAsync("div.el-input > input[type='textarea'][name='Description'][placeholder='请输入内容'].el-input__inner");
                    Assert.NotNull(input);
                    var inputValue = await input.EvaluateFunctionAsync<string>("x=>x.value");
                    Assert.Equal(string.Empty, inputValue);
                    continue;
                }
                if (index == 8)
                {
                    continue;
                }

                throw new Exception(index.ToString());
            }
        }
    }
}
