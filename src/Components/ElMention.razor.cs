using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElMention : ElementFieldComponentBase<string>
    {
        private static long mentionIdSeed;
        private HtmlPropertyBuilder wrapperClsBuilder;
        private InputSize effectiveSize = InputSize.Normal;
        private bool effectiveDisabled;
        private string searchText;
        private bool dropdownVisible;
        private int activeIndex;
        private string activePrefix;
        private int activeMentionStart = -1;
        private int activeMentionEnd = -1;
        private int lastSelectionStart = -1;
        private int lastSelectionEnd = -1;
        private readonly string generatedInputId = $"el-mention-input-{Interlocked.Increment(ref mentionIdSeed)}";
        private readonly string dropdownId = $"el-mention-dropdown-{Interlocked.Increment(ref mentionIdSeed)}";
        private ElementReference textareaElement;

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
        public IEnumerable<string> Prefixes { get; set; }

        [Parameter]
        public RenderFragment<MentionOption> ItemTemplate { get; set; }

        [Parameter]
        public RenderFragment<MentionOption> SuggestionItemTemplate
        {
            get => ItemTemplate;
            set => ItemTemplate = value;
        }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public int Rows { get; set; } = 2;

        [Parameter]
        public string Autocomplete { get; set; } = "off";

        [Parameter]
        public object Tabindex { get; set; } = 0;

        [Parameter]
        public string AriaLabel { get; set; }

        [Parameter]
        public string InputStyle { get; set; }

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
        public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

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
            Id = string.IsNullOrWhiteSpace(Id) ? ResolveAttributeId() ?? generatedInputId : Id;
            if (FormItem?.Form != null)
            {
                FormItem.Form.RegisterInput(FormItem.Name, Id, this, FormItem);
            }
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
            ClearSearchState();
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
            await CaptureSelectionAsync();
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
            ClearSearchState();
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

        private void OnBlurAsync(FocusEventArgs e)
        {
            dropdownVisible = false;
            if (ValidateEvent)
            {
                SetFieldValue(Value, true);
            }
        }

        private async Task OnFocusAsync(FocusEventArgs e)
        {
            await CaptureSelectionAsync();
        }

        private async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (effectiveDisabled || Readonly)
            {
                return;
            }

            if (e.Key == "ArrowDown" && !dropdownVisible)
            {
                await CaptureSelectionAsync();
                RefreshSearch();
                if (!dropdownVisible && OnKeyDown.HasDelegate)
                {
                    await OnKeyDown.InvokeAsync(e);
                }
                return;
            }

            var options = FilteredOptions.ToList();
            if (!dropdownVisible || !options.Any())
            {
                if (OnKeyDown.HasDelegate)
                {
                    await OnKeyDown.InvokeAsync(e);
                }
                return;
            }

            if (e.Key == "ArrowDown")
            {
                MoveActiveIndex(options, 1);
            }
            else if (e.Key == "ArrowUp")
            {
                MoveActiveIndex(options, -1);
            }
            else if (e.Key == "Enter" || e.Key == "Tab")
            {
                await SelectOptionAsync(options[activeIndex]);
            }
            else if (e.Key == "Escape")
            {
                dropdownVisible = false;
            }
            else if (e.Key == "Home")
            {
                activeIndex = FindNextEnabledIndex(options, 0, 1);
            }
            else if (e.Key == "End")
            {
                activeIndex = FindNextEnabledIndex(options, options.Count - 1, -1);
            }
            else if (OnKeyDown.HasDelegate)
            {
                await OnKeyDown.InvokeAsync(e);
            }
        }

        private async Task SelectOptionAsync(MentionOption option)
        {
            if (effectiveDisabled || Readonly || option == null || option.Disabled)
            {
                return;
            }

            var text = Value ?? string.Empty;
            var prefix = activePrefix ?? ResolvePrefixes().FirstOrDefault() ?? "@";
            var mentionValue = $"{prefix}{(string.IsNullOrWhiteSpace(option.Value) ? option.Label : option.Value)}";
            var start = activeMentionStart >= 0 ? activeMentionStart : text.Length;
            var end = activeMentionEnd >= start ? activeMentionEnd : start;
            Value = text.Substring(0, start) + mentionValue + text.Substring(Math.Min(end, text.Length));
            ClearSearchState();
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
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Value);
            }

            await SetSelectionAsync(start + mentionValue.Length);
        }

        private void RefreshSearch()
        {
            var text = Value ?? string.Empty;
            if (!TryGetActiveMention(text, out var context))
            {
                ClearSearchState();
                return;
            }

            activePrefix = context.Prefix;
            activeMentionStart = context.Start;
            activeMentionEnd = context.End;
            searchText = context.Query;
            dropdownVisible = FilteredOptions.Any();
            var options = FilteredOptions.ToList();
            activeIndex = options.Any()
                ? FindNextEnabledIndex(options, Math.Min(activeIndex, options.Count - 1), 1)
                : 0;
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

        private string ActiveDescendantId => dropdownVisible ? GetOptionId(activeIndex) : null;

        private bool IsMentionDisabled => effectiveDisabled;

        private bool ShowClear => Clearable && !effectiveDisabled && !Readonly && !string.IsNullOrEmpty(Value);

        private string AriaInvalid => IsAriaInvalid ? "true" : "false";

        private string GetOptionId(int index) => $"{dropdownId}-option-{index}";

        private IEnumerable<string> ResolvePrefixes()
        {
            var configured = (Prefixes ?? Enumerable.Empty<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.Ordinal)
                .ToList();

            if (!configured.Any())
            {
                configured.Add(string.IsNullOrWhiteSpace(Prefix) ? "@" : Prefix);
            }
            else if (!string.IsNullOrWhiteSpace(Prefix) && !configured.Contains(Prefix, StringComparer.Ordinal))
            {
                configured.Insert(0, Prefix);
            }

            return configured.OrderByDescending(x => x.Length);
        }

        private string ResolveAttributeId()
        {
            if (Attributes == null)
            {
                return null;
            }

            return Attributes.TryGetValue("id", out var id) ? Convert.ToString(id) : null;
        }

        private async Task CaptureSelectionAsync()
        {
            try
            {
                var selection = await JSRuntime.InvokeAsync<int[]>("elementMentionGetSelection", new object[] { textareaElement });
                if (selection?.Length >= 2)
                {
                    lastSelectionStart = selection[0];
                    lastSelectionEnd = selection[1];
                    return;
                }
            }
            catch
            {
            }

            var length = (Value ?? string.Empty).Length;
            lastSelectionStart = length;
            lastSelectionEnd = length;
        }

        private async Task SetSelectionAsync(int position)
        {
            lastSelectionStart = position;
            lastSelectionEnd = position;
            try
            {
                await JSRuntime.InvokeAsync<object>("elementMentionSetSelection", new object[] { textareaElement, position, position });
                await textareaElement.Dom(JSRuntime).FocusAsync();
            }
            catch
            {
            }
        }

        private void MoveActiveIndex(IReadOnlyList<MentionOption> options, int step)
        {
            if (options == null || options.Count == 0)
            {
                activeIndex = 0;
                return;
            }

            activeIndex = FindNextEnabledIndex(options, activeIndex + step, step);
        }

        private static int FindNextEnabledIndex(IReadOnlyList<MentionOption> options, int startIndex, int step)
        {
            if (options == null || options.Count == 0)
            {
                return 0;
            }

            if (options.All(x => x.Disabled))
            {
                return 0;
            }

            var index = ((startIndex % options.Count) + options.Count) % options.Count;
            for (var i = 0; i < options.Count; i++)
            {
                if (!options[index].Disabled)
                {
                    return index;
                }

                index = (index + step + options.Count) % options.Count;
            }

            return 0;
        }

        private bool TryGetActiveMention(string text, out MentionContext context)
        {
            context = null;
            var caret = ResolveCaret(text);
            foreach (var prefix in ResolvePrefixes())
            {
                var searchIndex = Math.Min(caret, text.Length);
                while (searchIndex >= 0)
                {
                    var index = text.LastIndexOf(prefix, searchIndex, StringComparison.Ordinal);
                    if (index < 0)
                    {
                        break;
                    }

                    if (!IsValidMentionBoundary(text, index))
                    {
                        searchIndex = index - 1;
                        continue;
                    }

                    var tokenStart = index + prefix.Length;
                    if (tokenStart > caret)
                    {
                        searchIndex = index - 1;
                        continue;
                    }

                    var tokenEnd = tokenStart;
                    while (tokenEnd < text.Length && !char.IsWhiteSpace(text[tokenEnd]))
                    {
                        tokenEnd++;
                    }

                    if (caret > tokenEnd)
                    {
                        searchIndex = index - 1;
                        continue;
                    }

                    var query = text.Substring(tokenStart, Math.Max(caret - tokenStart, 0));
                    if (query.Any(char.IsWhiteSpace))
                    {
                        searchIndex = index - 1;
                        continue;
                    }

                    context = new MentionContext
                    {
                        Prefix = prefix,
                        Start = index,
                        End = tokenEnd,
                        Query = query
                    };
                    return true;
                }
            }

            return false;
        }

        private int ResolveCaret(string text)
        {
            var length = (text ?? string.Empty).Length;
            if (lastSelectionStart < 0)
            {
                return length;
            }

            return Math.Max(0, Math.Min(lastSelectionStart, length));
        }

        private static bool IsValidMentionBoundary(string text, int index)
        {
            if (index <= 0)
            {
                return true;
            }

            return char.IsWhiteSpace(text[index - 1]);
        }

        private void ClearSearchState()
        {
            searchText = null;
            activePrefix = null;
            activeMentionStart = -1;
            activeMentionEnd = -1;
            dropdownVisible = false;
            activeIndex = 0;
        }

        private sealed class MentionContext
        {
            public string Prefix { get; set; }

            public int Start { get; set; }

            public int End { get; set; }

            public string Query { get; set; }
        }

        private static string GetSizeCssValue(InputSize size) => size switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };
    }
}
