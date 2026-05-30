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
        private static long mentionSenderIdSeed;
        private readonly string generatedHelpId = $"el-x-mention-sender-help-{System.Threading.Interlocked.Increment(ref mentionSenderIdSeed)}";
        private MentionOption selectedCommand;

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
        public EventCallback<MentionOption> SelectedCommandChanged { get; set; }

        [Parameter]
        public EventCallback<string> OnSubmit { get; set; }

        [Parameter]
        public EventCallback OnStop { get; set; }

        [Parameter]
        public EventCallback<string> OnClear { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnShortcutSubmit { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnEscape { get; set; }

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
        public bool SubmitOnShiftEnter { get; set; }

        [Parameter]
        public bool SubmitOnAltEnter { get; set; }

        [Parameter]
        public bool ClearOnSubmit { get; set; }

        [Parameter]
        public bool ClearOnEscape { get; set; }

        [Parameter]
        public bool ShowClearButton { get; set; }

        [Parameter]
        public bool Clearable { get; set; }

        [Parameter]
        public bool ApplyCommandOnSelect { get; set; }

        [Parameter]
        public bool ClearCommandOnSubmit { get; set; } = true;

        [Parameter]
        public Func<MentionOption, string> CommandTextSelector { get; set; }

        [Parameter]
        public string SendButtonText { get; set; } = "Send";

        [Parameter]
        public string StopButtonText { get; set; } = "Stop";

        [Parameter]
        public string ClearButtonText { get; set; } = "Clear";

        [Parameter]
        public string SendButtonIcon { get; set; }

        [Parameter]
        public string StopButtonIcon { get; set; }

        [Parameter]
        public string ClearButtonIcon { get; set; } = "el-icon-circle-close";

        [Parameter]
        public string HelpText { get; set; }

        [Parameter]
        public string AriaDescribedBy { get; set; }

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

        [Parameter]
        public MentionOption SelectedCommand
        {
            get => selectedCommand;
            set => selectedCommand = value;
        }

        protected string MentionSenderClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-mention-sender", Cls)
            .AddIf(Loading, "is-loading")
            .AddIf(Disabled, "is-disabled")
            .AddIf(Readonly, "is-readonly")
            .ToString();

        protected bool IsSendDisabled => Disabled || Readonly || Loading || (!AllowEmpty && string.IsNullOrWhiteSpace(Value));

        protected bool IsClearDisabled => Disabled || Readonly || string.IsNullOrEmpty(Value);

        protected string HelpId => generatedHelpId;

        protected string EffectiveAriaDescribedBy
        {
            get
            {
                if (string.IsNullOrWhiteSpace(HelpText))
                {
                    return AriaDescribedBy;
                }

                return string.IsNullOrWhiteSpace(AriaDescribedBy)
                    ? HelpId
                    : $"{AriaDescribedBy} {HelpId}";
            }
        }

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
            await SetSelectedCommandAsync(option);

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

            if (args?.Key == "Escape" && ClearOnEscape)
            {
                if (OnEscape.HasDelegate)
                {
                    await OnEscape.InvokeAsync(args);
                }
                await ClearValueAsync();
                return;
            }
            if (args?.Key == "Escape" && OnEscape.HasDelegate)
            {
                await OnEscape.InvokeAsync(args);
            }

            if (ShouldSubmit(args))
            {
                if (OnShortcutSubmit.HasDelegate)
                {
                    await OnShortcutSubmit.InvokeAsync(args);
                }
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
            if (ClearCommandOnSubmit)
            {
                await SetSelectedCommandAsync(null);
            }
        }

        private async Task StopAsync(MouseEventArgs args)
        {
            if (OnStop.HasDelegate)
            {
                await OnStop.InvokeAsync();
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
            await SetSelectedCommandAsync(null);
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
            if (args?.Key != "Enter")
            {
                return false;
            }

            if (args.ShiftKey)
            {
                return SubmitOnShiftEnter;
            }

            if (args.AltKey)
            {
                return SubmitOnAltEnter;
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

        private async Task SetSelectedCommandAsync(MentionOption option)
        {
            if (ReferenceEquals(SelectedCommand, option))
            {
                return;
            }

            SelectedCommand = option;
            if (SelectedCommandChanged.HasDelegate)
            {
                await SelectedCommandChanged.InvokeAsync(option);
            }
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
