using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.NavMenu
{
    public class BMenuItemBase : ComponentBase, IMenuItem
    {
        [Inject]
        NavigationManager navigationManager { get; set; }
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


        [CascadingParameter]
        public BMenu TopMenu { get; set; }
        [CascadingParameter]
        public BMenuContainer Menu { get; set; }

        [CascadingParameter]
        public BSubMenu ParentMenu { get; set; }
        [CascadingParameter]
        public MenuOptions Options { get; set; }

        protected string textColor;
        protected string backgroundColor;
        protected string borderColor;

        protected bool isActive { get; set; }

        public void Active()
        {
            isActive = true;
            textColor = Options.ActiveTextColor;
            borderColor = Options.ActiveTextColor;
            backgroundColor = Options.HoverColor;

        }
        public void DeActive()
        {
            isActive = false;
            textColor = Options.TextColor;
            borderColor = "transparent";
            backgroundColor = Options.BackgroundColor;
        }


        protected override void OnInitialized()
        {
            backgroundColor = Options.BackgroundColor;
            textColor = Options.TextColor;

            if (Options.DefaultActiveIndex == Index)
            {
                TopMenu.ActivateItem(this);
            }

            base.OnInitialized();
        }

        public void OnOver()
        {
            //todo: 颜色值经过计算而得
            if (Options.Mode == MenuMode.Horizontal || !string.IsNullOrWhiteSpace(Options.HoverColor))
            {
                return;
            }
            backgroundColor = Options.HoverColor;
        }

        public void OnOut()
        {
            if (Options.Mode == MenuMode.Horizontal)
            {
                backgroundColor = Options.BackgroundColor;
                return;
            }
            backgroundColor = isActive ? Options.HoverColor : Options.BackgroundColor;
        }

        public void OnClick()
        {
            if (!string.IsNullOrEmpty(Route))
            {
                navigationManager.NavigateTo(Route);
            }

            TopMenu.ActivateItem(this);
        }
    }
}
