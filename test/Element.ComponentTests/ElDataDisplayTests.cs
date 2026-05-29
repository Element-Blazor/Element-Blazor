using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Element.ComponentTests
{
    public class ElDataDisplayTests : BunitContext
    {
        public ElDataDisplayTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void AvatarRendersImageIconAndCustomSize()
        {
            var image = Render<ElAvatar>(parameters => parameters
                .Add(x => x.Src, "/avatar.png")
                .Add(x => x.Alt, "User avatar")
                .Add(x => x.Shape, AvatarShape.Square)
                .Add(x => x.CustomSize, "48"));

            var avatar = image.Find(".el-avatar");
            Assert.Contains("el-avatar--square", avatar.ClassList);
            Assert.Contains("width:48px", avatar.GetAttribute("style"));
            Assert.Equal("/avatar.png", image.Find("img").GetAttribute("src"));
            Assert.Equal("User avatar", image.Find("img").GetAttribute("alt"));

            var icon = Render<ElAvatar>(parameters => parameters
                .Add(x => x.Icon, "user")
                .Add(x => x.Size, ElementSize.Small));

            Assert.Contains("el-avatar--icon", icon.Find(".el-avatar").ClassList);
            Assert.Contains("el-avatar--small", icon.Find(".el-avatar").ClassList);
            Assert.Contains("el-icon-user", icon.Find("i").ClassList);
        }

        [Fact]
        public void BadgeSupportsMaxDotHiddenAndOffset()
        {
            var badge = Render<ElBadge>(parameters => parameters
                .Add(x => x.Value, 120)
                .Add(x => x.Max, 99)
                .Add(x => x.Type, BadgeType.Danger)
                .Add(x => x.OffsetX, 8)
                .Add(x => x.OffsetY, 4)
                .AddChildContent("<button>Inbox</button>"));

            var content = badge.Find(".el-badge__content");
            Assert.Equal("99+", content.TextContent.Trim());
            Assert.Contains("el-badge__content--danger", content.ClassList);
            Assert.Contains("right:-8px", content.GetAttribute("style"));
            Assert.Contains("margin-top:4px", content.GetAttribute("style"));

            var dot = Render<ElBadge>(parameters => parameters
                .Add(x => x.Dot, true)
                .AddChildContent("Notice"));

            Assert.Contains("is-dot", dot.Find(".el-badge__content").ClassList);
            Assert.Equal(string.Empty, dot.Find(".el-badge__content").TextContent.Trim());

            var hidden = Render<ElBadge>(parameters => parameters
                .Add(x => x.Hidden, true)
                .AddChildContent("Notice"));

            Assert.Empty(hidden.FindAll(".el-badge__content"));
        }

        [Fact]
        public void CardRendersChildContentBodyStyleAndFooter()
        {
            var card = Render<ElCard>(parameters => parameters
                .Add(x => x.Header, "Profile")
                .Add(x => x.BodyStyle, "padding:8px")
                .Add(x => x.Footer, "Footer")
                .AddChildContent("Body"));

            Assert.Equal("Profile", card.Find(".el-card__header").TextContent.Trim());
            Assert.Equal("Body", card.Find(".el-card__body").TextContent.Trim());
            Assert.Contains("padding:8px", card.Find(".el-card__body").GetAttribute("style"));
            Assert.Equal("Footer", card.Find(".el-card__footer").TextContent.Trim());
        }

        [Fact]
        public void DescriptionsRegistersItemsAndRendersBorderedRows()
        {
            var cut = Render(builder =>
            {
                builder.OpenComponent<ElDescriptions>(0);
                builder.AddAttribute(1, nameof(ElDescriptions.Title), "Account");
                builder.AddAttribute(2, nameof(ElDescriptions.Border), true);
                builder.AddAttribute(3, nameof(ElDescriptions.Column), 2);
                builder.AddAttribute(4, nameof(ElDescriptions.ChildContent), (RenderFragment)(child =>
                {
                    child.OpenComponent<ElDescriptionsItem>(0);
                    child.AddAttribute(1, nameof(ElDescriptionsItem.Label), "Name");
                    child.AddAttribute(2, nameof(ElDescriptionsItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Alice")));
                    child.CloseComponent();
                    child.OpenComponent<ElDescriptionsItem>(3);
                    child.AddAttribute(4, nameof(ElDescriptionsItem.Label), "Role");
                    child.AddAttribute(5, nameof(ElDescriptionsItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Admin")));
                    child.CloseComponent();
                }));
                builder.CloseComponent();
            });

            Assert.Contains("is-bordered", cut.Find(".el-descriptions").ClassList);
            Assert.Contains("Account", cut.Markup);
            Assert.Contains("Name", cut.Markup);
            Assert.Contains("Alice", cut.Markup);
            Assert.Contains("Role", cut.Markup);
            Assert.Contains("Admin", cut.Markup);
            Assert.Equal(2, cut.FindAll(".el-descriptions__content").Count);
        }

        [Fact]
        public void EmptyRendersDescriptionImageAndBottomSlot()
        {
            var empty = Render<ElEmpty>(parameters => parameters
                .Add(x => x.Image, "/empty.svg")
                .Add(x => x.ImageSize, "96")
                .Add(x => x.Description, "Nothing here")
                .AddChildContent("<button>Create</button>"));

            Assert.Equal("/empty.svg", empty.Find("img").GetAttribute("src"));
            Assert.Contains("width:96px", empty.Find(".el-empty__image").GetAttribute("style"));
            Assert.Equal("Nothing here", empty.Find(".el-empty__description").TextContent.Trim());
            Assert.Equal("Create", empty.Find(".el-empty__bottom").TextContent.Trim());
        }

        [Fact]
        public void ProgressRendersLineAndCircleStates()
        {
            var line = Render<ElProgress>(parameters => parameters
                .Add(x => x.Percentage, 68)
                .Add(x => x.TextInside, true)
                .Add(x => x.StrokeWidth, 12)
                .Add(x => x.Status, ProgressStatus.Success));

            Assert.Contains("el-progress--line", line.Find(".el-progress").ClassList);
            Assert.Contains("is-success", line.Find(".el-progress").ClassList);
            Assert.Contains("height:12px", line.Find(".el-progress-bar__outer").GetAttribute("style"));
            Assert.Contains("width:68%", line.Find(".el-progress-bar__inner").GetAttribute("style"));
            Assert.Equal("68%", line.Find(".el-progress-bar__innerText").TextContent.Trim());

            var circle = Render<ElProgress>(parameters => parameters
                .Add(x => x.Type, ProgressType.Circle)
                .Add(x => x.Percentage, 42)
                .Add(x => x.Width, 80)
                .Add(x => x.Color, "#123456"));

            Assert.Contains("el-progress--circle", circle.Find(".el-progress").ClassList);
            Assert.Contains("height:80px", circle.Find(".el-progress-circle").GetAttribute("style"));
            Assert.Equal("#123456", circle.Find(".el-progress-circle__path").GetAttribute("stroke"));
            Assert.Equal("42%", circle.Find(".el-progress__text").TextContent.Trim());
        }

        [Fact]
        public void ResultRendersIconTitleSubtitleAndExtra()
        {
            var result = Render<ElResult>(parameters => parameters
                .Add(x => x.Icon, ResultIcon.Success)
                .Add(x => x.Title, "Done")
                .Add(x => x.SubTitle, "Saved")
                .Add(x => x.Extra, "Continue"));

            Assert.Contains("el-result-icon--success", result.Find(".el-result__icon i").ClassList);
            Assert.Equal("Done", result.Find(".el-result__title").TextContent.Trim());
            Assert.Equal("Saved", result.Find(".el-result__subtitle").TextContent.Trim());
            Assert.Equal("Continue", result.Find(".el-result__extra").TextContent.Trim());
        }

        [Fact]
        public void SkeletonShowsTemplateWhileLoadingAndContentWhenReady()
        {
            var loading = Render<ElSkeleton>(parameters => parameters
                .Add(x => x.Loading, true)
                .Add(x => x.Animated, true)
                .Add(x => x.Template, (RenderFragment)(builder =>
                {
                    builder.OpenComponent<ElSkeletonItem>(0);
                    builder.AddAttribute(1, nameof(ElSkeletonItem.Variant), SkeletonItemVariant.Image);
                    builder.AddAttribute(2, nameof(ElSkeletonItem.Width), "120");
                    builder.CloseComponent();
                }))
                .AddChildContent("Loaded"));

            Assert.Contains("is-animated", loading.Find(".el-skeleton").ClassList);
            Assert.Contains("el-skeleton__image", loading.Find(".el-skeleton__item").ClassList);
            Assert.DoesNotContain("Loaded", loading.Markup);

            var ready = Render<ElSkeleton>(parameters => parameters
                .Add(x => x.Loading, false)
                .AddChildContent("Loaded"));

            Assert.Contains("Loaded", ready.Markup);
            Assert.Empty(ready.FindAll(".el-skeleton"));
        }

        [Fact]
        public void TimelineRendersItemsWithTimestampAndIcon()
        {
            var cut = Render(builder =>
            {
                builder.OpenComponent<ElTimeline>(0);
                builder.AddAttribute(1, nameof(ElTimeline.ChildContent), (RenderFragment)(child =>
                {
                    child.OpenComponent<ElTimelineItem>(0);
                    child.AddAttribute(1, nameof(ElTimelineItem.Timestamp), "2026-05-29");
                    child.AddAttribute(2, nameof(ElTimelineItem.Placement), TimelinePlacement.Top);
                    child.AddAttribute(3, nameof(ElTimelineItem.Icon), "check");
                    child.AddAttribute(4, nameof(ElTimelineItem.Type), "success");
                    child.AddAttribute(5, nameof(ElTimelineItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Released")));
                    child.CloseComponent();
                }));
                builder.CloseComponent();
            });

            Assert.Single(cut.FindAll(".el-timeline-item"));
            Assert.Contains("el-timeline-item__node--success", cut.Find(".el-timeline-item__node").ClassList);
            Assert.Contains("el-icon-check", cut.Find(".el-timeline-item__node i").ClassList);
            Assert.Contains("is-top", cut.Find(".el-timeline-item__timestamp").ClassList);
            Assert.Contains("Released", cut.Markup);
        }

        [Fact]
        public void StatisticAndCountdownRenderFormattedValues()
        {
            var statistic = Render<ElStatistic>(parameters => parameters
                .Add(x => x.Title, "Revenue")
                .Add(x => x.Value, 1234.567)
                .Add(x => x.Precision, 1)
                .Add(x => x.PrefixText, "$")
                .Add(x => x.SuffixText, "USD"));

            Assert.Equal("Revenue", statistic.Find(".el-statistic__head").TextContent.Trim());
            Assert.Equal("1,234.6", statistic.Find(".el-statistic__number").TextContent.Trim());
            Assert.Equal("$", statistic.Find(".el-statistic__prefix").TextContent.Trim());
            Assert.Equal("USD", statistic.Find(".el-statistic__suffix").TextContent.Trim());

            var countdown = Render<ElCountdown>(parameters => parameters
                .Add(x => x.Title, "Remaining")
                .Add(x => x.Value, DateTime.Now.AddSeconds(65))
                .Add(x => x.Format, "HH:mm:ss"));

            Assert.Contains("el-countdown", countdown.Find(".el-statistic").ClassList);
            Assert.Matches(@"\d{2}:\d{2}:\d{2}", countdown.Find(".el-statistic__number").TextContent.Trim());
        }
    }
}
