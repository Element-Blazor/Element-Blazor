using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Component.Test
{
    public class TestBase
    {
        protected IDictionary<string, Dictionary<string, Type>> demoTesterTypes;
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
                    Body = await card.QuerySelectorAsync(".el-card__body > .el-tabs > .el-tabs__content"),
                    Page = page
                });
            }
            return demoCards;
        }
    }
}
