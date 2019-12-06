using Blazui.ServerRender;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Blazui.Component.Test
{
    public class SetupTest
    {
        System.Threading.SemaphoreSlim SemaphoreSlim = new System.Threading.SemaphoreSlim(1, 1);
        private bool initilized = false;
        public SetupTest(ITestOutputHelper output)
        {
            Output = output;
        }

        public ITestOutputHelper Output { get; }
        public Browser Browser { get; private set; }

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
                _ = Task.Factory.StartNew(() =>
                  {
                      Program.Main(new string[0]);
                  });
                Output.WriteLine("下载浏览器");
                var fetcher = new BrowserFetcher();
                if (!File.Exists(fetcher.DownloadsFolder))
                {
                    await fetcher.DownloadAsync(BrowserFetcher.DefaultRevision);
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
                Output.WriteLine("初始化完成");
                initilized = true;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        protected async Task NavigateToMenuAsync(Page page, string menuText)
        {
            await page.WaitForSelectorAsync(".sidebar > .el-menu > li");
            while (true)
            {
                var menus = await page.QuerySelectorAllAsync(".sidebar > .el-menu > li");
                foreach (var menu in menus)
                {
                    var text = await menu.EvaluateFunctionAsync<string>("(m)=>m.innerText");
                    if (text?.Trim() == menuText)
                    {
                        await menu.ClickAsync();
                        return;
                    }
                }
            }
        }

        protected async Task<List<DemoCard>> WaitForDemoCardsAsync(Page page)
        {
            await page.WaitForSelectorAsync(".main > .el-card");
            await Task.Delay(1000);
            var demoCards = new List<DemoCard>();
            var cards = await page.QuerySelectorAllAsync(".main > .el-card");
            foreach (var card in cards)
            {
                var header = await card.QuerySelectorAsync(".el-card__header");
                var text = await header.EvaluateFunctionAsync<string>("(m)=>m.innerText");
                demoCards.Add(new DemoCard()
                {
                    Title = text,
                    Body = await card.QuerySelectorAsync("el-card__body")
                });
            }
            return demoCards;
        }
    }
}
