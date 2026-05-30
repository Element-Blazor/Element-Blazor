using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Element.X
{
    public partial class ElXSender : ElementComponentBase
    {
        private static long senderIdSeed;
        private readonly string generatedInputId = $"el-x-sender-input-{System.Threading.Interlocked.Increment(ref senderIdSeed)}";
        private ElementReference textareaElement;

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<string> OnSubmit { get; set; }

        [Parameter]
        public EventCallback OnStop { get; set; }

        [Parameter]
        public EventCallback<string> OnClear { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

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
        public bool SubmitOnCtrlEnter { get; set; }

        [Parameter]
        public bool SubmitOnMetaEnter { get; set; }

        [Parameter]
        public bool ClearOnSubmit { get; set; }

        [Parameter]
        public bool ClearOnEscape { get; set; }

        [Parameter]
        public bool ShowClearButton { get; set; }

        [Parameter]
        public bool AllowEmpty { get; set; }

        [Parameter]
        public string SendButtonText { get; set; } = "Send";

        [Parameter]
        public string StopButtonText { get; set; } = "Stop";

        [Parameter]
        public string ClearButtonText { get; set; } = "Clear";

        [Parameter]
        public string AriaLabel { get; set; } = "Message input";

        [Parameter]
        public string AriaDescribedBy { get; set; }

        [Parameter]
        public bool AriaInvalid { get; set; }

        [Parameter]
        public string InputStyle { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public int? Maxlength { get; set; }

        [Parameter]
        public string Autocomplete { get; set; } = "off";

        [Parameter]
        public bool Autofocus { get; set; }

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

        protected string InputId => string.IsNullOrWhiteSpace(Id) ? generatedInputId : Id;

        protected string AriaInvalidValue => AriaInvalid ? "true" : "false";

        protected string EffectiveAriaDescribedBy => AriaDescribedBy;

        protected bool IsSendDisabled => Disabled || Readonly || Loading || (!AllowEmpty && string.IsNullOrWhiteSpace(Value));

        protected bool IsClearDisabled => Disabled || Readonly || string.IsNullOrEmpty(Value);

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
            if (OnKeyDown.HasDelegate)
            {
                await OnKeyDown.InvokeAsync(args);
            }

            if (args.Key == "Escape" && ClearOnEscape)
            {
                await ClearValueAsync();
                return;
            }

            if (!ShouldSubmit(args))
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

            var submittedValue = Value;
            await OnSubmit.InvokeAsync(Value);
            if (ClearOnSubmit)
            {
                await SetValueAsync(string.Empty);
                if (OnClear.HasDelegate)
                {
                    await OnClear.InvokeAsync(submittedValue);
                }
            }
        }

        private async Task StopAsync(MouseEventArgs args)
        {
            if (OnStop.HasDelegate)
            {
                await OnStop.InvokeAsync();
            }
        }

        private async Task ClearAsync(MouseEventArgs args)
        {
            await ClearValueAsync();
        }

        private async Task ClearValueAsync()
        {
            if (IsClearDisabled)
            {
                return;
            }

            var oldValue = Value;
            await SetValueAsync(string.Empty);
            if (OnClear.HasDelegate)
            {
                await OnClear.InvokeAsync(oldValue);
            }
            try
            {
                await FocusAsync();
            }
            catch
            {
            }
        }

        private async Task SetValueAsync(string value)
        {
            Value = value;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }

        private bool ShouldSubmit(KeyboardEventArgs args)
        {
            if (args?.Key != "Enter" || args.ShiftKey || args.AltKey)
            {
                return false;
            }

            if (args.CtrlKey)
            {
                return SubmitOnCtrlEnter;
            }

            if (args.MetaKey)
            {
                return SubmitOnMetaEnter;
            }

            return SubmitOnEnter;
        }

        public ValueTask FocusAsync()
        {
            return textareaElement.Dom(JSRuntime).FocusAsync();
        }

        public ValueTask BlurAsync()
        {
            return textareaElement.Dom(JSRuntime).BlurAsync();
        }
    }
}
