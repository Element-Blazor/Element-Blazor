using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Element.ComponentTests
{
    public class ElFeedbackTests : BunitContext
    {
        public ElFeedbackTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void AlertRendersTypeDescriptionIconAndCloses()
        {
            var closed = false;
            var cut = Render<ElAlert>(parameters => parameters
                .Add(x => x.Title, "Saved")
                .Add(x => x.Description, "The record was saved.")
                .Add(x => x.Type, MessageType.Success)
                .Add(x => x.ShowIcon, true)
                .Add(x => x.OnClose, () => closed = true));

            Assert.Contains("el-alert--success", cut.Find(".el-alert").ClassList);
            Assert.Contains("is-description", cut.Find(".el-alert").ClassList);
            Assert.Contains("el-icon-success", cut.Find(".el-alert__icon").ClassList);

            cut.Find(".el-alert__closebtn").Click();

            Assert.True(closed);
            Assert.Empty(cut.FindAll(".el-alert"));
        }

        [Fact]
        public void DrawerBindsVisibilityAndRespectsBeforeClose()
        {
            var visible = true;
            var closed = 0;
            var allowClose = false;
            var cut = Render<ElDrawer>(parameters => parameters
                .Add(x => x.ModelValue, visible)
                .Add(x => x.ModelValueChanged, value => visible = value)
                .Add(x => x.Title, "Settings")
                .Add(x => x.Direction, DrawerDirection.Ltr)
                .Add(x => x.Size, "320px")
                .Add(x => x.BeforeClose, () => Task.FromResult(allowClose))
                .Add(x => x.OnClosed, () => closed++)
                .AddChildContent("Content"));

            Assert.Contains("el-drawer__open", cut.Find(".el-drawer__wrapper").ClassList);
            Assert.Contains("ltr", cut.Find(".el-drawer").ClassList);
            Assert.Contains("width:320px", cut.Find(".el-drawer").GetAttribute("style"));

            cut.Find(".el-drawer__close-btn").Click();
            Assert.True(visible);
            Assert.Equal(0, closed);

            allowClose = true;
            cut.Find(".el-drawer__close-btn").Click();

            Assert.False(visible);
            Assert.Equal(1, closed);
        }

        [Fact]
        public void TooltipPopoverAndPopconfirmToggleAndEmitActions()
        {
            var tooltip = Render<ElTooltip>(parameters => parameters
                .Add(x => x.Content, "Helpful text")
                .AddChildContent("<button>Hover</button>"));

            tooltip.Find(".el-tooltip__trigger").MouseOver();
            Assert.Contains("Helpful text", tooltip.Markup);
            tooltip.Find(".el-tooltip__trigger").MouseOut();
            Assert.DoesNotContain("Helpful text", tooltip.Markup);

            var popover = Render<ElPopover>(parameters => parameters
                .Add(x => x.Title, "Details")
                .Add(x => x.Content, "More information")
                .Add(x => x.Reference, (RenderFragment)(builder => builder.AddContent(0, "Open"))));

            popover.Find(".el-popover__reference-wrapper").Click();
            Assert.Contains("Details", popover.Markup);
            Assert.Contains("More information", popover.Markup);

            var confirmed = 0;
            var cancelled = 0;
            var popconfirm = Render<ElPopconfirm>(parameters => parameters
                .Add(x => x.Title, "Delete?")
                .Add(x => x.OnConfirm, () => confirmed++)
                .Add(x => x.OnCancel, () => cancelled++)
                .AddChildContent("<button>Delete</button>"));

            popconfirm.Find(".el-popconfirm__reference-wrapper").Click();
            Assert.Contains("Delete?", popconfirm.Markup);

            popconfirm.FindAll(".el-popconfirm__action button").Last().Click();
            Assert.Equal(1, confirmed);
            Assert.Empty(popconfirm.FindAll(".el-popconfirm"));

            popconfirm.Find(".el-popconfirm__reference-wrapper").Click();
            popconfirm.FindAll(".el-popconfirm__action button").First().Click();
            Assert.Equal(1, cancelled);
        }

        [Fact]
        public void MessageAndNotificationServicesRenderThroughPopup()
        {
            JSInterop.SetupVoid("RegisterAnimationBegin", _ => true);
            var cut = Render<ElPopup>();
            var messageService = Services.GetRequiredService<MessageService>();
            var notificationService = Services.GetRequiredService<NotificationService>();

            messageService.Show(new MessageInfo
            {
                Message = "Saved",
                Type = MessageType.Success,
                ShowClose = true,
                Plain = true,
                Grouping = true
            });
            messageService.Show(new MessageInfo
            {
                Message = "Saved",
                Type = MessageType.Success,
                Grouping = true
            });
            notificationService.Show(new NotificationOption
            {
                Title = "Deploy",
                Message = "Finished",
                Type = MessageType.Success,
                Duration = 0,
                Position = NotificationPosition.BottomLeft
            });

            cut.Render();

            Assert.Contains("Saved", cut.Markup);
            Assert.Contains("el-message__badge", cut.Markup);
            Assert.Contains("Deploy", cut.Markup);
            Assert.Contains("Finished", cut.Markup);
            Assert.Contains("bottom", cut.Find(".el-notification").GetAttribute("style"));
        }

        [Fact]
        public void LoadingComponentWrapsContentAndCanHideMask()
        {
            var cut = Render<ElLoading>(parameters => parameters
                .Add(x => x.Loading, true)
                .Add(x => x.Text, "Loading")
                .Add(x => x.Background, "rgba(255,255,255,.5)")
                .AddChildContent("<div>Body</div>"));

            Assert.Contains("is-loading", cut.Find(".el-loading-parent").ClassList);
            Assert.Contains("Body", cut.Markup);
            Assert.Equal("Loading", cut.Find(".el-loading-text").TextContent.Trim());

            cut.Render(parameters => parameters
                .Add(x => x.Loading, false)
                .AddChildContent("<div>Body</div>"));

            Assert.Empty(cut.FindAll(".el-loading-mask"));
            Assert.Contains("Body", cut.Markup);
        }

        [Fact]
        public async Task MessageBoxPromptCreatesInputAndBeforeCloseCanBlock()
        {
            var messageBox = Services.GetRequiredService<MessageBox>();
            var popup = Render<ElPopup>();
            var attemptedActions = new List<MessageBoxAction>();
            var option = new MessageBoxOption
            {
                Message = "Name",
                ShowInput = true,
                InputValue = "Alice",
                BeforeClose = action =>
                {
                    attemptedActions.Add(action);
                    return Task.FromResult(action == MessageBoxAction.Confirm);
                }
            };

            var task = popup.InvokeAsync(() => messageBox.PromptAsync(option));
            popup.Render();

            Assert.Contains("Name", popup.Markup);
            Assert.Equal("Alice", popup.Find(".el-message-box__input input").GetAttribute("value"));

            var cancelButton = popup.FindAll(".el-message-box__btns button").First();
            Assert.Contains("取消", cancelButton.TextContent);
            cancelButton.Click();
            Assert.Contains(MessageBoxAction.Cancel, attemptedActions);

            popup.FindAll(".el-message-box__btns button").Last().Click();
            popup.WaitForAssertion(() => Assert.True(task.IsCompleted));
            var result = await task;
            Assert.Equal(MessageBoxResult.Ok, result.Result);
            Assert.Equal("Alice", result.Value);
        }
    }
}
