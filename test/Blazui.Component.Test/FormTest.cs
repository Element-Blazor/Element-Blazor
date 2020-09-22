using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Element.Test
{
    public class FormTest : SetupTest
    {
        public FormTest(ITestOutputHelper output) : base(output)
        {
        }

        private async Task TestCaseAsync(string name)
        {
            await TestCaseAsync("Form 表单", name);
        }

        [Fact]
        public async Task Test1Async()
        {
            await TestCaseAsync("基础用法");
        }

        [Fact]
        public async Task Test2Async()
        {
            await TestCaseAsync("表单双向绑定");
        }
    }
}
