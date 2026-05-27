using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
            JSInterop.Setup<int[]>("elementMentionGetSelection", _ => true).SetResult(new[] { 2, 2 });
            JSInterop.SetupVoid("elementMentionSetSelection", _ => true);
            JSInterop.SetupVoid("execFocus", _ => true);
        }

        [Fact]
        public void FiltersAndSelectsMentionWithKeyboard()
        {
            string value = null;
            MentionOption selected = null;
            var cut = Render<ElMention>(parameters => parameters
                .Add(x => x.Options, new[]
                {
                    new MentionOption { Label = "Alice", Value = "alice" },
                    new MentionOption { Label = "Bob", Value = "bob" }
                })
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnSelect, next => selected = next));

            cut.Find("textarea").Input("@b");
            Assert.Single(cut.FindAll(".el-select-dropdown__item"));

            cut.Find("textarea").KeyDown(new KeyboardEventArgs { Key = "Enter" });

            Assert.Equal("@bob", value);
            Assert.Equal("bob", selected.Value);
        }

        [Fact]
        public void SupportsMultipleTriggerCharacters()
        {
            string value = null;
            JSInterop.Setup<int[]>("elementMentionGetSelection", _ => true).SetResult(new[] { 4, 4 });

            var cut = Render<ElMention>(parameters => parameters
                .Add(x => x.Prefix, "@")
                .Add(x => x.Prefixes, new[] { "@", "#" })
                .Add(x => x.Options, new[]
                {
                    new MentionOption { Label = "Topic", Value = "topic" }
                })
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find("textarea").Input("#to");
            cut.Find("textarea").KeyDown(new KeyboardEventArgs { Key = "Enter" });

            Assert.Equal("#topic", value);
        }

        [Fact]
        public void ReplacesMentionAtCaretInsideTextareaContent()
        {
            string value = "Hello @al world";
            JSInterop.Setup<int[]>("elementMentionGetSelection", _ => true).SetResult(new[] { 9, 9 });

            var cut = Render<ElMention>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.Options, new[]
                {
                    new MentionOption { Label = "Alice", Value = "alice" }
                })
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find("textarea").Input("Hello @al world");
            cut.Find("textarea").KeyDown(new KeyboardEventArgs { Key = "Enter" });

            Assert.Equal("Hello @alice world", value);
        }

        [Fact]
        public void KeyboardNavigationSkipsDisabledItemsAndTabSelects()
        {
            string value = null;
            JSInterop.Setup<int[]>("elementMentionGetSelection", _ => true).SetResult(new[] { 2, 2 });

            var cut = Render<ElMention>(parameters => parameters
                .Add(x => x.Options, new[]
                {
                    new MentionOption { Label = "Alice", Value = "alice", Disabled = true },
                    new MentionOption { Label = "Bob", Value = "bob" }
                })
                .Add(x => x.ValueChanged, next => value = next));

            cut.Find("textarea").Input("@b");
            var option = cut.Find(".el-select-dropdown__item");
            Assert.Equal("true", option.GetAttribute("aria-selected"));

            cut.Find("textarea").KeyDown(new KeyboardEventArgs { Key = "Tab" });

            Assert.Equal("@bob", value);
        }

        [Fact]
        public void ExposesComboboxAriaAndActiveDescendant()
        {
            var cut = Render<ElMention>(parameters => parameters
                .Add(x => x.Options, new[]
                {
                    new MentionOption { Label = "Alice", Value = "alice" }
                }));

            cut.Find("textarea").Input("@a");
            var textarea = cut.Find("textarea");
            var listbox = cut.Find("[role='listbox']");
            var option = cut.Find("[role='option']");

            Assert.Equal("combobox", textarea.GetAttribute("role"));
            Assert.Equal("list", textarea.GetAttribute("aria-autocomplete"));
            Assert.Equal("listbox", textarea.GetAttribute("aria-haspopup"));
            Assert.Equal("true", textarea.GetAttribute("aria-expanded"));
            Assert.Equal(listbox.ParentElement.Id, textarea.GetAttribute("aria-controls"));
            Assert.Equal(option.Id, textarea.GetAttribute("aria-activedescendant"));
            Assert.Equal("true", option.GetAttribute("aria-selected"));
        }

        [Fact]
        public void SuggestionItemTemplateAliasRendersCustomContent()
        {
            RenderFragment<MentionOption> template = item => builder =>
            {
                builder.OpenElement(0, "strong");
                builder.AddAttribute(1, "class", "mention-template");
                builder.AddContent(2, $"{item.Label}:{item.Value}");
                builder.CloseElement();
            };

            var cut = Render<ElMention>(parameters => parameters
                .Add(x => x.Options, new[]
                {
                    new MentionOption { Label = "Alice", Value = "alice" }
                })
                .Add(x => x.SuggestionItemTemplate, template));

            cut.Find("textarea").Input("@a");

            Assert.Contains("mention-template", cut.Markup);
            Assert.Contains("Alice:alice", cut.Markup);
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
