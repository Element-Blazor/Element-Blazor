using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Element.ComponentTests
{
    public class ElTimeSelectTests : BunitContext
    {
        public ElTimeSelectTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void RendersStepOptionsAndSelectsByClick()
        {
            var value = string.Empty;
            var changes = new List<string>();
            var cut = Render<ElTimeSelect>(parameters => parameters
                .Add(x => x.Start, "08:00")
                .Add(x => x.End, "09:00")
                .Add(x => x.Step, "00:30")
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnChange, next => changes.Add(next)));

            cut.Find(".el-time-select").Click();
            var fragment = Render(GetDropdown().OptionContent);
            var items = fragment.FindAll(".el-select-dropdown__item");

            Assert.Equal(new[] { "08:00", "08:30", "09:00" }, items.Select(x => x.TextContent.Trim()));

            items[1].Click();

            Assert.Equal("08:30", value);
            Assert.Equal(new[] { "08:30" }, changes);
        }

        [Fact]
        public void KeyboardNavigationSelectsActiveOptionAndSkipsDisabled()
        {
            var value = string.Empty;
            var cut = Render<ElTimeSelect>(parameters => parameters
                .Add(x => x.Start, "08:00")
                .Add(x => x.End, "09:00")
                .Add(x => x.Step, "00:30")
                .Add(x => x.MinTime, "08:30")
                .Add(x => x.ValueChanged, next => value = next));

            var root = cut.Find(".el-time-select");
            root.KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

            var fragment = Render(GetDropdown().OptionContent);
            Assert.Equal(fragment.Find(".el-select-dropdown__item.hover").Id, root.GetAttribute("aria-activedescendant"));
            Assert.Equal("08:30", fragment.Find(".el-select-dropdown__item.hover").TextContent.Trim());

            root.KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
            fragment = Render(GetDropdown().OptionContent);
            Assert.Equal("09:00", fragment.Find(".el-select-dropdown__item.hover").TextContent.Trim());

            root.KeyDown(new KeyboardEventArgs { Key = "Enter" });

            Assert.Equal("09:00", value);
            Assert.Empty(Services.GetRequiredService<PopupService>().SelectDropDownOptions);
        }

        [Fact]
        public void HomeAndEndMoveToFirstAndLastEnabledOptions()
        {
            var cut = Render<ElTimeSelect>(parameters => parameters
                .Add(x => x.Start, "08:00")
                .Add(x => x.End, "10:00")
                .Add(x => x.Step, "00:30")
                .Add(x => x.MinTime, "08:30")
                .Add(x => x.MaxTime, "09:30"));

            var root = cut.Find(".el-time-select");
            root.KeyDown(new KeyboardEventArgs { Key = "End" });
            var fragment = Render(GetDropdown().OptionContent);

            Assert.Equal("09:30", fragment.Find(".el-select-dropdown__item.hover").TextContent.Trim());

            root.KeyDown(new KeyboardEventArgs { Key = "Home" });
            fragment = Render(GetDropdown().OptionContent);

            Assert.Equal("08:30", fragment.Find(".el-select-dropdown__item.hover").TextContent.Trim());
        }

        [Fact]
        public void EscapeClosesDropdownAndNotifiesVisibility()
        {
            var visibleChanges = new List<bool>();
            var cut = Render<ElTimeSelect>(parameters => parameters
                .Add(x => x.OnVisibleChange, visible => visibleChanges.Add(visible)));

            var root = cut.Find(".el-time-select");
            root.KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
            root.KeyDown(new KeyboardEventArgs { Key = "Escape" });

            Assert.Equal(new[] { true, false }, visibleChanges);
            Assert.Equal("false", cut.Find(".el-time-select").GetAttribute("aria-expanded"));
            Assert.Empty(Services.GetRequiredService<PopupService>().SelectDropDownOptions);
        }

        [Fact]
        public void FormItemReceivesSelectedTime()
        {
            var cut = Render<ElForm>(parameters => parameters
                .AddChildContent<ElFormItem<string>>(item => item
                    .Add(x => x.Name, "StartAt")
                    .AddChildContent<ElTimeSelect>(time => time
                        .Add(x => x.Start, "09:00")
                        .Add(x => x.End, "10:00")
                        .Add(x => x.Step, "01:00"))));

            cut.Find(".el-time-select").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
            cut.Find(".el-time-select").KeyDown(new KeyboardEventArgs { Key = "Enter" });

            Assert.Equal("09:00", cut.Instance.Values["StartAt"]);
        }

        private DropDownOption GetDropdown()
        {
            return Services.GetRequiredService<PopupService>().SelectDropDownOptions.Single();
        }
    }
}
