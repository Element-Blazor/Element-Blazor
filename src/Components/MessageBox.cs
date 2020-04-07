
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class MessageBox
    {
        public MessageBox(DialogService dialogService)
        {
            DialogService = dialogService;
        }
        DialogService DialogService { get; set; }
        public async Task<MessageBoxResult> AlertAsync(string text)
        {
            var option = CreateOption(text);
            var okRenderFragment = CreateButtonRenderer(option, "确定", MessageBoxResult.Ok, ButtonType.Primary);
            option.Buttons.Add(okRenderFragment);
            DialogService.Dialogs.Add(option);
            var dialogResult = await option.TaskCompletionSource.Task;
            await Task.Delay(10);
            return (MessageBoxResult)dialogResult.Result;
        }

        private DialogOption CreateOption(string text)
        {
            var taskCompletionSource = new TaskCompletionSource<DialogResult>();
            return new DialogOption()
            {
                Title = "提示",
                Content = text,
                IsDialog = false,
                TaskCompletionSource = taskCompletionSource
            };
        }

        private RenderFragment CreateButtonRenderer(DialogOption option, string text, MessageBoxResult result, ButtonType type)
        {
            return builder =>
            {
                builder.OpenComponent<BButton>(0);
                builder.AddAttribute(1, nameof(BButton.OnClick), EventCallback.Factory.Create(option.Instance, async (MouseEventArgs e) =>
                {
                    await option.Instance.CloseDialogAsync(option, new DialogResult()
                    {
                        Result = result
                    });
                }));
                builder.AddAttribute(2, "Type", type);
                builder.AddAttribute(3, "ChildContent", new RenderFragment(__builder2 => __builder2.AddMarkupContent(4, text)));
                builder.AddAttribute(5, "Size", ButtonSize.Small);
                builder.CloseComponent();
            };
        }

        public async Task<MessageBoxResult> ConfirmAsync(string text)
        {
            var option = CreateOption(text);
            var cancelRenderFragment = CreateButtonRenderer(option, "取消", MessageBoxResult.Cancel, ButtonType.Default);
            var okRenderFragment = CreateButtonRenderer(option, "确定", MessageBoxResult.Ok, ButtonType.Primary);
            option.Buttons.Add(cancelRenderFragment);
            option.Buttons.Add(okRenderFragment);
            DialogService.Dialogs.Add(option);
            var dialogResult = await option.TaskCompletionSource.Task;
            await Task.Delay(10);
            return (MessageBoxResult)dialogResult.Result;
        }
    }
}
