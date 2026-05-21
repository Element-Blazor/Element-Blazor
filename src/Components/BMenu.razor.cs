using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class BMenu : BMenuContainer
    {
        internal SemaphoreSlim SemaphoreSlim { get; private set; } = new SemaphoreSlim(1, 1);
        [Parameter]
        public MenuMode Mode { get; set; } = MenuMode.Vertical;
        protected string modeClass;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool CanCollapse { get; set; } = true;

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
        protected IMenuItem activeItem;
        [Parameter]
        public IMenuItem ActiveItem { get => activeItem; set => activeItem = value; }
        [Parameter]
        public EventCallback<IMenuItem> ActiveItemChanged { get; set; }
        public virtual void ActivateItem(IMenuItem item)
        {
            if (item == null)
                return;
            if (item.Disabled)
                return;
            if (item == activeItem)
                return;

            item.Activate();
            if (item.Menu is BSubMenu subMenu)
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
                Mode = Mode
            };

            base.OnInitialized();
        }
        protected override bool ShouldRender()
        {
            return true;
        }

        protected async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
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
            else
            {
                var isPrev = e.Key == "ArrowUp" || e.Key == "ArrowLeft";
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
