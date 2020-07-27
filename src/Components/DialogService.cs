using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class DialogService
    {
        internal static ObservableCollection<DialogOption> Dialogs = new ObservableCollection<DialogOption>();

        /// <summary>
        /// 关闭对话框
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="instance"></param>
        /// <param name="result"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="title">标题</param>
        /// <param name="parameters">组件所需要的参数</param>
        public async Task<DialogResult<TResult>> ShowDialogAsync<TComponent, TResult>(string title, IDictionary<string, object> parameters)
            where TComponent : ComponentBase
        {
            return await ShowDialogAsync<TComponent, TResult>(title, 0, parameters);
        }


        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <typeparam name="TComponent">要显示的组件</typeparam>
        /// <typeparam name="TResult">容器的返回值</typeparam>
        /// <param name="title">标题</param>
        /// <param name="width">宽度</param>
        /// <returns></returns>
        public async Task<DialogResult<TResult>> ShowDialogAsync<TComponent, TResult>(string title, float width)
            where TComponent : ComponentBase
        {
            return await ShowDialogAsync<TResult>(typeof(TComponent), title, width, new Dictionary<string, object>());
        }

        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <typeparam name="TComponent">要显示的组件</typeparam>
        /// <typeparam name="TResult">容器的返回值</typeparam>
        /// <param name="title">标题</param>
        /// <param name="width">宽度</param>
        /// <param name="parameters">窗口组件所需要的参数</param>
        /// <returns></returns>
        public async Task<DialogResult<TResult>> ShowDialogAsync<TComponent, TResult>(string title, float width, IDictionary<string, object> parameters)
            where TComponent : ComponentBase
        {
            return await ShowDialogAsync<TResult>(typeof(TComponent), title, width, parameters);
        }

        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="type">要显示的内容，可以是一个组件的 <seealso cref="Type"/></param>
        /// <param name="title">标题</param>
        /// <returns></returns>
        public async Task<DialogResult<TResult>> ShowDialogAsync<TResult>(object type, string title)
        {
            return await ShowDialogAsync<TResult>(type, title, 0, new Dictionary<string, object>());
        }


        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="type">要显示的内容，可以是一个组件的 <seealso cref="Type"/></param>
        /// <param name="title">标题</param>
        /// <param name="width"></param>
        /// <returns></returns>
        public async Task<DialogResult<TResult>> ShowDialogAsync<TResult>(object type, string title, float width)
        {
            return await ShowDialogAsync<TResult>(type, title, width, new Dictionary<string, object>());
        }

        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="type">要显示的内容，可以是一个组件的 <seealso cref="Type"/></param>
        /// <param name="title">标题</param>
        /// <param name="width">宽度</param>
        /// <param name="parameters">显示该组件所需要的参数</param>
        /// <returns></returns>
        public async Task<DialogResult<TResult>> ShowDialogAsync<TResult>(object type, string title, float width, IDictionary<string, object> parameters)
        {
            var taskCompletionSource = new TaskCompletionSource<DialogResult>();
            var option = new DialogOption()
            {
                Content = type,
                IsDialog = true,
                Width = width,
                Title = title,
                Parameters = parameters,
                TaskCompletionSource = taskCompletionSource
            };
            ShowDialog(option);
            var dialogResult = await taskCompletionSource.Task;
            return new DialogResult<TResult>()
            {
                Result = (TResult)dialogResult.Result
            };
        }


        /// <summary>
        /// 显示一个非模态的层到指定位置
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="type">要显示的内容，可以是一个组件的 <seealso cref="Type"/></param>
        /// <param name="width">宽度</param>
        /// <param name="parameters">显示该组件所需要的参数</param>
        /// <param name="point"></param>
        /// <returns></returns>
        public async Task<DialogResult<TResult>> ShowLayerAsync<TResult>(object type, float width, IDictionary<string, object> parameters, PointF point)
        {
            var taskCompletionSource = new TaskCompletionSource<DialogResult>();
            var option = new DialogOption()
            {
                Content = type,
                IsDialog = false,
                IsModal = false,
                Width = width,
                Parameters = parameters,
                TaskCompletionSource = taskCompletionSource,
                Point = point
            };
            ShowDialog(option);
            var dialogResult = await taskCompletionSource.Task;
            return new DialogResult<TResult>()
            {
                Result = (TResult)dialogResult.Result
            };
        }

        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="title">标题</param>
        /// <returns></returns>
        public async Task<DialogResult> ShowDialogAsync<TComponent>(string title)
            where TComponent : ComponentBase
        {
            return await ShowDialogAsync<TComponent>(title, 0, new Dictionary<string, object>());
        }
        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="title">标题</param>
        /// <param name="parameters">显示该组件所需要的参数</param>
        /// <returns></returns>
        public async Task<DialogResult> ShowDialogAsync<TComponent>(string title, IDictionary<string, object> parameters)
            where TComponent : ComponentBase
        {
            return await ShowDialogAsync<TComponent>(title, 0, parameters);
        }

        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="title">标题</param>
        /// <param name="width">宽度</param>
        /// <param name="parameters">显示该组件所需要的参数</param>
        /// <returns></returns>
        public async Task<DialogResult> ShowDialogAsync<TComponent>(string title, float width, IDictionary<string, object> parameters)
            where TComponent : ComponentBase
        {
            var taskCompletionSource = new TaskCompletionSource<DialogResult>();
            var option = new DialogOption()
            {
                Content = typeof(TComponent),
                IsDialog = true,
                Title = title,
                Width = width,
                Parameters = parameters,
                TaskCompletionSource = taskCompletionSource,

            };
            ShowDialog(option);
            return await taskCompletionSource.Task;
        }
        /// <summary>
        /// 显示一个全屏窗口
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="title">标题</param>
        /// <param name="parameters">显示该组件所需要的参数</param>
        /// <returns></returns>
        public async Task<DialogResult> ShowFullScreenDialogAsync<TComponent>(string title, IDictionary<string, object> parameters)
            where TComponent : ComponentBase
        {
            var taskCompletionSource = new TaskCompletionSource<DialogResult>();
            var option = new DialogOption()
            {
                Content = typeof(TComponent),
                IsDialog = true,
                Title = title,
                Width = 0,
                FullScreen = true,
                Parameters = parameters,
                TaskCompletionSource = taskCompletionSource,

            };
            ShowDialog(option);
            return await taskCompletionSource.Task;
        }
        /// <summary>
        /// 显示一个窗口
        /// </summary>
        /// <param name="option"></param>
        public void ShowDialog(DialogOption option)
        {
            if (option.Parameters == null)
            {
                option.Parameters = new Dictionary<string, object>();
            }
            option.Parameters.Add("Dialog", option);
            Dialogs.Add(option);
        }
    }
}
