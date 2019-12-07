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
    public class MainTest : SetupTest
    {
        public MainTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("基础的、简洁的标签页")]
        [InlineData("选项卡样式的标签页")]
        [InlineData("卡片化的标签页")]
        [InlineData("在左边的标签页")]
        [InlineData("可编辑的标签页")]
        public async Task TestTabAsync(string title)
        {
            await InitilizeAsync();
            Page page = await Browser.NewPageAsync();
            await page.GoToAsync("https://localhost:5001");
            await NavigateToMenuAsync(page, "Tabs 标签页");
            var demoCards = await WaitForDemoCardsAsync(page);
            await TestAsync("Tabs 标签页", demoCards.FirstOrDefault(x => x.Title == title));
        }
    }
}
