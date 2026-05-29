using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace Element.ComponentTests
{
    public class ElNavigationTests : BunitContext
    {
        public ElNavigationTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void StepsRenderStatusDirectionAndSlots()
        {
            var cut = Render(builder =>
            {
                builder.OpenComponent<ElSteps>(0);
                builder.AddAttribute(1, nameof(ElSteps.Active), 1);
                builder.AddAttribute(2, nameof(ElSteps.Direction), StepDirection.Vertical);
                builder.AddAttribute(3, nameof(ElSteps.FinishStatus), StepStatus.Success);
                builder.AddAttribute(4, nameof(ElSteps.ChildContent), (RenderFragment)(steps =>
                {
                    steps.OpenComponent<ElStep>(0);
                    steps.AddAttribute(1, nameof(ElStep.Title), "Created");
                    steps.CloseComponent();
                    steps.OpenComponent<ElStep>(2);
                    steps.AddAttribute(3, nameof(ElStep.Title), "Review");
                    steps.AddAttribute(4, nameof(ElStep.Description), "Pending review");
                    steps.CloseComponent();
                    steps.OpenComponent<ElStep>(5);
                    steps.AddAttribute(6, nameof(ElStep.Title), "Done");
                    steps.AddAttribute(7, nameof(ElStep.Status), StepStatus.Error);
                    steps.AddAttribute(8, nameof(ElStep.Icon), "close");
                    steps.CloseComponent();
                }));
                builder.CloseComponent();
            });

            Assert.Contains("el-steps--vertical", cut.Find(".el-steps").ClassList);
            var titles = cut.FindAll(".el-step__title");
            Assert.Contains("is-success", titles[0].ClassList);
            Assert.Contains("is-process", titles[1].ClassList);
            Assert.Contains("is-error", titles[2].ClassList);
            Assert.Equal("Pending review", cut.Find(".el-step__description").TextContent.Trim());
            Assert.Contains("el-icon-close", cut.FindAll(".el-step__icon-inner")[2].ClassList);
        }

        [Fact]
        public void AnchorRendersLinksAndRaisesClickChange()
        {
            JSInterop.SetupVoid("elementAnchorInit", _ => true).SetVoidResult();
            JSInterop.SetupVoid("elementAnchorScrollTo", _ => true).SetVoidResult();
            var changes = new List<string>();
            AnchorClickEventArgs clicked = null;

            var cut = Render(builder =>
            {
                builder.OpenComponent<ElAnchor>(0);
                builder.AddAttribute(1, nameof(ElAnchor.Type), AnchorType.Underline);
                builder.AddAttribute(2, nameof(ElAnchor.ModelValue), "#intro");
                builder.AddAttribute(3, nameof(ElAnchor.OnChange), EventCallback.Factory.Create<string>(this, href => changes.Add(href)));
                builder.AddAttribute(4, nameof(ElAnchor.OnClick), EventCallback.Factory.Create<AnchorClickEventArgs>(this, args => clicked = args));
                builder.AddAttribute(5, nameof(ElAnchor.ChildContent), (RenderFragment)(anchor =>
                {
                    anchor.OpenComponent<ElAnchorLink>(0);
                    anchor.AddAttribute(1, nameof(ElAnchorLink.Href), "#intro");
                    anchor.AddAttribute(2, nameof(ElAnchorLink.Title), "Intro");
                    anchor.CloseComponent();
                    anchor.OpenComponent<ElAnchorLink>(3);
                    anchor.AddAttribute(4, nameof(ElAnchorLink.Href), "#api");
                    anchor.AddAttribute(5, nameof(ElAnchorLink.Title), "API");
                    anchor.CloseComponent();
                }));
                builder.CloseComponent();
            });

            Assert.Contains("el-anchor--underline", cut.Find(".el-anchor").ClassList);
            Assert.Contains("is-active", cut.FindAll(".el-anchor__item")[0].ClassList);

            cut.FindAll(".el-anchor__link")[1].Click();

            Assert.Equal("#api", changes[0]);
            Assert.NotNull(clicked);
            Assert.Equal("#api", clicked.Href);
            Assert.Contains("is-active", cut.FindAll(".el-anchor__item")[1].ClassList);
        }

        [Fact]
        public async System.Threading.Tasks.Task BacktopVisibilityAndClickUseConfiguredTarget()
        {
            JSInterop.SetupVoid("elementBacktopInit", _ => true).SetVoidResult();
            JSInterop.SetupVoid("elementBacktopScrollTo", _ => true).SetVoidResult();
            var clicked = 0;
            var cut = Render<ElBacktop>(parameters => parameters
                .Add(x => x.Target, ".scroll")
                .Add(x => x.Right, 24)
                .Add(x => x.Bottom, 32)
                .Add(x => x.OnClick, _ => clicked++));

            Assert.Contains("display:none", cut.Find(".el-backtop").GetAttribute("style"));
            await cut.InvokeAsync(() => cut.Instance.SetVisible(true));

            var root = cut.Find(".el-backtop");
            Assert.Contains("right:24px", root.GetAttribute("style"));
            Assert.Contains("bottom:32px", root.GetAttribute("style"));
            Assert.Contains("is-visible", root.ClassList);

            await cut.InvokeAsync(() => root.Click());

            Assert.Equal(1, clicked);
        }

        [Fact]
        public void PageHeaderRendersSlotsAndBackEvent()
        {
            var backs = 0;
            var cut = Render<ElPageHeader>(parameters => parameters
                .Add(x => x.Title, "Return")
                .Add(x => x.Content, "Settings")
                .Add(x => x.BreadcrumbContent, (RenderFragment)(b => b.AddContent(0, "Home / Settings")))
                .Add(x => x.ExtraContent, (RenderFragment)(b => b.AddContent(0, "Save")))
                .AddChildContent("Details")
                .Add(x => x.OnBack, _ => backs++));

            Assert.Equal("Home / Settings", cut.Find(".el-page-header__breadcrumb").TextContent.Trim());
            Assert.Equal("Return", cut.Find(".el-page-header__title").TextContent.Trim());
            Assert.Equal("Settings", cut.Find(".el-page-header__content").TextContent.Trim());
            Assert.Equal("Save", cut.Find(".el-page-header__extra").TextContent.Trim());
            Assert.Equal("Details", cut.Find(".el-page-header__main").TextContent.Trim());

            cut.Find(".el-page-header__left").Click();

            Assert.Equal(1, backs);
        }

        [Fact]
        public async System.Threading.Tasks.Task AffixUpdatesFixedStateAndRaisesEvents()
        {
            JSInterop.SetupVoid("elementAffixInit", _ => true).SetVoidResult();
            var changes = new List<bool>();
            var scrolls = new List<AffixScrollEventArgs>();
            var cut = Render<ElAffix>(parameters => parameters
                .Add(x => x.Offset, 12)
                .Add(x => x.Position, AffixPosition.Bottom)
                .Add(x => x.OnChange, value => changes.Add(value))
                .Add(x => x.OnScroll, args => scrolls.Add(args))
                .AddChildContent("Toolbar"));

            Assert.DoesNotContain("is-fixed", cut.Find(".el-affix").ClassList);

            await cut.InvokeAsync(() => cut.Instance.SetFixed(true, 240));

            Assert.Contains("is-fixed", cut.Find(".el-affix").ClassList);
            Assert.Equal(new[] { true }, changes);
            Assert.Single(scrolls);
            Assert.Equal(240, scrolls[0].ScrollTop);
            Assert.Contains("el-affix--fixed", cut.Find(".el-affix__content").ClassList);
        }

        [Fact]
        public void BreadcrumbSupportsSeparatorIconAndReplaceNavigation()
        {
            var cut = Render(builder =>
            {
                builder.OpenComponent<ElBreadcrumb>(0);
                builder.AddAttribute(1, nameof(ElBreadcrumb.SeparatorIcon), "arrow-right");
                builder.AddAttribute(2, nameof(ElBreadcrumb.ChildContent), (RenderFragment)(crumbs =>
                {
                    crumbs.OpenComponent<ElBreadcrumbItem>(0);
                    crumbs.AddAttribute(1, nameof(ElBreadcrumbItem.To), "/dashboard");
                    crumbs.AddAttribute(2, nameof(ElBreadcrumbItem.Replace), true);
                    crumbs.AddAttribute(3, nameof(ElBreadcrumbItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Dashboard")));
                    crumbs.CloseComponent();
                    crumbs.OpenComponent<ElBreadcrumbItem>(4);
                    crumbs.AddAttribute(5, nameof(ElBreadcrumbItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Settings")));
                    crumbs.CloseComponent();
                }));
                builder.CloseComponent();
            });

            Assert.Contains("el-icon-arrow-right", cut.Find(".el-breadcrumb__separator").ClassList);

            cut.Find("a.el-breadcrumb__inner").Click();

            Assert.EndsWith("/dashboard", Services.GetRequiredService<NavigationManager>().Uri);
        }

        [Fact]
        public async System.Threading.Tasks.Task DropdownSupportsSplitButtonDisabledItemsAndCommand()
        {
            DropdownCommandEventArgs command = null;
            var cut = Render<ElDropdown>(parameters => parameters
                .Add(x => x.SplitButton, true)
                .Add(x => x.ButtonContent, (RenderFragment)(b => b.AddContent(0, "Actions")))
                .Add(x => x.OnCommand, args => command = args)
                .Add(x => x.Items, (RenderFragment)(items =>
                {
                    items.OpenComponent<ElDropdownItem>(0);
                    items.AddAttribute(1, nameof(ElDropdownItem.Command), "save");
                    items.AddAttribute(2, nameof(ElDropdownItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Save")));
                    items.CloseComponent();
                    items.OpenComponent<ElDropdownItem>(3);
                    items.AddAttribute(4, nameof(ElDropdownItem.Command), "delete");
                    items.AddAttribute(5, nameof(ElDropdownItem.Disabled), true);
                    items.AddAttribute(6, nameof(ElDropdownItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Delete")));
                    items.CloseComponent();
                })));

            Assert.Contains("el-dropdown__caret-button", cut.FindAll("button")[1].ClassList);

            await cut.InvokeAsync(() => cut.Instance.ShowDropDownAsync());
            var dropdown = Services.GetRequiredService<PopupService>().DropDownMenuOptions[0];
            var fragment = Render(builder =>
            {
                builder.OpenComponent<CascadingValue<DropDownOption>>(0);
                builder.AddAttribute(1, "Value", dropdown);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(content => content.AddContent(0, dropdown.OptionContent)));
                builder.CloseComponent();
            });

            Assert.Contains("is-disabled", fragment.FindAll(".el-dropdown-menu__item")[1].ClassList);

            fragment.FindAll(".el-dropdown-menu__item")[0].Click();

            Assert.NotNull(command);
            Assert.Equal("save", command.Command);
        }

        [Fact]
        public void MenuSupportsCollapseRouterThemeSelectAndKeyboard()
        {
            var selected = new List<string>();
            var cut = Render(builder =>
            {
                builder.OpenComponent<ElMenu>(0);
                builder.AddAttribute(1, nameof(ElMenu.Collapse), true);
                builder.AddAttribute(2, nameof(ElMenu.Router), false);
                builder.AddAttribute(3, nameof(ElMenu.Theme), MenuTheme.Dark);
                builder.AddAttribute(4, nameof(ElMenu.OnSelect), EventCallback.Factory.Create<MenuSelectEventArgs>(this, args => selected.Add(args.Index)));
                builder.AddAttribute(5, nameof(ElMenu.ChildContent), (RenderFragment)(menu =>
                {
                    menu.OpenComponent<ElMenuItem>(0);
                    menu.AddAttribute(1, nameof(ElMenuItem.Index), "home");
                    menu.AddAttribute(2, nameof(ElMenuItem.Route), "/home");
                    menu.AddAttribute(3, nameof(ElMenuItem.Title), "Home");
                    menu.AddAttribute(4, nameof(ElMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Home")));
                    menu.CloseComponent();
                    menu.OpenComponent<ElMenuItem>(5);
                    menu.AddAttribute(6, nameof(ElMenuItem.Index), "settings");
                    menu.AddAttribute(7, nameof(ElMenuItem.Route), "/settings");
                    menu.AddAttribute(8, nameof(ElMenuItem.Title), "Settings");
                    menu.AddAttribute(9, nameof(ElMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Settings")));
                    menu.CloseComponent();
                }));
                builder.CloseComponent();
            });

            Assert.Contains("el-menu--collapse", cut.Find(".el-menu").ClassList);
            Assert.Contains("el-menu--dark", cut.Find(".el-menu").ClassList);
            Assert.DoesNotContain("<span>Home</span>", cut.Markup);

            cut.Find(".el-menu").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
            cut.Find(".el-menu").KeyDown(new KeyboardEventArgs { Key = "Enter" });

            Assert.Equal(new[] { "home" }, selected);
            Assert.False(Services.GetRequiredService<NavigationManager>().Uri.EndsWith("/home"));
        }

        [Fact]
        public async System.Threading.Tasks.Task TabsSupportClosableEditableStretchBeforeLeaveAndKeyboard()
        {
            JSInterop.Setup<int>("getClientWidth", _ => true).SetResult(80);
            JSInterop.Setup<int>("getPaddingLeft", _ => true).SetResult(0);
            JSInterop.Setup<int>("getPaddingRight", _ => true).SetResult(0);
            JSInterop.Setup<int>("getOffsetLeft", _ => true).SetResult(0);
            var active = "one";
            var closed = new List<string>();
            var tabs = new ObservableCollection<TabOption>
            {
                new TabOption { Name = "one", Title = "One", Content = "First", IsActive = true },
                new TabOption { Name = "two", Title = "Two", Content = "Second" },
                new TabOption { Name = "three", Title = "Three", Content = "Third" }
            };
            var cut = Render<ElTabs>(parameters => parameters
                .Add(x => x.DataSource, tabs)
                .Add(x => x.Editable, true)
                .Add(x => x.Stretch, true)
                .Add(x => x.ModelValue, active)
                .Add(x => x.ModelValueChanged, value => active = value)
                .Add(x => x.BeforeLeaveSync, (newName, oldName) => newName != "two")
                .Add(x => x.OnTabClose, tab => closed.Add(tab.Name)));

            Assert.Contains("is-stretch", cut.Find(".el-tabs").ClassList);
            Assert.NotNull(cut.Find(".el-tabs__new-tab"));
            Assert.Equal(3, cut.FindAll(".el-icon-close").Count);

            cut.Find(".el-tabs").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
            Assert.Equal("one", active);

            cut.Find(".el-tabs").KeyDown(new KeyboardEventArgs { Key = "End" });
            Assert.Equal("three", active);

            await cut.InvokeAsync(() => cut.FindAll(".el-icon-close")[2].Click());

            Assert.Equal(new[] { "three" }, closed);
        }
    }
}
