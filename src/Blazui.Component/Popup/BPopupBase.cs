using Blazui.Component.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Popup
{
    public class BPopupBase : ComponentBase
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private DialogService DialogService { get; set; }

        [Inject]
        private MessageService MessageService { get; set; }

        [Inject]
        private LoadingService LoadingService { get; set; }

        [Inject]
        private PopupService PopupService { get; set; }

        protected int ShadowCount { get; set; }
        [Inject]
        private Document Document { get; set; }
        public Action OnDispose { get; internal set; }
        public static int ZIndex { get; internal set; } = 2000;
        protected List<MessageInfo> Messages { get; set; } = new List<MessageInfo>();
        private List<MessageInfo> RemovingMessages = new List<MessageInfo>();

        protected List<LoadingOption> LoadingOptions = new List<LoadingOption>();
        internal protected List<DialogOption> DialogOptions = new List<DialogOption>();
        internal protected List<DateTimePickerOption> DateTimePickerOptions = new List<DateTimePickerOption>();
        internal protected List<DropDownOption> DropDownOptions = new List<DropDownOption>();

        internal async Task CloseDialogAsync(DialogOption option, DialogResult result)
        {
            var messageContent = option.Element;
            var dom = messageContent.Dom(JSRuntime);
            var top = await dom.GetOffsetTopAsync();
            var left = await dom.GetOffsetLeftAsync();
            var style = messageContent.Dom(JSRuntime).Style;
            await style.SetAsync("left", $"{left}px");
            await style.SetAsync("top", $"{top}px");
            await style.SetAsync("position", "absolute");
            await style.SetTransitionAsync("top 0.3s,opacity 0.3s");
            await Task.Delay(100);
            await style.SetAsync("top", $"{top - 10}px");
            await style.SetAsync("opacity", $"0");
            if (--ShadowCount <= 0)
            {
                await option.ShadowElement.Dom(JSRuntime).Style.SetAsync("opacity", "0");
            }
            await Task.Delay(300);
            await style.ClearAsync("left");
            await style.ClearAsync("top");
            await style.ClearAsync("position");
            option.TaskCompletionSource.SetResult(result);
            DialogService.Dialogs.Remove(option);
        }

        protected void SelectDay(DateTimePickerOption option, DateTime day)
        {
            option.CurrentMonth = day;
            option.TaskComplietionSource.SetResult(day);
            CloseDateTimePicker(option);
        }

        protected override void OnInitialized()
        {
            MessageService.Messages.CollectionChanged -= Messages_CollectionChanged;
            MessageService.Messages = new ObservableCollection<MessageInfo>();
            MessageService.Messages.CollectionChanged += Messages_CollectionChanged;

            LoadingService.LoadingOptions.CollectionChanged -= LoadingOptions_CollectionChanged;
            LoadingService.LoadingOptions = new ObservableCollection<LoadingOption>();
            LoadingService.LoadingOptions.CollectionChanged += LoadingOptions_CollectionChanged;

            DialogService.Dialogs.CollectionChanged -= Dialogs_CollectionChanged;
            DialogService.Dialogs = new ObservableCollection<DialogOption>();
            DialogService.Dialogs.CollectionChanged += Dialogs_CollectionChanged;

            PopupService.DateTimePickerOptions.CollectionChanged -= DateTimePickerOptions_CollectionChanged;
            PopupService.DateTimePickerOptions = new ObservableCollection<DateTimePickerOption>();
            PopupService.DateTimePickerOptions.CollectionChanged += DateTimePickerOptions_CollectionChanged;

            PopupService.DropDownOptions.CollectionChanged -= DropDownOptions_CollectionChanged;
            PopupService.DropDownOptions = new ObservableCollection<DropDownOption>();
            PopupService.DropDownOptions.CollectionChanged += DropDownOptions_CollectionChanged;
        }

        private void DropDownOptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var option = e.NewItems.OfType<DropDownOption>().FirstOrDefault();
                option.IsNew = true;
                option.Instance = this;
                DropDownOptions.Add(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var option = e.OldItems.OfType<DropDownOption>().FirstOrDefault();
                DropDownOptions.Remove(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }

        private void DateTimePickerOptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var option = e.NewItems.OfType<DateTimePickerOption>().FirstOrDefault();
                option.IsNew = true;
                option.Instance = this;
                DateTimePickerOptions.Add(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var option = e.OldItems.OfType<DateTimePickerOption>().FirstOrDefault();
                DateTimePickerOptions.Remove(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }

        private void Dialogs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var option = e.NewItems.OfType<DialogOption>().FirstOrDefault();
                option.IsNew = true;
                option.Instance = this;
                DialogOptions.Add(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var option = e.OldItems.OfType<DialogOption>().FirstOrDefault();
                DialogOptions.Remove(option);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }

        protected void GoPrevYear(DateTimePickerOption option)
        {
            if (option.ShowYear > 0 && option.ShowYear > 1900)
            {
                option.ShowYear -= 10;
                return;
            }
            if (option.CurrentMonth.Year < 1900)
            {
                return;
            }
            option.CurrentMonth = option.CurrentMonth.AddYears(-1);
        }


        protected void GoNextYear(DateTimePickerOption option)
        {
            if (option.ShowYear > 0)
            {
                option.ShowYear += 10;
                return;
            }
            option.CurrentMonth = option.CurrentMonth.AddYears(1);
        }
        protected void OpenMonth(DateTimePickerOption option, int month)
        {
            option.ShowYear = option.ShowMonth = 0;
            option.CurrentMonth = new DateTime(option.CurrentMonth.Year, month, 1);
        }

        private void LoadingOptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var loadingOption = e.NewItems.OfType<LoadingOption>().FirstOrDefault();
                loadingOption.IsNew = true;
                LoadingOptions.Add(loadingOption);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var loadingOption = e.OldItems.OfType<LoadingOption>().FirstOrDefault();
                LoadingOptions.Remove(loadingOption);
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }

        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var message = e.NewItems.OfType<MessageInfo>().FirstOrDefault();
                message.IsNew = true;
                lock (Messages)
                {
                    message.Index = Messages.Count;
                    var movingDistance = 30;
                    if (message.Index > 0)
                    {
                        var prevMessage = Messages.LastOrDefault();
                        message.BeginTop = prevMessage.EndTop + 30;
                        message.EndTop = message.BeginTop + movingDistance;
                    }
                    else
                    {
                        message.BeginTop = 0;
                        message.EndTop = movingDistance;
                    }
                    Messages.Add(message);
                }
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems.OfType<MessageInfo>())
                {
                    RemovingMessages.Add(item);
                }
                if (Messages.Count == RemovingMessages.Count && RemovingMessages.All(x => Messages.Any(y => x == y)))
                {
                    lock (Messages)
                    {
                        foreach (var item in RemovingMessages)
                        {
                            Messages.Remove(item);
                        }
                    }
                    RemovingMessages.Clear();
                    InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                }
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            _ = RenderMessageAsync();
            _ = RenderLoadingAsync();
            _ = RenderDialogAsync();
            _ = RenderDateTimePickerAsync();
            _ = RenderDropDownAsync();
        }

        private async Task RenderDropDownAsync()
        {
            var option = DropDownOptions.FirstOrDefault(x => x.IsNew);
            if (option == null)
            {
                return;
            }
            option.IsNew = false;
            var targetEl = option.Target.Dom(JSRuntime);
            var rect = await targetEl.GetBoundingClientRectAsync();
            var top = await targetEl.GetTopRelativeBodyAsync();
            option.Left = rect.Left;
            option.Top = top + rect.Height;
            option.Width = rect.Width;
            option.IsShow = true;
            option.ShowStatus = AnimationStatus.Begin;
            StateHasChanged();
            await Task.Delay(10);
            option.ShowStatus = AnimationStatus.End;
            StateHasChanged();
        }

        internal async Task CloseDropDownAsync(DropDownOption option)
        {
            option.IsShow = false;
            option.HideStatus = AnimationStatus.Begin;
            StateHasChanged();
            await Task.Delay(10);
            option.HideStatus = AnimationStatus.End;
            StateHasChanged();
            await Task.Delay(200);
            PopupService.DropDownOptions.Remove(option);
            option.Refresh();
        }

        async Task RenderDateTimePickerAsync()
        {
            var option = DateTimePickerOptions.FirstOrDefault(x => x.IsNew);
            if (option == null)
            {
                return;
            }
            option.IsNew = false;
            var targetEl = option.Target.Dom(JSRuntime);
            var rect = await targetEl.GetBoundingClientRectAsync();
            var top = await targetEl.GetTopRelativeBodyAsync();
            option.Left = rect.Left;
            option.Top = top + rect.Height;
            var style = option.Element.Dom(JSRuntime).Style;
            await style.SetAsync("left", $"{rect.Left}px");
            await style.SetAsync("top", $"{option.Top + 10}px");
            await style.ClearAsync("display");
            await Task.Delay(10);
            await style.SetAsync("top", $"{option.Top}px");
            await style.SetAsync("opacity", $"1");
        }

        async Task RenderMessageAsync()
        {
            var newMessage = Messages.FirstOrDefault(x => x.IsNew);
            if (newMessage == null)
            {
                return;
            }
            newMessage.IsNew = false;
            var messageContent = newMessage.Element;
            var style = messageContent.Dom(JSRuntime).Style;
            await Task.Delay(50);
            await style.SetAsync("top", $"{newMessage.EndTop}px");
            await style.SetAsync("opacity", $"1");
            await Task.Delay(newMessage.Duration + 500);
            await style.SetAsync("top", $"{newMessage.BeginTop}px");
            await style.SetAsync("opacity", $"0");
            await Task.Delay(500);
            MessageService.Messages.Remove(newMessage);
        }

        protected void CloseDateTimePicker(DateTimePickerOption option)
        {
            PopupService.DateTimePickerOptions.Remove(option);
        }
        async Task RenderDialogAsync()
        {
            var option = DialogOptions.FirstOrDefault(x => x.IsNew);
            if (option == null)
            {
                return;
            }
            option.IsNew = false;
            var messageContent = option.Element;
            var dom = messageContent.Dom(JSRuntime);
            var top = await dom.GetOffsetTopAsync();
            var left = await dom.GetOffsetLeftAsync();
            var style = messageContent.Dom(JSRuntime).Style;
            await style.SetAsync("position", "absolute");
            if (option.IsDialog)
            {
                await style.ClearAsync("margin-top");
                await style.SetAsync("position", $"absolute");
            }
            await style.SetAsync("left", $"{left}px");
            await style.SetAsync("top", $"{top - 10}px");
            await style.SetTransitionAsync("top 0.3s,opacity 0.3s");
            await Task.Delay(100);
            await style.SetAsync("opacity", "1");
            await style.SetAsync("top", $"{top}px");
            if (ShadowCount++ <= 0)
            {
                await option.ShadowElement.Dom(JSRuntime).Style.SetAsync("opacity", "0.5");
            }
            await Task.Delay(500);
            if (!option.IsDialog)
            {
                await style.ClearAsync("position");
                await style.ClearAsync("top");
                await style.ClearAsync("left");
            }
            await style.ClearAsync("transition");
        }

        async Task ShowFullScreenLoadingAsync(LoadingOption option)
        {
            await Document.AppendAsync(option.Element);
            await Document.DisableScrollAsync();
            var style = option.Element.Dom(JSRuntime).Style;
            await style.SetAsync("display", "none");
            await style.SetAsync("width", "100%");
            await style.SetAsync("height", "100%");
            await style.SetAsync("position", "fixed");
            await style.SetAsync("top", "0");
            await style.SetAsync("bottom", "0");
            await style.SetAsync("display", "block");
        }

        async Task RenderLoadingAsync()
        {
            var option = LoadingOptions.FirstOrDefault(x => x.IsNew);
            if (option == null)
            {
                return;
            }
            option.IsNew = false;
            if (string.IsNullOrWhiteSpace(option.Target.Id))
            {
                await ShowFullScreenLoadingAsync(option);
                return;
            }
            await option.Target.Dom(JSRuntime).AppendAsync(option.Element);
            await option.Element.Dom(JSRuntime).Style.SetAsync("display", "block");
        }
    }
}
