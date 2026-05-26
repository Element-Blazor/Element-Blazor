using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElInputTag : ElementFieldComponentBase<IList<string>>
    {
        private readonly List<string> currentTags = new List<string>();
        private HtmlPropertyBuilder wrapperClsBuilder;
        private ElementReference inputElement;
        private InputSize effectiveSize = InputSize.Normal;
        private bool effectiveDisabled;
        private string inputText;

        [Parameter]
        public IList<string> Value { get; set; } = new List<string>();

        [Parameter]
        public IList<string> ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<IList<string>> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<IList<string>> ModelValueChanged { get; set; }

        [Parameter]
        public string Placeholder { get; set; } = "请输入";

        [Parameter]
        public bool Clearable { get; set; }

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
        public InputSize Size { get; set; } = InputSize.Normal;

        [Parameter]
        public int? Max { get; set; }

        [Parameter]
        public bool AllowDuplicates { get; set; }

        [Parameter]
        public string[] TriggerKeys { get; set; } = new[] { "Enter", "," };

        [Parameter]
        public bool AddOnBlur { get; set; } = true;

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public EventCallback<IList<string>> OnChange { get; set; }

        [Parameter]
        public EventCallback<string> OnAdd { get; set; }

        [Parameter]
        public EventCallback<string> OnRemove { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            effectiveDisabled = Disabled || (FormItem?.Form?.Disabled ?? false);
            effectiveSize = Size == InputSize.Normal
                ? FormItem?.Size ?? FormItem?.Form?.EffectiveSize ?? ResolveInputSize(InputSize.Normal)
                : Size;
            var sizeCssValue = GetSizeCssValue(effectiveSize);
            wrapperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-input-tag", Cls)
                .AddIf(effectiveDisabled, "is-disabled")
                .AddIf(Readonly, "is-readonly")
                .AddIf(sizeCssValue != null, $"el-input-tag--{sizeCssValue}");

            if (FormItem != null && !FormItem.OriginValueHasRendered)
            {
                FormItem.OriginValueHasRendered = true;
                if (FormItem.Form.Values.Any())
                {
                    Value = NormalizeValue(FormItem.OriginValue);
                }
                SetFieldValue(Value, false);
            }

            SyncTagsFromValue();
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = NormalizeValue(value);
            SyncTagsFromValue();
            inputText = string.Empty;
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

        private Task OnInputAsync(ChangeEventArgs e)
        {
            inputText = Convert.ToString(e.Value);
            return Task.CompletedTask;
        }

        private async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (TriggerKeys != null && TriggerKeys.Contains(e.Key))
            {
                await AddInputTagAsync();
            }
            else if (e.Key == "Backspace" && string.IsNullOrEmpty(inputText) && currentTags.Any())
            {
                await RemoveTagAsync(currentTags.Last());
            }
        }

        private async Task OnBlurAsync(FocusEventArgs e)
        {
            if (AddOnBlur)
            {
                await AddInputTagAsync();
            }
        }

        private async Task AddInputTagAsync()
        {
            if (effectiveDisabled || Readonly || IsLimitReached)
            {
                return;
            }

            var tag = (inputText ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(tag))
            {
                return;
            }
            if (!AllowDuplicates && currentTags.Contains(tag))
            {
                inputText = string.Empty;
                return;
            }

            currentTags.Add(tag);
            inputText = string.Empty;
            await CommitTagsAsync(ValidateEvent);
            if (OnAdd.HasDelegate)
            {
                await OnAdd.InvokeAsync(tag);
            }
        }

        private async Task RemoveTagAsync(string tag)
        {
            if (effectiveDisabled || Readonly)
            {
                return;
            }
            if (!currentTags.Remove(tag))
            {
                return;
            }
            await CommitTagsAsync(ValidateEvent);
            if (OnRemove.HasDelegate)
            {
                await OnRemove.InvokeAsync(tag);
            }
        }

        private async Task CommitTagsAsync(bool validate)
        {
            Value = currentTags.ToList();
            SetFieldValue(Value, validate);
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

        private async Task FocusInputAsync()
        {
            if (effectiveDisabled || Readonly || IsLimitReached)
            {
                return;
            }
            await inputElement.Dom(JSRuntime).FocusAsync();
        }

        private void SyncTagsFromValue()
        {
            currentTags.Clear();
            currentTags.AddRange(Value ?? Enumerable.Empty<string>());
        }

        private IReadOnlyList<string> CurrentTags => currentTags;

        private bool IsInputTagDisabled => effectiveDisabled;

        private bool IsLimitReached => Max.HasValue && currentTags.Count >= Max.Value;

        private string EffectivePlaceholder => currentTags.Any() ? null : Placeholder;

        private static IList<string> NormalizeValue(object value)
        {
            if (value == null)
            {
                return new List<string>();
            }
            if (value is IList<string> stringList)
            {
                return stringList;
            }
            if (value is IEnumerable<string> stringEnumerable)
            {
                return stringEnumerable.ToList();
            }
            return new List<string> { Convert.ToString(value) };
        }

        private static string GetSizeCssValue(InputSize size) => size switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };
    }
}
