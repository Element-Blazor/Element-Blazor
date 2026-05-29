using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Element
{
    public class MessageBoxOption
    {
        public string Title { get; set; } = "提示";

        public string Message { get; set; }

        public RenderFragment MessageContent { get; set; }

        public string ConfirmButtonText { get; set; } = "确定";

        public string CancelButtonText { get; set; } = "取消";

        public ButtonType ConfirmButtonType { get; set; } = ButtonType.Primary;

        public ButtonType CancelButtonType { get; set; } = ButtonType.Default;

        public bool ShowCancelButton { get; set; }

        public bool ShowConfirmButton { get; set; } = true;

        public bool ShowClose { get; set; } = true;

        public bool DistinguishCancelAndClose { get; set; }

        public bool CloseOnClickModal { get; set; } = true;

        public bool CloseOnPressEscape { get; set; } = true;

        public bool ShowInput { get; set; }

        public string InputValue { get; set; }

        public string InputPlaceholder { get; set; }

        public Func<MessageBoxAction, Task<bool>> BeforeClose { get; set; }
    }
}
