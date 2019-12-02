using Blazui.ServerRender;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blazui.Component.Test
{
    public class SetupTest
    {
        private static bool initilized = false;
        public SetupTest()
        {
        }
        protected async Task InitilizeAsync()
        {
            if (initilized)
            {
                return;
            }
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            _ = Program.CreateHostBuilder(new string[0]).Build().RunAsync();
            await Task.Delay(1000);
            initilized = true;
        }
    }
}
