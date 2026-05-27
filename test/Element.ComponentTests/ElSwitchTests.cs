using Bunit;
using Element;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Element.ComponentTests
{
    public class ElSwitchTests : BunitContext
    {
        public ElSwitchTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void LoadingSwitchDoesNotChangeValue()
        {
            var value = false;
            var cut = Render<ElSwitch<bool>>(parameters => parameters
                .Add(x => x.ActiveValue, true)
                .Add(x => x.InactiveValue, false)
                .Add(x => x.Value, value)
                .Add(x => x.Loading, true)
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find(".el-switch__core").Click();

            Assert.False(value);
            Assert.Contains("is-loading", cut.Find(".el-switch").ClassList);
        }

        [Fact]
        public void BeforeChangeCanCancelSwitch()
        {
            var value = false;
            var cut = Render<ElSwitch<bool>>(parameters => parameters
                .Add(x => x.ActiveValue, true)
                .Add(x => x.InactiveValue, false)
                .Add(x => x.Value, value)
                .Add(x => x.BeforeChange, (_, _) => Task.FromResult(false))
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find(".el-switch__core").Click();

            Assert.False(value);
        }

        [Fact]
        public void ValueChangingCanCancelSwitch()
        {
            var value = false;
            var cut = Render<ElSwitch<bool>>(parameters => parameters
                .Add(x => x.ActiveValue, true)
                .Add(x => x.InactiveValue, false)
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanging, args => args.DisallowChange = true)
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find(".el-switch__core").Click();

            Assert.False(value);
        }
    }
}
