using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.NavMenu
{
    public class BSubMenuBase : BMenuContainer, IMenuItem
    {
        protected ElementReference Element { get; set; }
        [Inject]
        PopupService PopupService { get; set; }

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
        private SubMenuOption subMenuOption;

        protected bool IsVertical
        {
            get
            {
                return Options != null && Options.Mode == MenuMode.Vertical;
            }
        }

        public void Activate()
        {
            isActive = true;
            if (TopMenu.Mode == MenuMode.Horizontal)
            {
                isOpened = true;
            }
        }
        public void DeActivate()
        {
            isActive = false;
            if (TopMenu.Mode == MenuMode.Horizontal)
            {
                isOpened = false;
            }
            borderColor = "transparent";

            //TopMenu.DeActive();
            OnOut();
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            backgroundColor = Options.BackgroundColor;
            textColor = Options.TextColor;

            base.OnInitialized();
        }

        protected async Task OnOverAsync()
        {
            if (TopMenu.Mode == MenuMode.Horizontal)
            {
                if (isOpened && subMenuOption.Closing)
                {
                    subMenuOption.Closing = false;
                    return;
                }
                var taskCompletionSource = new TaskCompletionSource<int>();
                subMenuOption = new SubMenuOption()
                {
                    SubMenu = this,
                    Content = ChildContent,
                    Options = Options,
                    Target = Element,
                    Closing = false,
                    TaskCompletionSource = taskCompletionSource
                };
                var prevMenuOption = PopupService.SubMenuOptions.FirstOrDefault();
                if (prevMenuOption != null)
                {
                    await prevMenuOption.Close(prevMenuOption);
                }
                PopupService.SubMenuOptions.Add(subMenuOption);
                isOpened = true;
                await taskCompletionSource.Task;
                DeActivate();
            }
            else
            {
                backgroundColor = Options.HoverColor;
                textColor = Options.ActiveTextColor;
                Activate();
            }
            //opened = true;
        }

        internal async Task CloseAsync()
        {
            await subMenuOption.Close(subMenuOption);
            isOpened = false;
            isActive = false;
        }

        internal void CancelClose()
        {
            if (subMenuOption == null)
            {
                return;
            }
            subMenuOption.CancelClose = true;
        }

        protected void OnOut()
        {
            if (isActive || isOpened)
            {
                backgroundColor = Options.BackgroundColor;
                textColor = Options.TextColor;
                if (TopMenu.Mode == MenuMode.Horizontal)
                {
                    subMenuOption.Closing = true;
                    Task.Delay(50).ContinueWith(task =>
                    {
                        if (!subMenuOption.Closing)
                        {
                            return;
                        }
                        isOpened = false;
                        isActive = false;
                        if (subMenuOption.Close == null)
                        {
                            return;
                        }
                        InvokeAsync(() =>
                        {
                            subMenuOption.Close(subMenuOption);
                        });
                    });
                }
                else
                {
                    isActive = false;
                }
            }
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
