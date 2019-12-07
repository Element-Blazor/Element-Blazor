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
            return await ShowDialogAsync<TComponent, TResult>(title, 0);
        }

        public async Task CloseDialogAsync<TComponent, TResult>(TComponent instance, TResult result)
            where TComponent : ComponentBase
        {
            var dialog = Dialogs.FirstOrDefault(x => typeof(TComponent).IsAssignableFrom((Type)x.Content));
            if (dialog == null)
            {
                return;
            }
            await dialog.Instance.CloseDialogAsync(dialog, new DialogResult()
            {
                Result = result
            });
        }

        public async Task<DialogResult<TResult>> ShowDialogAsync<TResult>(RenderFragment render, string title, float width)
        {
            return await ShowDialogAsync<TResult>((object)render, title, width);
        }

        public async Task<DialogResult<TResult>> ShowDialogAsync<TComponent, TResult>(string title, float width)
            where TComponent : ComponentBase
        {
            return await ShowDialogAsync<TResult>(typeof(TComponent), title, width);
        }
        public async Task<DialogResult<TResult>> ShowDialogAsync<TResult>(object typeOrRender, string title)
        {
            return await ShowDialogAsync<TResult>(typeOrRender, title, 0);
        }
        public async Task<DialogResult<TResult>> ShowDialogAsync<TResult>(object typeOrRender, string title, float width)
        {
            var taskCompletionSource = new TaskCompletionSource<DialogResult>();
            var option = new DialogOption()
            {
                Content = typeOrRender,
                IsDialog = true,
                Width = width,
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
            return await ShowDialogAsync<TComponent>(title, 0);
        }
        public async Task<DialogResult> ShowDialogAsync<TComponent>(string title, float width)
            where TComponent : ComponentBase
        {
            var taskCompletionSource = new TaskCompletionSource<DialogResult>();
            var option = new DialogOption()
            {
                Content = typeof(TComponent),
                IsDialog = true,
                Title = title,
                Width = width,
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
