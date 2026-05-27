using Bunit;
using Element;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Xunit;

namespace Element.ComponentTests
{
    public class ElMentionTests : BunitContext
    {
        public ElMentionTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void FiltersAndSelectsMentionWithKeyboard()
        {
            string value = null;
            var cut = Render<ElMention>(parameters => parameters
                .Add(x => x.Options, new[]
                {
                    new MentionOption { Label = "Alice", Value = "alice" },
                    new MentionOption { Label = "Bob", Value = "bob" }
                })
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find("textarea").Input("@b");
            Assert.Single(cut.FindAll(".el-select-dropdown__item"));

            cut.Find("textarea").KeyDown("Enter");

            Assert.Equal("@bob", value);
        }

        [Fact]
        public void ClearableClearsMentionValue()
        {
            string value = "hello @alice";
            var cut = Render<ElMention>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.Clearable, true)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.Options, new List<MentionOption>()));

            cut.Find(".el-mention__clear").Click();

            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void DisabledMentionOptionDoesNotChangeValue()
        {
            string value = "@a";
            var cut = Render<ElMention>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.Options, new[]
                {
                    new MentionOption { Label = "Alice", Value = "alice", Disabled = true }
                }));

            cut.Find("textarea").Input("@a");
            cut.Find(".el-select-dropdown__item").Click();

            Assert.Equal("@a", value);
        }
    }
}
