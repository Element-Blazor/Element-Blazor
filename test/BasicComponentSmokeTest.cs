using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Element.Test
{
    public class BasicComponentSmokeTest : SetupTest
    {
        public BasicComponentSmokeTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("button", ".el-button")]
        [InlineData("border", ".border-demo__item")]
        [InlineData("color", ".color-demo__main")]
        [InlineData("configuration", ".el-space .el-button")]
        [InlineData("container", ".el-container .el-header")]
        [InlineData("icon", ".el-icon.el-icon-edit")]
        [InlineData("layout", ".el-row .el-col")]
        [InlineData("link", ".el-link")]
        [InlineData("text", ".el-text")]
        [InlineData("scrollbar", ".el-scrollbar")]
        [InlineData("space", ".el-space")]
        [InlineData("splitter", ".el-splitter .el-splitter-panel")]
        [InlineData("typography", ".typography-demo")]
        public async Task BasicRouteRendersAsync(string route, string selector)
        {
            try
            {
                await InitilizeAsync();
                await Page.GoToAsync($"https://localhost:5001/{route}");
                await Page.WaitForSelectorAsync(selector);
            }
            finally
            {
                TestSemaphoreSlim.Release();
            }
        }
    }
}
