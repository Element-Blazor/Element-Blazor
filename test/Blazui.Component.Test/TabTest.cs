using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Blazui.ServerRender;
using PuppeteerSharp;
using Xunit;
using Xunit.Abstractions;

namespace Blazui.Component.Test
{
    public class TabTest : SetupTest
    {
        public TabTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task TestTab1Async()
        {
            await TestCaseAsync("基础的、简洁的标签页");
        }
        [Fact]
        public async Task TestTab2Async()
        {
            await TestCaseAsync("选项卡样式的标签页");
        }
        [Fact]
        public async Task TestTab3Async()
        {
            await TestCaseAsync("卡片化的标签页");
        }

        private async Task TestCaseAsync(string name)
        {
            await TestCaseAsync("Tabs 标签页", name); 
        }

        [Fact]
        public async Task TestTab4Async()
        {
            await TestCaseAsync("在左边的标签页");
        }
        [Fact]
        public async Task TestTab5Async()
        {
            await TestCaseAsync("调用事件API实现可编辑的标签页");
        }
        [Fact]
        public async Task TestTab6Async()
        {
            await TestCaseAsync("双向绑定实现可编辑的标签页");
        }
    }
}
