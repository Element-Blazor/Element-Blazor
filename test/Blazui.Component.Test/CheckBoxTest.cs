using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Blazui.Component.Test
{
    public class CheckBoxTest : SetupTest
    {
        public CheckBoxTest(ITestOutputHelper output) : base(output)
        {
        }

        private async Task TestCaseAsync(string name)
        {
            await TestCaseAsync("Checkbox 多选框", name);
        }

        [Fact]
        public async Task Test1Async()
        {
            await TestCaseAsync("硬编码");
        }

        [Fact]
        public async Task Test2Async()
        {
            await TestCaseAsync("复选框组");
        }

        [Fact]
        public async Task Test3Async()
        {
            await TestCaseAsync("按钮复选框组");
        }
    }
}
