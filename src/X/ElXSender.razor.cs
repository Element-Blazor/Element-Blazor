using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Element.X
{
    public partial class ElXSender : ElementComponentBase
    {
        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<string> OnSubmit { get; set; }

        [Parameter]
        public EventCallback OnStop { get; set; }

        [Parameter]
        public string Placeholder { get; set; } = "Ask anything";

        [Parameter]
        public int Rows { get; set; } = 3;

        [Parameter]
        public bool Loading { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool Readonly { get; set; }

        [Parameter]
        public bool SubmitOnEnter { get; set; } = true;

        [Parameter]
        public bool AllowEmpty { get; set; }

        [Parameter]
        public string SendButtonText { get; set; } = "Send";

        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter]
        public RenderFragment Prefix { get; set; }

        [Parameter]
        public RenderFragment Suffix { get; set; }

        [Parameter]
        public RenderFragment Footer { get; set; }

        [Parameter]
        public RenderFragment Attachments { get; set; }

        protected bool IsSendDisabled => Disabled || Readonly || Loading || (!AllowEmpty && string.IsNullOrWhiteSpace(Value));

        protected string SenderClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-sender", Cls)
            .AddIf(Loading, "is-loading")
            .AddIf(Disabled, "is-disabled")
            .ToString();

        private async Task OnInputAsync(ChangeEventArgs args)
        {
            Value = Convert.ToString(args.Value);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }

        private async Task OnKeyDownAsync(KeyboardEventArgs args)
        {
            if (!SubmitOnEnter || args.Key != "Enter" || args.ShiftKey)
            {
                return;
            }

            await SubmitAsync();
        }

        private async Task SendAsync(MouseEventArgs args)
        {
            await SubmitAsync();
        }

        private async Task SubmitAsync()
        {
            if (IsSendDisabled || !OnSubmit.HasDelegate)
            {
                return;
            }

            await OnSubmit.InvokeAsync(Value);
        }

        private async Task StopAsync(MouseEventArgs args)
        {
            if (OnStop.HasDelegate)
            {
                await OnStop.InvokeAsync();
            }
        }
    }
}
