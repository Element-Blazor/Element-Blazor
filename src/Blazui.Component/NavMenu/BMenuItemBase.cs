using Blazui.Component.EventArgs;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.NavMenu
{
    public class BMenuItemBase : BComponentBase, IMenuItem
    {
        [Parameter]
        public string Index { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Disabled { get; set; } = false;

        [Parameter]
        public string Route { get; set; }

        [Parameter]
        public object Model { get; set; }

        [Parameter]
        public string Icon { get; set; } = "el-icon-menu";

        [CascadingParameter]
        public BMenu TopMenu { get; set; }
        [CascadingParameter]
        public BMenuContainer Menu { get; set; }

        [CascadingParameter]
        public BSubMenuBase ParentMenu { get; set; }
        [CascadingParameter]
        public MenuOptions Options { get; set; }

        protected string textColor;
        protected string borderColor;

        protected bool isActive { get; set; }

        private string currentRoute;
        public void Activate()
        {
            isActive = true;
            textColor = Options.ActiveTextColor;
            borderColor = Options.ActiveTextColor;
            BackgroundColor = Options.HoverColor;

        }
        public void DeActivate()
        {
            isActive = false;
            textColor = Options.TextColor;
            borderColor = "transparent";
            BackgroundColor = Options.BackgroundColor;
        }

        [Parameter]
        public EventCallback<BChangeEventArgs<string>> OnRouteChanging { get; set; }
        protected string BackgroundColor { get; set; }

        protected override void OnInitialized()
        {
            BackgroundColor = Options.BackgroundColor;
            textColor = Options.TextColor;

            if (Options.DefaultActiveIndex == Index)
            {
                TopMenu.ActivateItem(this);
            }

            base.OnInitialized();
        }

        public void OnOver()
        {
            if (Options.Mode == MenuMode.Horizontal && ParentMenu != null)
            {
                ParentMenu.KeepSubMenuOpen();
            }
            if (Options.Mode == MenuMode.Horizontal || string.IsNullOrWhiteSpace(Options.HoverColor))
            {
                return;
            }
            BackgroundColor = Options.HoverColor;
        }

        public void OnOut()
        {
            if (Options.Mode == MenuMode.Horizontal)
            {
                BackgroundColor = Options.BackgroundColor;
                return;
            }
            BackgroundColor = isActive ? Options.HoverColor : Options.BackgroundColor;
        }

        public async Task OnClickAsync()
        {
            if (ParentMenu != null && TopMenu.Mode == MenuMode.Horizontal)
            {
                await ParentMenu.CloseAsync();
            }
            if (!string.IsNullOrEmpty(Route))
            {
                TopMenu.ActivateItem(this);
                if (OnRouteChanging.HasDelegate)
                {
                    var arg = new BChangeEventArgs<string>();
                    arg.NewValue = Route;
                    arg.OldValue = currentRoute;
                    await OnRouteChanging.InvokeAsync(arg);
                    if (arg.DisallowChange)
                    {
                        return;
                    }
                }
                NavigationManager.NavigateTo(Route);
            }
        }
    }
}
