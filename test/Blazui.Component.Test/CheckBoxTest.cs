using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Blazui.Component.Test
{
    public class CheckBoxTest : SetupTest
    {
        public CheckBoxTest(ITestOutputHelper output) : base(output)
        {
        }

        private async Task TestCaseAsync(string name)
        {
            await TestCaseAsync("Checkbox 多选框", name);
        }

        [Fact]
        public async Task Test1Async()
        {
            await TestCaseAsync("动态渲染");
        }
    }
}
