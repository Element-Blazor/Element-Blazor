using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElSubMenu : ElMenuContainer, IMenuItem
    {
        internal SemaphoreSlim SemaphoreSlim { get; private set; } = new SemaphoreSlim(1, 1);
        protected ElementReference Element { get; set; }
        [Inject]
        PopupService PopupService { get; set; }

        [Parameter]
        public string Index { get; set; }

        internal string EffectiveIndex => string.IsNullOrWhiteSpace(Index) ? Label : Index;

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
        public ElSubMenu ParentMenu { get; set; }

        [CascadingParameter]
        public ElMenuContainer Menu { get; set; }

        [CascadingParameter]
        public MenuOptions Options { get; set; }

        protected string textColor;
        protected string backgroundColor;
        protected string borderColor;

        protected bool isActive = false;
        private SubMenuOption subMenuOption;

        protected bool IsVertical
        {
            get
            {
                return Options != null && Options.Mode == MenuMode.Vertical && !Options.Collapse;
            }
        }

        protected bool IsOpened { get; set; } = false;

        private bool UsesPopup => TopMenu?.Mode == MenuMode.Horizontal || Options?.Collapse == true;

        protected string SubMenuClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-submenu", Cls)
            .AddIf(isActive, "is-active")
            .AddIf(IsOpened, "is-opened")
            .AddIf(Disabled || Options.Disabled, "is-disabled")
            .ToString();

        protected string EffectiveTitle => EffectiveCollapse ? Label : null;

        protected bool EffectiveCollapse => Options?.Collapse == true;

        public void Activate()
        {
            if (Disabled || Options.Disabled)
            {
                return;
            }
            isActive = true;
            IsOpened = true;
        }
        public void DeActivate()
        {
            isActive = false;
            if (UsesPopup)
            {
                IsOpened = false;
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
            Menu?.AddMenuItem(this);
            if (!TopMenu.CanCollapse)
            {
                IsOpened = true;
            }
            base.OnInitialized();
        }

        protected async Task OnOverAsync()
        {
            if (Disabled || Options.Disabled)
            {
                return;
            }
            if (UsesPopup)
            {
                await SemaphoreSlim.WaitAsync();
                try
                {
                    if (IsOpened)
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
                        Target = Element,
                        PopperClass = Options.PopperClass,
                        PopperStyle = Options.PopperStyle
                    };
                    var taskCompletionSource = new TaskCompletionSource<int>();
                    subMenuOption.TaskCompletionSource = taskCompletionSource;
                    while (PopupService.SubMenuOptions.Any())
                    {
                        await Task.Delay(50);
                    }
                    PopupService.SubMenuOptions.Add(subMenuOption);
                    IsOpened = true;
                    await TopMenu.NotifyOpenAsync(this);
                }
                finally
                {
                    SemaphoreSlim.Release();
                }
                await subMenuOption.TaskCompletionSource.Task;
                borderColor = "transparent";

                IsOpened = false;
                await OnOutAsync();
            }
            else
            {
                backgroundColor = Options.HoverColor;
                textColor = Options.ActiveTextColor;
                isActive = true;
                if (!IsOpened)
                {
                    IsOpened = true;
                    await TopMenu.NotifyOpenAsync(this);
                }
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
            subMenuOption?.Instance?.KeepShowSubMenu(subMenuOption);
        }

        internal async Task CloseAsync()
        {
            if (subMenuOption?.Close != null)
            {
                await subMenuOption.Close(subMenuOption);
            }
            IsOpened = false;
            isActive = false;
            await TopMenu.NotifyCloseAsync(this);
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
            if (Disabled || Options.Disabled)
            {
                return;
            }
            if (UsesPopup && Options.MenuTrigger == MenuTrigger.Click)
            {
                return;
            }
            if (isActive || IsOpened)
            {
                backgroundColor = Options.BackgroundColor;
                textColor = Options.TextColor;
                if (UsesPopup && subMenuOption?.IsShow == true)
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
                             IsOpened = false;
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
                if (IsOpened)
                {
                    await TopMenu.NotifyCloseAsync(this);
                }
                isActive = false;
                IsOpened = false;
            }
        }
        }

        protected async Task OnClickAsync()
        {
            if (Disabled || Options.Disabled)
            {
                return;
            }
            if (IsVertical && TopMenu.CanCollapse)
            {
                IsOpened = !IsOpened;
                if (IsOpened)
                {
                    await TopMenu.NotifyOpenAsync(this);
                }
                else
                {
                    await TopMenu.NotifyCloseAsync(this);
                }
            }
            else if (UsesPopup && Options.MenuTrigger == MenuTrigger.Click)
            {
                if (IsOpened)
                {
                    await CloseAsync();
                }
                else
                {
                    await OnOverAsync();
                }
            }
        }

        protected async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (e.Key != "Enter" && e.Key != " ")
            {
                return;
            }
            await OnClickAsync();
        }

        internal Task ToggleByKeyboardAsync()
        {
            if (Disabled || Options.Disabled)
            {
                return Task.CompletedTask;
            }
            if (UsesPopup)
            {
                return OnOverAsync();
            }
            if (IsVertical)
            {
                return OnClickAsync();
            }

            return OnOverAsync();
        }

        internal async Task CloseFromMenuAsync(bool notify)
        {
            if (!IsOpened)
            {
                return;
            }

            IsOpened = false;
            isActive = false;
            if (notify)
            {
                await TopMenu.NotifyCloseAsync(this);
            }
        }

        private static string NormalizeIcon(string icon)
        {
            if (string.IsNullOrWhiteSpace(icon))
            {
                return null;
            }

            return icon.StartsWith("el-icon-") ? icon : $"el-icon-{icon}";
        }

        public override void Dispose()
        {
            base.Dispose();
            Menu?.RemoveMenuItem(this);
        }
        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
