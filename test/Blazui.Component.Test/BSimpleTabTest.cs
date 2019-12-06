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
            Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = !Debugger.IsAttached
            });

            // Create a new page and go to Bing Maps
            Page page = await browser.NewPageAsync();
            await page.GoToAsync("https://localhost:5001");
            await Task.Delay(TimeSpan.FromDays(1));
        }
    }
}
