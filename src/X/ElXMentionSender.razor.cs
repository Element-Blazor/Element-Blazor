using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Element.X
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
        public EventCallback<MentionOption> OnCommandSelect { get; set; }

        [Parameter]
        public EventCallback<string> OnSubmit { get; set; }

        [Parameter]
        public EventCallback<string> OnClear { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

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
        public bool SubmitOnEnter { get; set; } = true;

        [Parameter]
        public bool SubmitOnCtrlEnter { get; set; }

        [Parameter]
        public bool SubmitOnMetaEnter { get; set; }

        [Parameter]
        public bool ClearOnSubmit { get; set; }

        [Parameter]
        public bool ShowClearButton { get; set; }

        [Parameter]
        public bool Clearable { get; set; }

        [Parameter]
        public bool ApplyCommandOnSelect { get; set; }

        [Parameter]
        public Func<MentionOption, string> CommandTextSelector { get; set; }

        [Parameter]
        public string SendButtonText { get; set; } = "Send";

        [Parameter]
        public string ClearButtonText { get; set; } = "Clear";

        [Parameter]
        public string AriaLabel { get; set; } = "Command message input";

        [Parameter]
        public string Autocomplete { get; set; } = "off";

        [Parameter]
        public object Tabindex { get; set; } = 0;

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string InputStyle { get; set; }

        [Parameter]
        public RenderFragment<MentionOption> ItemTemplate { get; set; }

        [Parameter]
        public RenderFragment Toolbar { get; set; }

        protected string MentionSenderClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-mention-sender", Cls)
            .AddIf(Loading, "is-loading")
            .ToString();

        protected bool IsSendDisabled => Disabled || Readonly || Loading || (!AllowEmpty && string.IsNullOrWhiteSpace(Value));

        protected bool IsClearDisabled => Disabled || Readonly || string.IsNullOrEmpty(Value);

        private async Task OnValueChangedAsync(string value)
        {
            await SetValueAsync(value);
        }

        private async Task SendAsync(MouseEventArgs args)
        {
            await SubmitAsync();
        }

        private async Task ClearAsync(MouseEventArgs args)
        {
            await ClearValueAsync();
        }

        private async Task HandleMentionSelectAsync(MentionOption option)
        {
            if (OnMentionSelect.HasDelegate)
            {
                await OnMentionSelect.InvokeAsync(option);
            }
            if (OnCommandSelect.HasDelegate)
            {
                await OnCommandSelect.InvokeAsync(option);
            }

            if (ApplyCommandOnSelect && option != null)
            {
                await SetValueAsync(ResolveCommandText(option));
            }
        }

        private async Task OnMentionKeyDownAsync(KeyboardEventArgs args)
        {
            if (OnKeyDown.HasDelegate)
            {
                await OnKeyDown.InvokeAsync(args);
            }

            if (ShouldSubmit(args))
            {
                await SubmitAsync();
            }
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
        }

        private async Task SetValueAsync(string value)
        {
            Value = value;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
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

        private string ResolveCommandText(MentionOption option)
        {
            if (CommandTextSelector != null)
            {
                return CommandTextSelector(option);
            }

            var prefix = string.IsNullOrWhiteSpace(Prefix) ? "/" : Prefix;
            var value = string.IsNullOrWhiteSpace(option.Value) ? option.Label : option.Value;
            return $"{prefix}{value} ";
        }
    }
}
