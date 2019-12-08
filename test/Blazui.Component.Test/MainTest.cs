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

        [Fact]
        public async Task TestTab1Async()
        {
            await InitilizeAsync();
            Page page = await Browser.NewPageAsync();
            await page.GoToAsync("https://localhost:5001");
            await NavigateToMenuAsync(page, "Tabs 标签页");
            var demoCards = await WaitForDemoCardsAsync(page);
            await TestAsync("Tabs 标签页", demoCards.FirstOrDefault(x => x.Title == "基础的、简洁的标签页"));
        }
        [Fact]
        public async Task TestTab2Async()
        {
            await InitilizeAsync();
            Page page = await Browser.NewPageAsync();
            await page.GoToAsync("https://localhost:5001");
            await NavigateToMenuAsync(page, "Tabs 标签页");
            var demoCards = await WaitForDemoCardsAsync(page);
            await TestAsync("Tabs 标签页", demoCards.FirstOrDefault(x => x.Title == "选项卡样式的标签页"));
        }
        [Fact]
        public async Task TestTab3Async()
        {
            await InitilizeAsync();
            Page page = await Browser.NewPageAsync();
            await page.GoToAsync("https://localhost:5001");
            await NavigateToMenuAsync(page, "Tabs 标签页");
            var demoCards = await WaitForDemoCardsAsync(page);
            await TestAsync("Tabs 标签页", demoCards.FirstOrDefault(x => x.Title == "卡片化的标签页"));
        }
        [Fact]
        public async Task TestTab4Async()
        {
            await InitilizeAsync();
            Page page = await Browser.NewPageAsync();
            await page.GoToAsync("https://localhost:5001");
            await NavigateToMenuAsync(page, "Tabs 标签页");
            var demoCards = await WaitForDemoCardsAsync(page);
            await TestAsync("Tabs 标签页", demoCards.FirstOrDefault(x => x.Title == "在左边的标签页"));
        }
        [Fact]
        public async Task TestTab5Async()
        {
            await InitilizeAsync();
            Page page = await Browser.NewPageAsync();
            await page.GoToAsync("https://localhost:5001");
            await NavigateToMenuAsync(page, "Tabs 标签页");
            var demoCards = await WaitForDemoCardsAsync(page);
            await TestAsync("Tabs 标签页", demoCards.FirstOrDefault(x => x.Title == "调用事件API实现可编辑的标签页"));
        }
        [Fact]
        public async Task TestTab6Async()
        {
            await InitilizeAsync();
            Page page = await Browser.NewPageAsync();
            await page.GoToAsync("https://localhost:5001");
            await NavigateToMenuAsync(page, "Tabs 标签页");
            var demoCards = await WaitForDemoCardsAsync(page);
            await TestAsync("Tabs 标签页", demoCards.FirstOrDefault(x => x.Title == "双向绑定实现可编辑的标签页"));
        }
    }
}
