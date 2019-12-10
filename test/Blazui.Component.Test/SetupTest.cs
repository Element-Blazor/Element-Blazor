using Blazui.ServerRender;
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

namespace Blazui.Component.Test
{
    public class SetupTest : TestBase, IDisposable
    {
        System.Threading.SemaphoreSlim SemaphoreSlim = new System.Threading.SemaphoreSlim(1, 1);
        private bool initilized = false;
        private IHostBuilder host;
        private CancellationTokenSource source;

        public SetupTest(ITestOutputHelper output)
        {
            Output = output;
        }

        public ITestOutputHelper Output { get; }
        public Browser Browser { get; private set; }

        protected async Task TestCaseAsync(string tabName, string caseName)
        {
            await InitilizeAsync();
            await NavigateToMenuAsync(tabName);
            var demoCards = await WaitForDemoCardsAsync();
            await TestAsync(tabName, demoCards.FirstOrDefault(x => x.Title == caseName));
        }
        protected async Task InitilizeAsync()
        {
            if (initilized)
            {
                return;
            }
            await SemaphoreSlim.WaitAsync();
            try
            {
                if (initilized)
                {
                    return;
                }
                Output.WriteLine("启动服务器");
                host = Program.CreateHostBuilder(new string[0]);
                source = new System.Threading.CancellationTokenSource();
                _ = host.RunConsoleAsync(source.Token);
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
                    await fetcher.DownloadAsync(BrowserFetcher.DefaultRevision);
                }
                else
                {
                    await Task.Delay(1000);
                }
                Browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = !Debugger.IsAttached,
                    DefaultViewport = new ViewPortOptions()
                    {
                        DeviceScaleFactor = 1,
                        Height = 800,
                        Width = 1024
                    }
                });

                Page = (await Browser.PagesAsync())[0];
                await Page.GoToAsync("https://localhost:5001");
                Output.WriteLine("初始化完成");
                initilized = true;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        protected async Task NavigateToMenuAsync(string menuText)
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

        protected async Task NavigateToAllMenuAsync()
        {
            await Page.WaitForSelectorAsync(".sidebar > .el-menu > li");
            while (true)
            {
                try
                {
                    var menus = await Page.QuerySelectorAllAsync(".sidebar > .el-menu > li");
                    Assert.True(menus.Count() == 17);
                    foreach (var menu in menus)
                    {
                        var backgroundColor = await menu.EvaluateFunctionAsync<string>("x=>x.style.backgroundColor");
                        Assert.True(string.IsNullOrWhiteSpace(backgroundColor));
                    }
                    //foreach (var menu in menus)
                    //{
                    //    await menu.HoverAsync();
                    //    await menu.ClickAsync();
                    //}
                    break;
                }
                catch (PuppeteerException pe) when (pe.Message == "Node is detached from document" || pe.Message == "Node is either not visible or not an HTMLElement")
                {
                    await Task.Delay(50);
                }
            }
        }

        protected async Task TestAsync(string menuName, DemoCard demoCard)
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
            _ = Browser.CloseAsync();
            source.Cancel();
        }
    }
}
