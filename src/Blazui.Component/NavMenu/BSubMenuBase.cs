using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.NavMenu
{
    public class BSubMenuBase : BMenuContainer, IMenuItem
    {
        [Parameter]
        public string Index { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Disabled { get; set; } = false;

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public object Model { get; set; }

        [CascadingParameter]
        public BMenuContainer Menu { get; set; }

        [CascadingParameter]
        public MenuOptions Options { get; set; }

        protected string textColor;
        protected string backgroundColor;
        protected string borderColor;

        protected bool isActive = false;
        protected bool isOpened = false;

        protected bool IsVertical
        {
            get
            {
                return Options != null && Options.Mode == "vertical";
            }
        }

        public void Active()
        {
            isActive = true;
            isOpened = true;
        }
        public void DeActive()
        {
            isActive = false;
            isOpened = false;
            borderColor = "transparent";

            TopMenu.DeActive();
            OnOut();
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            backgroundColor = Options.BackgroundColor;
            textColor = Options.TextColor;

            base.OnInitialized();
        }

        protected void OnOver()
        {
            //todo: 颜色值经过计算而得
            backgroundColor = Options.HoverColor;
            textColor = Options.ActiveTextColor;
            //opened = true;
        }

        protected void OnOut()
        {
            if (!isActive || isOpened)
            {
                backgroundColor = Options.BackgroundColor;
                textColor = Options.TextColor;
                return;
            }
            /*
            if (IsVertical && !opened) {
                backgroundColor = Options.HoverColor;
                textColor = Options.ActiveTextColor;
            }
            //opened = false;
            backgroundColor = (!IsVertical || !opened)?Options.HoverColor:Options.BackgroundColor;
            */
        }

        protected void OnClick()
        {
            if (IsVertical)
            {
                isOpened = !isOpened;
            }
        }
    }
}
