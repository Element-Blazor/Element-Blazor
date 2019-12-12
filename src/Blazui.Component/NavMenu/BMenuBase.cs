using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blazui.Component.NavMenu
{
    public class BMenuBase : BMenuContainer
    {
        internal SemaphoreSlim SemaphoreSlim { get; private set; } = new SemaphoreSlim(1, 1);
        [Parameter]
        public MenuMode Mode { get; set; } = MenuMode.Vertical;
        protected string modeClass;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Collapse { get; set; } = false;

        [Parameter]
        public string BackgroundColor { get; set; }

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
            else
            {
                StateHasChanged();
            }
        }

        public void DeActive()
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
    }
}
