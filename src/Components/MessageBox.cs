using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Element
{
    public class MessageBox
    {
        public MessageBox(DialogService dialogService)
        {
            DialogService = dialogService;
        }

        private DialogService DialogService { get; }

        public Task<MessageBoxResult> AlertAsync(string text)
        {
            return AlertAsync(new MessageBoxOption { Message = text });
        }

        public async Task<MessageBoxResult> AlertAsync(MessageBoxOption messageBoxOption)
        {
            messageBoxOption.ShowCancelButton = false;
            messageBoxOption.ShowConfirmButton = true;
            var dialogResult = await ShowAsync(messageBoxOption);
            return dialogResult.Result;
        }

        public Task<MessageBoxResult> ConfirmAsync(string text)
        {
            return ConfirmAsync(new MessageBoxOption { Message = text });
        }

        public async Task<MessageBoxResult> ConfirmAsync(MessageBoxOption messageBoxOption)
        {
            messageBoxOption.ShowCancelButton = true;
            messageBoxOption.ShowConfirmButton = true;
            var dialogResult = await ShowAsync(messageBoxOption);
            return dialogResult.Result;
        }

        public Task<MessageBoxPromptResult> PromptAsync(string message, string title = null)
        {
            return PromptAsync(new MessageBoxOption
            {
                Message = message,
                Title = title ?? "提示",
                ShowInput = true,
                ShowCancelButton = true
            });
        }

        public async Task<MessageBoxPromptResult> PromptAsync(MessageBoxOption messageBoxOption)
        {
            messageBoxOption.ShowInput = true;
            messageBoxOption.ShowCancelButton = true;
            messageBoxOption.ShowConfirmButton = true;
            return await ShowAsync(messageBoxOption);
        }

        private async Task<MessageBoxPromptResult> ShowAsync(MessageBoxOption messageBoxOption)
        {
            var option = CreateOption(messageBoxOption);
            if (messageBoxOption.ShowCancelButton)
            {
                option.Buttons.Add(CreateButtonRenderer(option, messageBoxOption, messageBoxOption.CancelButtonText, MessageBoxResult.Cancel, messageBoxOption.CancelButtonType, MessageBoxAction.Cancel));
            }
            if (messageBoxOption.ShowConfirmButton)
            {
                option.Buttons.Add(CreateButtonRenderer(option, messageBoxOption, messageBoxOption.ConfirmButtonText, MessageBoxResult.Ok, messageBoxOption.ConfirmButtonType, MessageBoxAction.Confirm));
            }

            DialogService.Dialogs.Add(option);
            var dialogResult = await option.TaskCompletionSource.Task;
            await Task.Delay(10);
            return dialogResult.Result as MessageBoxPromptResult ?? new MessageBoxPromptResult
            {
                Result = dialogResult.Result is MessageBoxResult result ? result : MessageBoxResult.Close,
                Value = messageBoxOption.InputValue
            };
        }

        private DialogOption CreateOption(MessageBoxOption messageBoxOption)
        {
            var taskCompletionSource = new TaskCompletionSource<DialogResult>();
            return new DialogOption
            {
                Title = messageBoxOption.Title,
                Content = typeof(MessageBoxContent),
                Parameters = new Dictionary<string, object>
                {
                    [nameof(MessageBoxContent.Option)] = messageBoxOption
                },
                IsDialog = false,
                IsModal = true,
                ShowClose = messageBoxOption.ShowClose,
                CloseOnClickModal = messageBoxOption.CloseOnClickModal,
                CloseOnPressEscape = messageBoxOption.CloseOnPressEscape,
                MessageBoxOption = messageBoxOption,
                TaskCompletionSource = taskCompletionSource
            };
        }

        private RenderFragment CreateButtonRenderer(DialogOption option, MessageBoxOption messageBoxOption, string text, MessageBoxResult result, ButtonType type, MessageBoxAction action)
        {
            return builder =>
            {
                builder.OpenComponent<ElButton>(0);
                builder.AddAttribute(1, nameof(ElButton.OnClick), EventCallback.Factory.Create<MouseEventArgs>(option.Instance, async e =>
                {
                    await CloseAsync(option, messageBoxOption, result, action);
                }));
                builder.AddAttribute(2, nameof(ElButton.Type), type);
                builder.AddAttribute(3, nameof(ElButton.ChildContent), new RenderFragment(child => child.AddContent(0, text)));
                builder.AddAttribute(4, nameof(ElButton.Size), ButtonSize.Small);
                builder.CloseComponent();
            };
        }

        internal static async Task CloseAsync(DialogOption option, MessageBoxOption messageBoxOption, MessageBoxResult result, MessageBoxAction action)
        {
            if (messageBoxOption.BeforeClose != null && !await messageBoxOption.BeforeClose(action))
            {
                return;
            }

            await option.Instance.CloseDialogAsync(option, new DialogResult
            {
                Result = new MessageBoxPromptResult
                {
                    Result = result,
                    Value = messageBoxOption.InputValue
                }
            });
        }
    }
}