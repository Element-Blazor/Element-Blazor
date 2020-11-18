using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Blazui.Component.Test
{
    public class RadioTest : SetupTest
    {
        public RadioTest(ITestOutputHelper output) : base(output)
        {
        }

        private async Task TestCaseAsync(string name)
        {
            await TestCaseAsync("Radio 单选框", name);
        }

        [Fact]
        public async Task Test1Async()
        {
            await TestCaseAsync("基础用法");
        }
        [Fact]
        public async Task Test3Async()
        {
            await TestCaseAsync("Change事件实现不允许切换状态");
        }
        [Fact]
        public async Task Test4Async()
        {
            await TestCaseAsync("单选框组");
        }
        [Fact]
        public async Task Test5Async()
        {
            await TestCaseAsync("单选按钮组");
        }
    }
}
