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
        private int? selectedTagIndex;
        private int? draggingIndex;
        private int? dropIndex;
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
        public bool Draggable { get; set; }

        [Parameter]
        public bool ValidateEvent { get; set; } = true;

        [Parameter]
        public EventCallback<IList<string>> OnChange { get; set; }

        [Parameter]
        public EventCallback<string> OnAdd { get; set; }

        [Parameter]
        public EventCallback<string> OnRemove { get; set; }

        [Parameter]
        public EventCallback<(int OldIndex, int NewIndex, string Tag)> OnDrag { get; set; }

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
                .AddIf(selectedTagIndex.HasValue, "is-focused")
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
            else if (FormItem != null && FormItem.OriginValueHasRendered && (Value == null || !Value.Any()) && FormItem.Value != null)
            {
                Value = NormalizeValue(FormItem.Value);
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
            selectedTagIndex = null;
            return Task.CompletedTask;
        }

        private async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (TriggerKeys != null && TriggerKeys.Contains(e.Key))
            {
                await AddInputTagAsync();
            }
            else if ((e.Key == "z" || e.Key == "Z") && (e.CtrlKey || e.MetaKey))
            {
                return;
            }
            else if (e.Key == "Backspace" && string.IsNullOrEmpty(inputText) && currentTags.Any())
            {
                if (e.CtrlKey || e.MetaKey)
                {
                    await RemoveTagsBackwardsAsync();
                }
                else if (selectedTagIndex.HasValue)
                {
                    await RemoveTagAtAsync(selectedTagIndex.Value);
                }
                else
                {
                    SelectLastTag();
                }
            }
            else if (e.Key == "Delete" && string.IsNullOrEmpty(inputText) && currentTags.Any() && selectedTagIndex.HasValue)
            {
                if (e.CtrlKey || e.MetaKey)
                {
                    await RemoveTagsFromAsync(selectedTagIndex.Value);
                }
                else
                {
                    await RemoveTagAtAsync(selectedTagIndex.Value);
                }
            }
            else if (e.Key == "ArrowLeft" && string.IsNullOrEmpty(inputText) && currentTags.Any())
            {
                MoveSelectionLeft();
            }
            else if (e.Key == "ArrowRight" && string.IsNullOrEmpty(inputText) && currentTags.Any())
            {
                MoveSelectionRight();
            }
            else if ((e.Key == "a" || e.Key == "A") && (e.CtrlKey || e.MetaKey) && currentTags.Any() && string.IsNullOrEmpty(inputText))
            {
                SelectLastTag();
            }
            else if (e.Key == "Home" && string.IsNullOrEmpty(inputText) && currentTags.Any())
            {
                selectedTagIndex = 0;
            }
            else if (e.Key == "End" && string.IsNullOrEmpty(inputText) && currentTags.Any())
            {
                selectedTagIndex = currentTags.Count - 1;
            }
            else if ((e.Key == "Escape" || e.Key == "Esc") && selectedTagIndex.HasValue)
            {
                selectedTagIndex = null;
            }
        }

        private async Task OnBlurAsync(FocusEventArgs e)
        {
            selectedTagIndex = null;
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
            selectedTagIndex = null;
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

            var index = currentTags.IndexOf(tag);
            if (index < 0)
            {
                return;
            }
            currentTags.RemoveAt(index);
            selectedTagIndex = ResolveSelectionAfterRemoval(index);
            await CommitTagsAsync(ValidateEvent);
            if (OnRemove.HasDelegate)
            {
                await OnRemove.InvokeAsync(tag);
            }
        }

        private async Task RemoveTagAtAsync(int index)
        {
            if (index < 0 || index >= currentTags.Count)
            {
                return;
            }

            var tag = currentTags[index];
            currentTags.RemoveAt(index);
            selectedTagIndex = ResolveSelectionAfterRemoval(index);
            await CommitTagsAsync(ValidateEvent);
            if (OnRemove.HasDelegate)
            {
                await OnRemove.InvokeAsync(tag);
            }
        }

        private async Task RemoveTagsFromAsync(int startIndex)
        {
            if (effectiveDisabled || Readonly || startIndex < 0 || startIndex >= currentTags.Count)
            {
                return;
            }

            var removedTags = currentTags.Skip(startIndex).ToList();
            currentTags.RemoveRange(startIndex, currentTags.Count - startIndex);
            selectedTagIndex = ResolveSelectionAfterRemoval(startIndex);
            await CommitTagsAsync(ValidateEvent);
            if (OnRemove.HasDelegate)
            {
                foreach (var removedTag in removedTags)
                {
                    await OnRemove.InvokeAsync(removedTag);
                }
            }
        }

        private async Task RemoveTagsBackwardsAsync()
        {
            if (effectiveDisabled || Readonly || !currentTags.Any())
            {
                return;
            }
            await RemoveTagsFromAsync(0);
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
            selectedTagIndex = null;
            await inputElement.Dom(JSRuntime).FocusAsync();
        }

        private Task SelectTagAsync(int index)
        {
            if (effectiveDisabled || Readonly || index < 0 || index >= currentTags.Count)
            {
                return Task.CompletedTask;
            }

            selectedTagIndex = index;
            return Task.CompletedTask;
        }

        private Task OnTagDragStartAsync(int index, DragEventArgs e)
        {
            if (!IsTagDraggable)
            {
                return Task.CompletedTask;
            }

            draggingIndex = index;
            dropIndex = index;
            return Task.CompletedTask;
        }

        private Task OnTagDragOverAsync(int index, DragEventArgs e)
        {
            if (!IsTagDraggable || !draggingIndex.HasValue)
            {
                return Task.CompletedTask;
            }

            dropIndex = index;
            return Task.CompletedTask;
        }

        private async Task OnTagDropAsync(int index, DragEventArgs e)
        {
            if (!IsTagDraggable || !draggingIndex.HasValue)
            {
                return;
            }

            dropIndex = index;
            await CommitDragAsync();
        }

        private async Task OnTagDragEndAsync(DragEventArgs e)
        {
            if (!draggingIndex.HasValue || !dropIndex.HasValue || draggingIndex == dropIndex)
            {
                draggingIndex = null;
                dropIndex = null;
                return;
            }

            await CommitDragAsync();
        }

        private async Task CommitDragAsync()
        {
            if (!draggingIndex.HasValue || !dropIndex.HasValue)
            {
                return;
            }

            var oldIndex = draggingIndex.Value;
            var newIndex = dropIndex.Value;
            draggingIndex = null;
            dropIndex = null;

            if (oldIndex == newIndex || oldIndex < 0 || oldIndex >= currentTags.Count || newIndex < 0 || newIndex >= currentTags.Count)
            {
                return;
            }

            var tag = currentTags[oldIndex];
            currentTags.RemoveAt(oldIndex);
            currentTags.Insert(newIndex, tag);
            selectedTagIndex = newIndex;

            await CommitTagsAsync(ValidateEvent);
            if (OnDrag.HasDelegate)
            {
                await OnDrag.InvokeAsync((oldIndex, newIndex, tag));
            }
        }

        private void SyncTagsFromValue()
        {
            currentTags.Clear();
            currentTags.AddRange(Value ?? Enumerable.Empty<string>());
            if (selectedTagIndex.HasValue && selectedTagIndex.Value >= currentTags.Count)
            {
                selectedTagIndex = currentTags.Count > 0 ? currentTags.Count - 1 : null;
            }
        }

        private IReadOnlyList<string> CurrentTags => currentTags;

        private bool IsInputTagDisabled => effectiveDisabled;

        private bool IsTagDraggable => Draggable && !effectiveDisabled && !Readonly && currentTags.Count > 1;

        private bool IsLimitReached => Max.HasValue && currentTags.Count >= Max.Value;

        private string EffectivePlaceholder => currentTags.Any() ? null : Placeholder;

        private string GetTagClass(int index) => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-tag", "el-tag--info", "el-tag--small", "el-tag--light")
            .AddIf(index == selectedTagIndex, "is-focus")
            .AddIf(IsTagDraggable, "is-draggable")
            .ToString();

        private void SelectLastTag()
        {
            selectedTagIndex = currentTags.Count > 0 ? currentTags.Count - 1 : null;
        }

        private void MoveSelectionLeft()
        {
            if (!selectedTagIndex.HasValue)
            {
                SelectLastTag();
                return;
            }

            selectedTagIndex = Math.Max(0, selectedTagIndex.Value - 1);
        }

        private void MoveSelectionRight()
        {
            if (!selectedTagIndex.HasValue)
            {
                return;
            }

            if (selectedTagIndex.Value >= currentTags.Count - 1)
            {
                selectedTagIndex = null;
                return;
            }

            selectedTagIndex++;
        }

        private int? ResolveSelectionAfterRemoval(int removedIndex)
        {
            if (!currentTags.Any())
            {
                return null;
            }

            if (!selectedTagIndex.HasValue)
            {
                return null;
            }

            if (selectedTagIndex.Value > removedIndex)
            {
                return selectedTagIndex.Value - 1;
            }

            return Math.Min(selectedTagIndex.Value, currentTags.Count - 1);
        }

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
