using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElDropdown : ElementComponentBase
    {
        internal ElementReference Target;
        private DropDownOption dropDownOption;
        private int hoverCloseVersion;

        [Parameter]
        public RenderFragment Trigger { get; set; }

        [Parameter]
        public RenderFragment Items { get; set; }

        [Parameter]
        public DropdownTrigger TriggerMode { get; set; } = DropdownTrigger.Click;

        [Parameter]
        public string TriggerType
        {
            get => TriggerMode.ToString().ToLowerInvariant();
            set => TriggerMode = ParseTrigger(value);
        }

        [Parameter]
        public bool SplitButton { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public ButtonType Type { get; set; } = ButtonType.Default;

        [Parameter]
        public ButtonSize Size { get; set; } = ButtonSize.Default;

        [Parameter]
        public string Icon { get; set; } = "el-icon-arrow-down";

        [Parameter]
        public RenderFragment ButtonContent { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public EventCallback<DropdownCommandEventArgs> OnCommand { get; set; }

        [Inject]
        private PopupService PopupService { get; set; }

        protected string DropdownClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-dropdown", Cls)
            .AddIf(Disabled, "is-disabled")
            .ToString();

        internal bool IsDropDownOpen => dropDownOption != null && PopupService.DropDownMenuOptions.Any(x => x == dropDownOption);

        internal async Task NotifyCommandAsync(ElDropdownItem item)
        {
            if (!OnCommand.HasDelegate)
            {
                return;
            }

            await OnCommand.InvokeAsync(new DropdownCommandEventArgs
            {
                Command = item.Command,
                Item = item
            });
        }

        protected async Task OnTriggerClickAsync(MouseEventArgs e)
        {
            if (SplitButton || TriggerMode != DropdownTrigger.Click)
            {
                return;
            }

            await ToggleDropDownAsync();
        }

        protected async Task OnTriggerMouseOverAsync(MouseEventArgs e)
        {
            if (TriggerMode != DropdownTrigger.Hover)
            {
                return;
            }

            hoverCloseVersion++;
            await ShowDropDownAsync();
        }

        protected async Task OnTriggerMouseOutAsync(MouseEventArgs e)
        {
            if (TriggerMode != DropdownTrigger.Hover || dropDownOption?.Instance == null)
            {
                return;
            }

            await CloseAfterHoverDelayAsync();
        }

        internal void KeepOpen()
        {
            hoverCloseVersion++;
        }

        internal Task CloseAfterHoverDelayAsync()
        {
            if (TriggerMode != DropdownTrigger.Hover)
            {
                return Task.CompletedTask;
            }

            return CloseAfterHoverDelayCoreAsync(++hoverCloseVersion);
        }

        private async Task CloseAfterHoverDelayCoreAsync(int version)
        {
            var option = dropDownOption;
            await Task.Delay(120);
            if (version != hoverCloseVersion || option?.Instance == null)
            {
                return;
            }

            await InvokeAsync(() => option.Instance.CloseDropDownAsync(option));
        }

        protected async Task OnTriggerContextMenuAsync(MouseEventArgs e)
        {
            if (TriggerMode != DropdownTrigger.ContextMenu)
            {
                return;
            }

            await ShowDropDownAsync();
        }

        protected async Task OnSplitButtonClickAsync(MouseEventArgs e)
        {
            if (Disabled)
            {
                return;
            }

            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(e);
            }
        }

        protected async Task OnCaretButtonClickAsync(MouseEventArgs e)
        {
            await ToggleDropDownAsync();
        }

        protected async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (e.Key != "Enter" && e.Key != " " && e.Key != "ArrowDown")
            {
                return;
            }

            await ToggleDropDownAsync();
        }

        internal async Task ToggleDropDownAsync()
        {
            if (IsDropDownOpen && dropDownOption?.Instance != null)
            {
                await dropDownOption.Instance.CloseDropDownAsync(dropDownOption);
                return;
            }

            await ShowDropDownAsync();
        }

        internal Task ShowDropDownAsync()
        {
            if (Disabled)
            {
                return Task.CompletedTask;
            }

            if (PopupService.DropDownMenuOptions.Any(x => x.Target.Id == Target.Id))
            {
                return Task.CompletedTask;
            }

            dropDownOption = new DropDownOption()
            {
                Target = Target,
                OptionContent = Items,
                Select = this,
                Refresh = () => StateHasChanged(),
                OnClosed = () =>
                {
                    StateHasChanged();
                    return Task.CompletedTask;
                },
                IsShow = true
            };
            PopupService.DropDownMenuOptions.Add(dropDownOption);
            return Task.CompletedTask;
        }

        internal void ShowDropDown()
        {
            _ = ShowDropDownAsync();
        }

        private static DropdownTrigger ParseTrigger(string value)
        {
            if (string.Equals(value, "click", StringComparison.OrdinalIgnoreCase))
            {
                return DropdownTrigger.Click;
            }

            if (string.Equals(value, "contextmenu", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "context-menu", StringComparison.OrdinalIgnoreCase))
            {
                return DropdownTrigger.ContextMenu;
            }

            return DropdownTrigger.Hover;
        }
    }
}
