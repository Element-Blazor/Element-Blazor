using Bunit;
using Element;
using Element.X;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
            JSInterop.SetupVoid("elementXBubbleListScrollToEnd", _ => true).SetVoidResult();
            JSInterop.SetupVoid("execFocus", _ => true).SetVoidResult();
            JSInterop.Setup<int[]>("elementMentionGetSelection", _ => true).SetResult(new[] { 2, 2 });
            JSInterop.SetupVoid("elementMentionSetSelection", _ => true).SetVoidResult();
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
        public void ConversationsEmitsCreateRenameAndDelete()
        {
            var created = false;
            XConversationItem renamed = null;
            XConversationItem deleted = null;

            var cut = Render<ElXConversations>(parameters => parameters
                .Add(x => x.ShowCreate, true)
                .Add(x => x.ShowActions, true)
                .Add(x => x.OnCreate, () => created = true)
                .Add(x => x.OnRename, item => renamed = item)
                .Add(x => x.OnDelete, item => deleted = item)
                .Add(x => x.Items, new[]
                {
                    new XConversationItem { Id = "a", Title = "Current" }
                }));

            cut.Find(".el-x-conversations__create").Click();
            cut.Find("[aria-label='Rename Current']").Click();
            cut.Find("[aria-label='Delete Current']").Click();

            Assert.True(created);
            Assert.Equal("a", renamed.Id);
            Assert.Equal("a", deleted.Id);
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
        public void SenderClearsAndSupportsShortcutSubmit()
        {
            string value = "Explain Blazor";
            string submitted = null;
            string cleared = null;

            var cut = Render<ElXSender>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnSubmit, next => submitted = next)
                .Add(x => x.OnClear, next => cleared = next)
                .Add(x => x.SubmitOnEnter, false)
                .Add(x => x.SubmitOnCtrlEnter, true)
                .Add(x => x.ClearOnSubmit, true)
                .Add(x => x.ShowClearButton, true));

            cut.Find("textarea").KeyDown(new KeyboardEventArgs { Key = "Enter" });
            Assert.Null(submitted);

            cut.Find("textarea").KeyDown(new KeyboardEventArgs { Key = "Enter", CtrlKey = true });
            Assert.Equal("Explain Blazor", submitted);
            Assert.Equal("Explain Blazor", cleared);
            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public async Task AttachmentsMapsUploadStatusAndTemplate()
        {
            IList<XAttachmentItem> latest = null;
            RenderFragment<XAttachmentItem> template = item => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "attachment-template");
                builder.AddContent(2, $"{item.FileName}:{item.Status}:{item.Size}:{item.Type}");
                builder.CloseElement();
            };

            var file = new UploadModel
            {
                Id = "file-1",
                FileName = "report.pdf",
                Size = 2048,
                Url = "/files/report.pdf",
                Status = UploadStatus.UnStart
            };

            var cut = Render<ElXAttachments>(parameters => parameters
                .Add(x => x.UploadUrl, "/upload")
                .Add(x => x.ItemTemplate, template)
                .Add(x => x.ItemsChanged, items => latest = items));

            await cut.InvokeAsync(() => cut.FindComponent<ElUpload>().Instance.OnChange.InvokeAsync(new UploadChangeEventArgs
            {
                File = file,
                FileList = new IFileModel[] { file }
            }));
            cut.Render();

            Assert.Contains("report.pdf:Ready:2 KB:pdf", cut.Find(".attachment-template").TextContent);

            await cut.InvokeAsync(() => cut.FindComponent<ElUpload>().Instance.OnProgress.InvokeAsync(new UploadProgressEventArgs
            {
                File = file,
                FileList = new IFileModel[] { file },
                Percent = 45
            }));

            Assert.Equal(XAttachmentStatus.Uploading, latest[0].Status);
            Assert.Equal(45, latest[0].Progress);

            file.Status = UploadStatus.Failure;
            file.Message = "network error";
            await cut.InvokeAsync(() => cut.FindComponent<ElUpload>().Instance.OnError.InvokeAsync(new UploadRequestEventArgs
            {
                File = file,
                FileList = new IFileModel[] { file },
                Response = UploadRequestResult.Failure("network error")
            }));

            Assert.Equal(XAttachmentStatus.Error, latest[0].Status);
            Assert.Equal("network error", latest[0].Error);
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

        [Fact]
        public void MentionSenderAppliesSelectedCommandAndSubmitsShortcut()
        {
            string value = null;
            string submitted = null;
            MentionOption selected = null;

            var cut = Render<ElXMentionSender>(parameters => parameters
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnCommandSelect, option => selected = option)
                .Add(x => x.OnSubmit, next => submitted = next)
                .Add(x => x.ApplyCommandOnSelect, true)
                .Add(x => x.SubmitOnEnter, false)
                .Add(x => x.SubmitOnCtrlEnter, true)
                .Add(x => x.Options, new[] { new MentionOption { Value = "summarize", Label = "Summarize" } }));

            cut.Find("textarea").Input("/s");
            cut.Find("textarea").KeyDown(new KeyboardEventArgs { Key = "Enter" });

            Assert.Equal("summarize", selected.Value);
            Assert.Equal("/summarize ", value);

            cut.Render(parameters => parameters.Add(x => x.Value, value));
            cut.Find("textarea").KeyDown(new KeyboardEventArgs { Key = "Enter", CtrlKey = true });

            Assert.Equal("/summarize ", submitted);
        }
    }
}
