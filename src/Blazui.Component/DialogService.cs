using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class DialogService
    {
        internal ObservableCollection<DialogOption> Dialogs = new ObservableCollection<DialogOption>();

        public async Task<DialogResult<TResult>> ShowDialogAsync<TComponent, TResult>(string title)
            where TComponent : ComponentBase
        {
            var taskCompletionSource = new TaskCompletionSource<DialogResult>();
            var option = new DialogOption()
            {
                Content = typeof(TComponent),
                IsDialog = true,
                Title = title,
                TaskCompletionSource = taskCompletionSource
            };
            ShowDialog(option);
            var dialogResult = await taskCompletionSource.Task;
            return new DialogResult<TResult>()
            {
                Result = (TResult)dialogResult.Result
            };
        }
        public async Task<DialogResult> ShowDialogAsync<TComponent>(string title)
            where TComponent : ComponentBase
        {
            var taskCompletionSource = new TaskCompletionSource<DialogResult>();
            var option = new DialogOption()
            {
                Content = typeof(TComponent),
                IsDialog = true,
                Title = title,
                TaskCompletionSource = taskCompletionSource
            };
            ShowDialog(option);
            return await taskCompletionSource.Task;
        }
        public void ShowDialog(DialogOption option)
        {
            Dialogs.Add(option);
        }
    }
}
