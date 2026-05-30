using Bunit;
using Element;
using Element.X;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Element.ComponentTests
{
    public class ElXComponentTests : BunitContext
    {
        public ElXComponentTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void BubbleRendersRoleContentAndAttachments()
        {
            var cut = Render<ElXBubble>(parameters => parameters
                .Add(x => x.Role, XMessageRole.User)
                .Add(x => x.Header, "Mystic")
                .Add(x => x.Content, "Build an AI page")
                .Add(x => x.Footer, "09:30")
                .Add(x => x.Attachments, new[]
                {
                    new XAttachmentItem { FileName = "brief.pdf", Size = "2 MB", Type = "pdf", Status = XAttachmentStatus.Success }
                }));

            var bubble = cut.Find(".el-x-bubble");
            Assert.Contains("el-x-bubble--user", bubble.ClassList);
            Assert.Contains("is-end", bubble.ClassList);
            Assert.Equal("Mystic", cut.Find(".el-x-bubble__header").TextContent.Trim());
            Assert.Contains("Build an AI page", cut.Find(".el-x-bubble__content").TextContent);
            Assert.Equal("brief.pdf", cut.Find(".el-x-files-card__name").TextContent.Trim());
        }

        [Fact]
        public void BubbleListRendersEmptyAndMessages()
        {
            var empty = Render<ElXBubbleList>();
            Assert.NotEmpty(empty.FindAll(".el-empty"));

            var messages = Render<ElXBubbleList>(parameters => parameters
                .Add(x => x.Items, new[]
                {
                    new XMessageItem { Role = XMessageRole.Assistant, Content = "Hello" },
                    new XMessageItem { Role = XMessageRole.User, Content = "Hi" }
                }));

            Assert.Equal(2, messages.FindAll(".el-x-bubble").Count);
            Assert.Contains("Hello", messages.Markup);
            Assert.Contains("Hi", messages.Markup);
        }

        [Fact]
        public void ConversationsSelectsItemAndUpdatesActiveId()
        {
            string active = null;
            XConversationItem selected = null;

            var cut = Render<ElXConversations>(parameters => parameters
                .Add(x => x.ActiveId, "a")
                .Add(x => x.ActiveIdChanged, value => active = value)
                .Add(x => x.OnSelect, item => selected = item)
                .Add(x => x.Items, new[]
                {
                    new XConversationItem { Id = "a", Title = "Current" },
                    new XConversationItem { Id = "b", Title = "Next", Group = "Today" }
                }));

            Assert.Contains("is-active", cut.FindAll(".el-x-conversations__item")[0].ClassList);
            cut.FindAll(".el-x-conversations__item")[1].Click();

            Assert.Equal("b", active);
            Assert.Equal("Next", selected.Title);
        }

        [Fact]
        public void PromptsEmitsSelectedPrompt()
        {
            XPromptItem selected = null;
            var cut = Render<ElXPrompts>(parameters => parameters
                .Add(x => x.OnSelect, item => selected = item)
                .Add(x => x.Items, new[]
                {
                    new XPromptItem { Id = "write", Title = "Write", Description = "Draft a release note" },
                    new XPromptItem { Id = "disabled", Title = "Disabled", Disabled = true }
                }));

            cut.Find(".el-x-prompts__item").Click();

            Assert.Equal("write", selected.Id);
        }

        [Fact]
        public void SenderSubmitsValueAndCanStop()
        {
            string submitted = null;
            var stopped = false;

            var cut = Render<ElXSender>(parameters => parameters
                .Add(x => x.Value, "Explain Blazor")
                .Add(x => x.OnSubmit, value => submitted = value)
                .Add(x => x.OnStop, () => stopped = true));

            cut.Find(".el-button").Click();
            Assert.Equal("Explain Blazor", submitted);

            cut = Render<ElXSender>(parameters => parameters
                .Add(x => x.Value, "Explain Blazor")
                .Add(x => x.Loading, true)
                .Add(x => x.OnSubmit, value => submitted = value)
                .Add(x => x.OnStop, () => stopped = true));

            cut.Find(".el-button--danger").Click();
            Assert.True(stopped);
        }

        [Fact]
        public void ThinkingTogglesContent()
        {
            var cut = Render<ElXThinking>(parameters => parameters
                .Add(x => x.Title, "Reasoning")
                .Add(x => x.Content, "Checking route constraints")
                .Add(x => x.DefaultExpanded, true));

            Assert.Contains("Checking route constraints", cut.Markup);
            cut.Find(".el-x-thinking__header").Click();
            Assert.DoesNotContain("Checking route constraints", cut.Markup);
        }

        [Fact]
        public void ThoughtChainRendersTimelineItems()
        {
            var cut = Render<ElXThoughtChain>(parameters => parameters
                .Add(x => x.Items, new[]
                {
                    new XThoughtItem
                    {
                        Title = "Plan",
                        Description = "Map components",
                        Duration = "1s",
                        Status = "done",
                        Type = "success"
                    }
                }));

            Assert.NotEmpty(cut.FindAll(".el-timeline-item"));
            Assert.Contains("Plan", cut.Markup);
            Assert.Contains("Map components", cut.Markup);
        }

        [Fact]
        public void MentionSenderSubmitsValue()
        {
            string submitted = null;
            var cut = Render<ElXMentionSender>(parameters => parameters
                .Add(x => x.Value, "/summarize roadmap")
                .Add(x => x.Options, new[] { new MentionOption { Value = "summarize", Label = "Summarize" } })
                .Add(x => x.OnSubmit, value => submitted = value));

            cut.Find(".el-button").Click();

            Assert.Equal("/summarize roadmap", submitted);
        }
    }
}
