using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Blazui.Component.Test
{
    public class FormTest : SetupTest
    {
        public FormTest(ITestOutputHelper output) : base(output)
        {
        }

        private async ValueTask TestCaseAsync(string name)
        {
            await TestCaseAsync("Form 表单", name);
        }

        [Fact]
        public async ValueTask Test1Async()
        {
            await TestCaseAsync("基础用法");
        }
    }
}
