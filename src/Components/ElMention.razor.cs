using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElMention : ElementFieldComponentBase<string>
    {
        private HtmlPropertyBuilder wrapperClsBuilder;
        private InputSize effectiveSize = InputSize.Normal;
        private bool effectiveDisabled;
        private string searchText;
        private bool dropdownVisible;
        private int activeIndex;

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public string ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<string> ModelValueChanged { get; set; }

        [Parameter]
        public IEnumerable<MentionOption> Options { get; set; } = Enumerable.Empty<MentionOption>();

        [Parameter]
        public string Prefix { get; set; } = "@";

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public int Rows { get; set; } = 2;

        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        [Parameter]
        public bool Readonly { get; set; }

        [Parameter]
        public bool Clearable { get; set; }

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public EventCallback<string> OnInput { get; set; }

        [Parameter]
        public EventCallback<MentionOption> OnSelect { get; set; }

        [Parameter]
        public EventCallback<string> OnChange { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            effectiveDisabled = Disabled || (FormItem?.Form?.Disabled ?? false);
            effectiveSize = Size == InputSize.Normal
                ? FormItem?.Size ?? FormItem?.Form?.EffectiveSize ?? ResolveInputSize(InputSize.Normal)
                : Size;
            var sizeCssValue = GetSizeCssValue(effectiveSize);
            wrapperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-mention", "el-textarea", Cls)
                .AddIf(sizeCssValue != null, $"el-textarea--{sizeCssValue}")
                .AddIf(effectiveDisabled, "is-disabled")
                .AddIf(dropdownVisible, "is-focus");

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    Value = Convert.ToString(FormItem.OriginValue);
                }
                SetFieldValue(Value, false);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = Convert.ToString(value);
            searchText = null;
            dropdownVisible = false;
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                _ = ModelValueChanged.InvokeAsync(Value);
            }
            else
            {
                StateHasChanged();
            }
        }

        private async Task OnInputAsync(ChangeEventArgs e)
        {
            Value = Convert.ToString(e.Value);
            RefreshSearch();
            SetFieldValue(Value, false);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
            if (OnInput.HasDelegate)
            {
                await OnInput.InvokeAsync(Value);
            }
        }

        private async Task OnChangeAsync(ChangeEventArgs e)
        {
            if (ValidateEvent)
            {
                SetFieldValue(Value, true);
            }
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
        }

        private async Task ClearAsync(MouseEventArgs e)
        {
            if (!Clearable || effectiveDisabled || Readonly)
            {
                return;
            }

            Value = string.Empty;
            searchText = null;
            dropdownVisible = false;
            SetFieldValue(Value, ValidateEvent);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }
        }

        private void OnBlur(FocusEventArgs e)
        {
            dropdownVisible = false;
            if (ValidateEvent)
            {
                SetFieldValue(Value, true);
            }
        }

        private async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            var options = FilteredOptions.ToList();
            if (!dropdownVisible || !options.Any())
            {
                return;
            }

            if (e.Key == "ArrowDown")
            {
                activeIndex = (activeIndex + 1) % options.Count;
            }
            else if (e.Key == "ArrowUp")
            {
                activeIndex = (activeIndex - 1 + options.Count) % options.Count;
            }
            else if (e.Key == "Enter")
            {
                await SelectOptionAsync(options[activeIndex]);
            }
            else if (e.Key == "Escape")
            {
                dropdownVisible = false;
            }
        }

        private async Task SelectOptionAsync(MentionOption option)
        {
            if (effectiveDisabled || Readonly || option == null || option.Disabled)
            {
                return;
            }

            var token = $"{Prefix}{searchText}";
            var text = Value ?? string.Empty;
            var index = text.LastIndexOf(token, StringComparison.Ordinal);
            var mentionValue = $"{Prefix}{(string.IsNullOrWhiteSpace(option.Value) ? option.Label : option.Value)}";
            Value = index >= 0
                ? text.Substring(0, index) + mentionValue + text.Substring(index + token.Length)
                : text + mentionValue;
            dropdownVisible = false;
            searchText = null;
            SetFieldValue(Value, ValidateEvent);
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(Value);
            }
            if (OnSelect.HasDelegate)
            {
                await OnSelect.InvokeAsync(option);
            }
        }

        private void RefreshSearch()
        {
            var text = Value ?? string.Empty;
            var prefix = string.IsNullOrEmpty(Prefix) ? "@" : Prefix;
            var index = text.LastIndexOf(prefix, StringComparison.Ordinal);
            if (index < 0)
            {
                searchText = null;
                dropdownVisible = false;
                activeIndex = 0;
                return;
            }

            var query = text.Substring(index + prefix.Length);
            if (query.Any(char.IsWhiteSpace))
            {
                searchText = null;
                dropdownVisible = false;
                activeIndex = 0;
                return;
            }

            searchText = query;
            dropdownVisible = FilteredOptions.Any();
            activeIndex = Math.Min(activeIndex, Math.Max(FilteredOptions.Count() - 1, 0));
        }

        private IEnumerable<MentionOption> FilteredOptions
        {
            get
            {
                var options = Options ?? Enumerable.Empty<MentionOption>();
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    return options;
                }
                return options.Where(x =>
                    (x.Label ?? string.Empty).IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) >= 0
                    || (x.Value ?? string.Empty).IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) >= 0);
            }
        }

        private bool IsMentionDisabled => effectiveDisabled;

        private bool ShowClear => Clearable && !effectiveDisabled && !Readonly && !string.IsNullOrEmpty(Value);

        private static string GetSizeCssValue(InputSize size) => size switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };
    }
}
