using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElMenu : ElMenuContainer
    {
        internal SemaphoreSlim SemaphoreSlim { get; private set; } = new SemaphoreSlim(1, 1);
        [Parameter]
        public MenuMode Mode { get; set; } = MenuMode.Vertical;
        protected string modeClass;
        protected string menuClass;
        protected string menuStyle;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool CanCollapse { get; set; } = true;

        [Parameter]
        public bool Collapse { get; set; }

        [Parameter]
        public bool IsCollapse
        {
            get => Collapse;
            set => Collapse = value;
        }

        [Parameter]
        public bool Router { get; set; } = true;

        [Parameter]
        public bool UniqueOpened { get; set; }

        [Parameter]
        public MenuTrigger MenuTrigger { get; set; } = MenuTrigger.Hover;

        [Parameter]
        public string Trigger
        {
            get => MenuTrigger.ToString().ToLowerInvariant();
            set => MenuTrigger = string.Equals(value, "click", StringComparison.OrdinalIgnoreCase)
                ? MenuTrigger.Click
                : MenuTrigger.Hover;
        }

        [Parameter]
        public string PopperClass { get; set; }

        [Parameter]
        public string PopperStyle { get; set; }

        [Parameter]
        public string PopperEffect { get; set; } = "dark";

        [Parameter]
        public MenuTheme Theme { get; set; } = MenuTheme.Light;

        [Parameter]
        public EventCallback<MenuSelectEventArgs> OnSelect { get; set; }

        [Parameter]
        public EventCallback<string> OnOpen { get; set; }

        [Parameter]
        public EventCallback<string> OnClose { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// 菜单匹配方法，参数为当前菜单的路由
        /// </summary>
        [Parameter]
        public Func<string, bool> Match { get; set; }

        [Parameter]
        public string TextColor { get; set; }

        [Parameter]
        public string ActiveTextColor { get; set; } = "#409EFF";

        [Parameter]
        public string HoverColor { get; set; } = "#ecf5ff";

        [Parameter]
        public string DefaultActive { get; set; }

        [Parameter]
        public string DefaultActiveIndex
        {
            get => DefaultActive;
            set => DefaultActive = value;
        }

        protected IMenuItem activeItem;
        [Parameter]
        public IMenuItem ActiveItem { get => activeItem; set => activeItem = value; }
        [Parameter]
        public EventCallback<IMenuItem> ActiveItemChanged { get; set; }
        internal async Task NotifySelectAsync(ElMenuItem item)
        {
            if (!OnSelect.HasDelegate || item == null)
            {
                return;
            }

            await OnSelect.InvokeAsync(new MenuSelectEventArgs
            {
                Index = item.EffectiveIndex,
                IndexPath = item.GetIndexPath(),
                Route = item.Route,
                Item = item
            });
        }

        internal async Task NotifyOpenAsync(ElSubMenu item)
        {
            if (item == null)
            {
                return;
            }

            if (UniqueOpened)
            {
                foreach (var child in Children.OfType<ElSubMenu>().Where(x => x != item))
                {
                    await child.CloseFromMenuAsync(false);
                }
            }

            if (OnOpen.HasDelegate)
            {
                await OnOpen.InvokeAsync(item.EffectiveIndex);
            }
        }

        internal async Task NotifyCloseAsync(ElSubMenu item)
        {
            if (OnClose.HasDelegate && item != null)
            {
                await OnClose.InvokeAsync(item.EffectiveIndex);
            }
        }

        public virtual void ActivateItem(IMenuItem item)
        {
            if (Disabled)
                return;
            if (item == null)
                return;
            if (item.Disabled)
                return;
            if (item == activeItem)
                return;

            item.Activate();
            if (item.Menu is ElSubMenu subMenu)
            {
                subMenu.Activate();
            }
            if (activeItem != null)
                activeItem.DeActivate();

            activeItem = item;
            if (ActiveItemChanged.HasDelegate)
            {
                ActiveItemChanged.InvokeAsync(activeItem);
            }
        }

        public void DeActiveItem()
        {
            if (activeItem == null)
            {
                return;
            }
            ActiveItem.DeActivate();
            ActiveItem = null;
        }
        protected MenuOptions Options { get; set; }

        protected override void OnInitialized()
        {
            modeClass = $"el-menu--{Mode.ToString().ToLower()}";

            Options = new MenuOptions
            {
                BackgroundColor = BackgroundColor,
                ActiveTextColor = ActiveTextColor,
                TextColor = TextColor,
                DefaultActiveIndex = DefaultActive,
                HoverColor = HoverColor,
                Mode = Mode,
                Disabled = Disabled,
                Collapse = Collapse,
                Router = Router,
                MenuTrigger = MenuTrigger,
                PopperClass = PopperClass,
                PopperStyle = PopperStyle,
                PopperEffect = PopperEffect,
                Theme = Theme
            };

            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            modeClass = $"el-menu--{Mode.ToString().ToLower()}";
            menuClass = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add(modeClass, "el-menu", Cls)
                .AddIf(Disabled, "is-disabled")
                .AddIf(Collapse, "el-menu--collapse")
                .AddIf(Theme == MenuTheme.Dark, "el-menu--dark")
                .ToString();
            menuStyle = HtmlPropertyBuilder.CreateCssStyleBuilder()
                .Add("overflow:auto")
                .AddIf(Collapse, "width:64px")
                .AddIf(!string.IsNullOrWhiteSpace(BackgroundColor), $"--el-menu-bg-color:{BackgroundColor}")
                .AddIf(!string.IsNullOrWhiteSpace(TextColor), $"--el-menu-text-color:{TextColor}")
                .AddIf(!string.IsNullOrWhiteSpace(ActiveTextColor), $"--el-menu-active-color:{ActiveTextColor}")
                .AddIf(!string.IsNullOrWhiteSpace(HoverColor), $"--el-menu-hover-bg-color:{HoverColor}")
                .Add(Style)
                .ToString();
            if (Options != null)
            {
                Options.BackgroundColor = BackgroundColor;
                Options.ActiveTextColor = ActiveTextColor;
                Options.TextColor = TextColor;
                Options.DefaultActiveIndex = DefaultActive;
                Options.HoverColor = HoverColor;
                Options.Mode = Mode;
                Options.Disabled = Disabled;
                Options.Collapse = Collapse;
                Options.Router = Router;
                Options.MenuTrigger = MenuTrigger;
                Options.PopperClass = PopperClass;
                Options.PopperStyle = PopperStyle;
                Options.PopperEffect = PopperEffect;
                Options.Theme = Theme;
            }
        }
        protected override bool ShouldRender()
        {
            return true;
        }

        protected async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (Disabled)
            {
                return;
            }
            if (e.Key != "ArrowDown" && e.Key != "ArrowRight" && e.Key != "ArrowUp" && e.Key != "ArrowLeft" && e.Key != "Home" && e.Key != "End" && e.Key != "Enter" && e.Key != " ")
            {
                return;
            }
            var enabledItems = Children.Where(x => !x.Disabled).ToList();
            if (!enabledItems.Any())
            {
                return;
            }
            var currentIndex = activeItem == null ? -1 : enabledItems.IndexOf(activeItem);
            if (e.Key == "Home")
            {
                currentIndex = 0;
            }
            else if (e.Key == "End")
            {
                currentIndex = enabledItems.Count - 1;
            }
            else if (e.Key == "Enter" || e.Key == " ")
            {
                if (activeItem is ElMenuItem menuItem)
                {
                    await menuItem.OnClickAsync();
                }
                else if (activeItem is ElSubMenu subMenu)
                {
                    await subMenu.ToggleByKeyboardAsync();
                }
                return;
            }
            else
            {
                var isPrev = Mode == MenuMode.Horizontal
                    ? e.Key == "ArrowLeft"
                    : e.Key == "ArrowUp";
                if (Mode == MenuMode.Horizontal && (e.Key == "ArrowUp" || e.Key == "ArrowDown"))
                {
                    return;
                }
                if (Mode == MenuMode.Vertical && (e.Key == "ArrowLeft" || e.Key == "ArrowRight"))
                {
                    return;
                }
                currentIndex = (currentIndex + (isPrev ? -1 : 1) + enabledItems.Count) % enabledItems.Count;
            }
            ActivateItem(enabledItems[currentIndex]);
            if (ActiveItemChanged.HasDelegate)
            {
                await ActiveItemChanged.InvokeAsync(activeItem);
            }
        }
    }
}
