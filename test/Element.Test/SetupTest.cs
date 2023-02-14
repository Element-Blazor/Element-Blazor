using Element.Admin.Sample.ServerRender;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Element.Test
{
    public class SetupTest : TestBase, IDisposable
    {
        static System.Threading.SemaphoreSlim InitilizeSemaphoreSlim = new System.Threading.SemaphoreSlim(1, 1);
        protected static SemaphoreSlim TestSemaphoreSlim = new SemaphoreSlim(1, 1);
        private bool initilized = false;
        private IHostBuilder hostBuilder;
        private CancellationTokenSource source;

        public SetupTest(ITestOutputHelper output)
        {
            Output = output;
        }

        public ITestOutputHelper Output { get; }
        public IBrowser Browser { get; private set; }

        protected async ValueTask TestCaseAsync(string tabName, string caseName)
        {
            try
            {
                await InitilizeAsync();
                await NavigateToMenuAsync(tabName);
                var demoCards = await WaitForDemoCardsAsync();
                await TestAsync(tabName, demoCards.FirstOrDefault(x => x.Title == caseName));
            }
            finally
            {
                TestSemaphoreSlim.Release();
            }
        }
        protected async ValueTask InitilizeAsync()
        {
            if (initilized)
            {
                await TestSemaphoreSlim.WaitAsync();
                await RunBrowserAsync();
                Page = await Browser.NewPageAsync();
                await Page.GoToAsync("https://localhost:5001");
                return;
            }
            await InitilizeSemaphoreSlim.WaitAsync();
            try
            {
                if (initilized)
                {
                    await TestSemaphoreSlim.WaitAsync();
                    await RunBrowserAsync();
                    Page = await Browser.NewPageAsync();
                    await Page.GoToAsync("https://localhost:5001");
                    return;
                }
                Output.WriteLine("启动服务器");
                hostBuilder = Program.CreateHostBuilder(new string[0]);
                source = new System.Threading.CancellationTokenSource();
                var host = hostBuilder.Build();
                _ = host.RunAsync(source.Token);
                demoTesterTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => !x.IsDynamic)
                    .SelectMany(x => x.ExportedTypes)
                    .Where(x => x.GetInterface(nameof(IDemoTester)) != null)
                    .Select(x =>
                    {
                        var testNameAttribute = x.GetCustomAttributes(false).OfType<TestNameAttribute>().FirstOrDefault();
                        return new
                        {
                            Menu = testNameAttribute.MenuName,
                            testNameAttribute.Name,
                            Type = x
                        };
                    })
                    .GroupBy(x => x.Menu)
                    .ToDictionary(x => x.Key, x => x.ToDictionary(y => y.Name, y => y.Type));
                Output.WriteLine("下载浏览器");
                var fetcher = new BrowserFetcher();
                if (!File.Exists(fetcher.DownloadsFolder))
                {
                    await fetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                }
                else
                {
                    await Task.Delay(1000);
                }
                await TestSemaphoreSlim.WaitAsync();
                await RunBrowserAsync();
                Page = await Browser.NewPageAsync();
                await Page.GoToAsync("https://localhost:5001");
                Output.WriteLine("初始化完成");
                initilized = true;
            }
            finally
            {
                InitilizeSemaphoreSlim.Release();
            }
        }

        private async ValueTask RunBrowserAsync()
        {
            Browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
                DefaultViewport = new ViewPortOptions()
                {
                    DeviceScaleFactor = 1,
                    Height = 800,
                    Width = 1024
                }
            });
        }

        protected async ValueTask NavigateToMenuAsync(string menuText)
        {
            await Page.WaitForSelectorAsync(".sidebar > .el-menu > li");
            while (true)
            {
                var menus = await Page.QuerySelectorAllAsync(".sidebar > .el-menu > li");
                foreach (var menu in menus)
                {
                    var text = await menu.EvaluateFunctionAsync<string>("(m)=>m.innerText");
                    if (text?.Trim() == menuText)
                    {
                        try
                        {
                            await menu.ClickAsync();
                            return;
                        }
                        catch (PuppeteerException pe) when (pe.Message == "Node is detached from document" || pe.Message == "Node is either not visible or not an HTMLElement")
                        {
                            await Task.Delay(50);
                        }
                    }
                }
            }
        }

        protected async ValueTask TestAsync(string menuName, DemoCard demoCard)
        {
            demoTesterTypes.TryGetValue(menuName, out var menuDemos);
            Assert.NotNull(menuDemos);
            menuDemos.TryGetValue(demoCard.Title, out var testType);
            Assert.True(testType != null, $"Demo \"{demoCard.Title}\" 对应的单元测试未找到");
            var tester = (IDemoTester)Activator.CreateInstance(testType);
            await tester.TestAsync(demoCard);
        }

        public void Dispose()
        {
            Browser.CloseAsync().Wait();
        }
    }
}
