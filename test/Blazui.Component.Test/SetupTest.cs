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
                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
                Output.WriteLine("初始化完成");
                initilized = true;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }
    }
}
