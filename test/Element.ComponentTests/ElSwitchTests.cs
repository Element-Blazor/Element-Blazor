using Bunit;
using Element;
using Element.ControlConfigs;
using Microsoft.AspNetCore.Components.Web;
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
        public void DefaultBoolSwitchTogglesWithoutExplicitValues()
        {
            var value = false;
            var cut = Render<ElSwitch<bool>>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.Id, "enabled")
                .Add(x => x.ActiveText, "On")
                .Add(x => x.InactiveText, "Off"));

            var root = cut.Find(".el-switch");
            var input = cut.Find("input");

            Assert.Equal("switch", root.GetAttribute("role"));
            Assert.Equal("false", root.GetAttribute("aria-checked"));
            Assert.Equal("Off", cut.Find(".el-switch__label--left").TextContent.Trim());
            Assert.Equal("On", cut.Find(".el-switch__label--right").TextContent.Trim());
            Assert.Contains("is-active", cut.Find(".el-switch__label--left").ClassList);
            Assert.Equal("enabled", input.Id);
            Assert.Equal("False", input.GetAttribute("value"));
            Assert.False(input.HasAttribute("checked"));

            cut.Find(".el-switch__core").Click();

            Assert.True(value);
            Assert.Equal("true", cut.Find(".el-switch").GetAttribute("aria-checked"));
            Assert.Contains("is-checked", cut.Find(".el-switch").ClassList);
            Assert.True(cut.Find("input").HasAttribute("checked"));
            Assert.Contains("is-active", cut.Find(".el-switch__label--right").ClassList);
        }

        [Fact]
        public void CustomActiveInactiveValuesToggleAndEmitEvents()
        {
            var value = "off";
            var modelValue = string.Empty;
            var changed = 0;
            ElementChangeEventArgs<string> changing = null;
            var cut = Render<ElSwitch<string>>(parameters => parameters
                .Add(x => x.ActiveValue, "on")
                .Add(x => x.InactiveValue, "off")
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.ModelValueChanged, next => modelValue = next)
                .Add(x => x.ValueChanging, args => changing = args)
                .Add(x => x.OnChanged, _ => changed++));

            cut.Find(".el-switch__core").Click();

            Assert.Equal("on", value);
            Assert.Equal("on", modelValue);
            Assert.Equal(1, changed);
            Assert.NotNull(changing);
            Assert.Equal("off", changing.OldValue);
            Assert.Equal("on", changing.NewValue);
            Assert.Equal("on", cut.Find("input").GetAttribute("value"));
            Assert.Equal("on", cut.Find(".el-switch").GetAttribute("aria-valuetext"));
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
            Assert.Equal("true", cut.Find(".el-switch").GetAttribute("aria-disabled"));
            Assert.Equal("true", cut.Find(".el-switch").GetAttribute("aria-busy"));
            Assert.Contains("el-icon-loading", cut.Find(".el-switch__loading").ClassList);
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
        public void BeforeChangeReceivesOldAndNewValuesBeforeCommit()
        {
            var value = false;
            bool? oldValue = null;
            bool? newValue = null;
            var cut = Render<ElSwitch<bool>>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.BeforeChange, (old, next) =>
                {
                    oldValue = old;
                    newValue = next;
                    return Task.FromResult(true);
                })
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find(".el-switch__core").Click();

            Assert.False(oldValue);
            Assert.True(newValue);
            Assert.True(value);
        }

        [Fact]
        public void KeyboardActivatesSwitch()
        {
            var value = false;
            var cut = Render<ElSwitch<bool>>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find(".el-switch").KeyDown(new KeyboardEventArgs { Key = "Enter" });

            Assert.True(value);

            cut.Find(".el-switch").KeyDown(new KeyboardEventArgs { Key = " " });

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

        [Fact]
        public void GeneratedSwitchAppliesAttributeConfiguration()
        {
            var model = new GeneratedSwitchModel
            {
                Enabled = false,
                State = "N"
            };

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.EntityType, typeof(GeneratedSwitchModel))
                .Add(x => x.Value, model));

            var switches = cut.FindAll(".el-switch");
            Assert.Equal(2, switches.Count);
            Assert.Contains("No", cut.Markup);
            Assert.Contains("Yes", cut.Markup);
            Assert.Contains("Loading", cut.Markup);
            Assert.Contains("Ready", cut.Markup);
            Assert.Contains("is-loading", switches[0].ClassList);
            Assert.Contains("custom-loading", cut.Find(".el-switch__loading").ClassList);

            switches[0].KeyDown(new KeyboardEventArgs { Key = "Enter" });
            cut.FindAll(".el-switch__core")[1].Click();

            Assert.False(model.Enabled);
            Assert.Equal("Y", model.State);
        }

        private class GeneratedSwitchModel
        {
            [Switch(ActiveText = "Yes", InactiveText = "No", Loading = true, LoadingIcon = "custom-loading")]
            public bool Enabled { get; set; }

            [Switch(ActiveText = "Loading", InactiveText = "Ready", ActiveValue = "Y", InactiveValue = "N")]
            public string State { get; set; }
        }
    }
}
