using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Element.Test
{
    public interface IDemoTester
    {
        Task TestAsync(DemoCard demoCard);
    }
}
