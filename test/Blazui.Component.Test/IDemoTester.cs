using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Component.Test
{
    public interface IDemoTester
    {
        Task TestAsync(DemoCard demoCard);
    }
}
