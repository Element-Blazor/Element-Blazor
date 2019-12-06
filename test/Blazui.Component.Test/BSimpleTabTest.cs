using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Blazui.ServerRender;
using PuppeteerSharp;
using Xunit;
using Xunit.Abstractions;

namespace Blazui.Component.Test
{
    public class BSimpleTabTest : SetupTest
    {
        public BSimpleTabTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task Test1Async()
        {
            await InitilizeAsync();
            Page page = await Browser.NewPageAsync();
            await page.GoToAsync("https://localhost:5001");
            await NavigateToMenuAsync(page, "Tabs ±Í«©“≥");
            var demoCards = await WaitForDemoCardsAsync(page);
            await Task.Delay(TimeSpan.FromDays(1));
        }
    }
}
