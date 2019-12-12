using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blazui.Component.NavMenu
{
    public class BSubMenuBase : BMenuContainer, IMenuItem
    {
        internal SemaphoreSlim SemaphoreSlim { get; private set; } = new SemaphoreSlim(1, 1);
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

        [Parameter]
        public string Icon { get; set; }
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
            isOpened = true;
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
            _ = OnOutAsync();
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
                await SemaphoreSlim.WaitAsync();
                try
                {
                    if (isOpened)
                    {
                        try
                        {
                            if (subMenuOption != null
                                && subMenuOption.ClosingTask != null
                                && subMenuOption.ClosingTask.Status != TaskStatus.RanToCompletion
                                && subMenuOption.ClosingTask.Status != TaskStatus.Canceled)
                            {
                                subMenuOption.ClosingTaskCancellationTokenSource.Cancel();
                            }
                        }
                        catch (ObjectDisposedException)
                        {

                        }
                        return;
                    }
                    subMenuOption = new SubMenuOption()
                    {
                        SubMenu = this,
                        Content = ChildContent,
                        Options = Options,
                        Target = Element
                    };
                    var taskCompletionSource = new TaskCompletionSource<int>();
                    subMenuOption.TaskCompletionSource = taskCompletionSource;
                    while (PopupService.SubMenuOptions.Any())
                    {
                        await Task.Delay(50);
                    }
                    PopupService.SubMenuOptions.Add(subMenuOption);
                    isOpened = true;
                }
                finally
                {
                    SemaphoreSlim.Release();
                }
                await subMenuOption.TaskCompletionSource.Task;
                borderColor = "transparent";

                isOpened = false;
                await OnOutAsync();
            }
            else
            {
                backgroundColor = Options.HoverColor;
                textColor = Options.ActiveTextColor;
                isActive = true;
            }
        }

        private void SubMenuOptions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (PopupService.SubMenuOptions.Any())
            {
                return;
            }
            PopupService.SubMenuOptions.CollectionChanged -= SubMenuOptions_CollectionChanged;
            PopupService.SubMenuOptions.Add(subMenuOption);
        }

        internal void KeepSubMenuOpen()
        {
            subMenuOption.Instance.KeepShowSubMenu(subMenuOption);
        }

        internal async Task CloseAsync()
        {
            await subMenuOption.Close(subMenuOption);
            isOpened = false;
            isActive = false;
        }

        void DisposeTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            if (cancellationTokenSource == null)
            {
                return;
            }
            try
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }
                cancellationTokenSource.Dispose();
            }
            catch (ObjectDisposedException)
            {

            }
        }

        protected async Task OnOutAsync()
        {
            if (isActive || isOpened)
            {
                backgroundColor = Options.BackgroundColor;
                textColor = Options.TextColor;
                if (TopMenu.Mode == MenuMode.Horizontal && subMenuOption.IsShow)
                {
                    subMenuOption.ClosingTaskCancellationTokenSource = new System.Threading.CancellationTokenSource();
                    var option = subMenuOption;
                    var closingTask = Task.Delay(50, subMenuOption.ClosingTaskCancellationTokenSource.Token).ContinueWith(async task =>
                         {
                             if (task.IsCanceled)
                             {
                                 DisposeTokenSource(option.ClosingTaskCancellationTokenSource);
                                 return;
                             }
                             DisposeTokenSource(option.ClosingTaskCancellationTokenSource);
                             isOpened = false;
                             isActive = false;
                             if (option.Close == null)
                             {
                                 return;
                             }
                             await InvokeAsync(async () =>
                             {
                                 await option.Close(option);
                             });
                         });
                    subMenuOption.ClosingTask = await closingTask;
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
        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
