using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElXMentionSender : ElementComponentBase
    {
        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public IEnumerable<MentionOption> Options { get; set; }

        [Parameter]
        public string Prefix { get; set; } = "/";

        [Parameter]
        public IEnumerable<string> Prefixes { get; set; }

        [Parameter]
        public EventCallback<MentionOption> OnMentionSelect { get; set; }

        [Parameter]
        public EventCallback<string> OnSubmit { get; set; }

        [Parameter]
        public string Placeholder { get; set; } = "Type / for commands";

        [Parameter]
        public int Rows { get; set; } = 3;

        [Parameter]
        public bool Loading { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool Readonly { get; set; }

        [Parameter]
        public bool AllowEmpty { get; set; }

        [Parameter]
        public string SendButtonText { get; set; } = "Send";

        protected string MentionSenderClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-mention-sender", Cls)
            .AddIf(Loading, "is-loading")
            .ToString();

        protected bool IsSendDisabled => Disabled || Readonly || Loading || (!AllowEmpty && string.IsNullOrWhiteSpace(Value));

        private async Task OnValueChangedAsync(string value)
        {
            Value = value;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }
        }

        private async Task SendAsync(MouseEventArgs args)
        {
            if (!IsSendDisabled && OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync(Value);
            }
        }
    }
}
