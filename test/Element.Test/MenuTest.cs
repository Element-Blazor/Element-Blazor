using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Element.Test
{
    public class MenuTest : SetupTest
    {
        public MenuTest(ITestOutputHelper output) : base(output)
        {
        }

        async Task StartTestAsync(Func<Task> action)
        {
            try
            {
                await InitilizeAsync();
                await Page.WaitForSelectorAsync(".sidebar > .el-menu > li");
                while (true)
                {
                    try
                    {
                        await action();
                        break;
                    }
                    catch (PuppeteerException pe) when (pe.Message == "Node is detached from document" || pe.Message == "Node is either not visible or not an HTMLElement")
                    {
                        await Task.Delay(50);
                    }
                }
            }
            finally
            {
                TestSemaphoreSlim.Release();
            }
        }


        [Fact]
        private async Task TestShowAsync()
        {
            await StartTestAsync(async () =>
            {
                var menus = await Page.QuerySelectorAllAsync(".sidebar > .el-menu > li");
                Assert.True(menus.Count() == 19);
                foreach (var menu in menus)
                {
                    var backgroundColor = await menu.EvaluateFunctionAsync<string>("x=>x.style.backgroundColor");
                    Assert.True(string.IsNullOrWhiteSpace(backgroundColor));
                }
            });
        }

        [Fact]
        public async Task TestHoverAsync()
        {
            await StartTestAsync(async () =>
             {
                 var menus = await Page.QuerySelectorAllAsync(".sidebar > .el-menu > li");
                 Assert.True(menus.Count() == 19);
                 foreach (var menu in menus)
                 {
                     await menu.HoverAsync();
                     await Task.Delay(50);
                     foreach (var otherMenu in menus)
                     {
                         if (menu == otherMenu)
                         {
                             continue;
                         }
                         var otherBackgroundColor = await otherMenu.EvaluateFunctionAsync<string>("x=>x.style.backgroundColor");
                         Assert.True(string.IsNullOrWhiteSpace(otherBackgroundColor));
                     }
                     var backgroundColor = await menu.EvaluateFunctionAsync<string>("x=>x.style.backgroundColor");
                     Assert.Equal("rgb(236, 245, 255)", backgroundColor);
                 }
             });
        }
        [Fact]
        public async Task TestClickAsync()
        {
            var count = 5;
            while (count-- >= 0)
            {
                try
                {
                    await StartTestAsync(async () =>
                    {
                        var menus = await Page.QuerySelectorAllAsync(".sidebar > .el-menu > li");
                        Assert.True(menus.Count() == 18);
                        foreach (var menu in menus)
                        {
                            await menu.ClickAsync();
                            await Task.Delay(200);
                            foreach (var otherMenu in menus)
                            {
                                if (menu == otherMenu)
                                {
                                    continue;
                                }
                                var otherBackgroundColor = await otherMenu.EvaluateFunctionAsync<string>("x=>x.style.backgroundColor");
                                Assert.True(string.IsNullOrWhiteSpace(otherBackgroundColor));
                            }
                            var backgroundColor = await menu.EvaluateFunctionAsync<string>("x=>x.style.backgroundColor");
                            Assert.Equal("rgb(236, 245, 255)", backgroundColor);
                        }
                    });
                    break;
                }
                catch
                {
                    await Page.ReloadAsync();
                }
            }
        }
    }
}
