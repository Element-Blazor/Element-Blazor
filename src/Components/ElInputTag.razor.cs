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
        private int? selectionAnchorIndex;
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
            ClearSelection();
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
            ClearSelection();
            return Task.CompletedTask;
        }

        private async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (TriggerKeys != null && TriggerKeys.Contains(e.Key) && !HasCommandModifier(e))
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
                    await RemoveTagsBackwardsAsync(e.ShiftKey);
                }
                else if (selectedTagIndex.HasValue)
                {
                    await RemoveSelectedTagsAsync(backwards: true);
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
                    await RemoveTagsFromAsync(e.ShiftKey ? SelectionStart : selectedTagIndex.Value);
                }
                else
                {
                    await RemoveSelectedTagsAsync(backwards: false);
                }
            }
            else if (e.Key == "ArrowLeft" && string.IsNullOrEmpty(inputText) && currentTags.Any())
            {
                MoveSelectionLeft(e.ShiftKey);
            }
            else if (e.Key == "ArrowRight" && string.IsNullOrEmpty(inputText) && currentTags.Any())
            {
                MoveSelectionRight(e.ShiftKey);
            }
            else if ((e.Key == "a" || e.Key == "A") && (e.CtrlKey || e.MetaKey) && currentTags.Any() && string.IsNullOrEmpty(inputText))
            {
                SelectTagRange(0, currentTags.Count - 1);
            }
            else if (e.Key == "Home" && string.IsNullOrEmpty(inputText) && currentTags.Any())
            {
                if (e.ShiftKey && selectedTagIndex.HasValue)
                {
                    ExtendSelectionTo(0);
                }
                else
                {
                    SelectTag(0);
                }
            }
            else if (e.Key == "End" && string.IsNullOrEmpty(inputText) && currentTags.Any())
            {
                if (e.ShiftKey && selectedTagIndex.HasValue)
                {
                    ExtendSelectionTo(currentTags.Count - 1);
                }
                else
                {
                    SelectTag(currentTags.Count - 1);
                }
            }
            else if ((e.Key == "Escape" || e.Key == "Esc") && selectedTagIndex.HasValue)
            {
                ClearSelection();
            }
        }

        private static bool HasCommandModifier(KeyboardEventArgs e)
        {
            return e.CtrlKey || e.MetaKey || e.AltKey;
        }

        private async Task OnBlurAsync(FocusEventArgs e)
        {
            ClearSelection();
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
            ClearSelection();
            await CommitTagsAsync(ValidateEvent);
            if (OnAdd.HasDelegate)
            {
                await OnAdd.InvokeAsync(tag);
            }
        }

        private async Task RemoveTagAtAsync(int index)
        {
            if (effectiveDisabled || Readonly || index < 0 || index >= currentTags.Count)
            {
                return;
            }

            var tag = currentTags[index];
            currentTags.RemoveAt(index);
            ResolveSelectionAfterRemoval(index);
            await CommitTagsAsync(ValidateEvent);
            if (OnRemove.HasDelegate)
            {
                await OnRemove.InvokeAsync(tag);
            }
        }

        private async Task RemoveSelectedTagsAsync(bool backwards)
        {
            if (effectiveDisabled || Readonly || !selectedTagIndex.HasValue)
            {
                return;
            }

            await RemoveTagsRangeAsync(SelectionStart, SelectionEnd, backwards);
        }

        private async Task RemoveTagsFromAsync(int startIndex)
        {
            if (effectiveDisabled || Readonly || startIndex < 0 || startIndex >= currentTags.Count)
            {
                return;
            }

            var removedTags = currentTags.Skip(startIndex).ToList();
            currentTags.RemoveRange(startIndex, currentTags.Count - startIndex);
            SelectAfterRangeRemoval(startIndex, removedTags.Count, backwards: false);
            await CommitTagsAsync(ValidateEvent);
            if (OnRemove.HasDelegate)
            {
                foreach (var removedTag in removedTags)
                {
                    await OnRemove.InvokeAsync(removedTag);
                }
            }
        }

        private async Task RemoveTagsBackwardsAsync(bool selectionOnly)
        {
            if (effectiveDisabled || Readonly || !currentTags.Any())
            {
                return;
            }

            if (selectionOnly && selectedTagIndex.HasValue)
            {
                await RemoveTagsRangeAsync(0, SelectionEnd, backwards: true);
                return;
            }

            await RemoveTagsRangeAsync(0, currentTags.Count - 1, backwards: true);
        }

        private async Task RemoveTagsRangeAsync(int startIndex, int endIndex, bool backwards)
        {
            if (effectiveDisabled || Readonly || startIndex < 0 || endIndex < startIndex || startIndex >= currentTags.Count)
            {
                return;
            }

            endIndex = Math.Min(endIndex, currentTags.Count - 1);
            var count = endIndex - startIndex + 1;
            var removedTags = currentTags.Skip(startIndex).Take(count).ToList();
            currentTags.RemoveRange(startIndex, count);
            SelectAfterRangeRemoval(startIndex, count, backwards);
            await CommitTagsAsync(ValidateEvent);
            if (OnRemove.HasDelegate)
            {
                foreach (var removedTag in removedTags)
                {
                    await OnRemove.InvokeAsync(removedTag);
                }
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
            ClearSelection();
            await inputElement.Dom(JSRuntime).FocusAsync();
        }

        private Task SelectTagAsync(int index)
        {
            if (effectiveDisabled || Readonly || index < 0 || index >= currentTags.Count)
            {
                return Task.CompletedTask;
            }

            SelectTag(index);
            return Task.CompletedTask;
        }

        private Task OnTagDragStartAsync(int index, DragEventArgs e)
        {
            if (!IsTagDraggable)
            {
                return Task.CompletedTask;
            }

            ClearSelection();
            draggingIndex = index;
            dropIndex = index;
            return Task.CompletedTask;
        }

        private Task OnTagDragOverAsync(int index, DragEventArgs e)
        {
            if (!IsTagDraggable || !draggingIndex.HasValue || index < 0 || index >= currentTags.Count)
            {
                return Task.CompletedTask;
            }

            dropIndex = index;
            return Task.CompletedTask;
        }

        private async Task OnTagDropAsync(int index, DragEventArgs e)
        {
            if (!IsTagDraggable || !draggingIndex.HasValue || index < 0 || index >= currentTags.Count)
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
            SelectTag(newIndex);

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
                SelectTag(currentTags.Count > 0 ? currentTags.Count - 1 : null);
            }
            if (selectionAnchorIndex.HasValue && selectionAnchorIndex.Value >= currentTags.Count)
            {
                selectionAnchorIndex = selectedTagIndex;
            }
        }

        private IReadOnlyList<string> CurrentTags => currentTags;

        private bool IsInputTagDisabled => effectiveDisabled;

        private bool IsTagDraggable => Draggable && !effectiveDisabled && !Readonly && currentTags.Count > 1;

        private bool IsLimitReached => Max.HasValue && currentTags.Count >= Max.Value;

        private string EffectivePlaceholder => currentTags.Any() ? null : Placeholder;

        private string GetTagClass(int index) => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-tag", "el-tag--info", "el-tag--small", "el-tag--light")
            .AddIf(IsTagSelected(index), "is-focus")
            .AddIf(IsTagDraggable, "is-draggable")
            .AddIf(index == draggingIndex, "is-dragging")
            .AddIf(index == dropIndex && draggingIndex.HasValue && dropIndex != draggingIndex, "is-drop-target")
            .ToString();

        private void SelectLastTag()
        {
            SelectTag(currentTags.Count > 0 ? currentTags.Count - 1 : null);
        }

        private void MoveSelectionLeft(bool extend)
        {
            if (!selectedTagIndex.HasValue)
            {
                SelectLastTag();
                return;
            }

            var next = Math.Max(0, selectedTagIndex.Value - 1);
            if (extend)
            {
                ExtendSelectionTo(next);
                return;
            }

            SelectTag(next);
        }

        private void MoveSelectionRight(bool extend)
        {
            if (!selectedTagIndex.HasValue)
            {
                return;
            }

            if (extend)
            {
                ExtendSelectionTo(Math.Min(currentTags.Count - 1, selectedTagIndex.Value + 1));
                return;
            }

            if (selectedTagIndex.Value >= currentTags.Count - 1)
            {
                ClearSelection();
                return;
            }

            SelectTag(selectedTagIndex.Value + 1);
        }

        private void ResolveSelectionAfterRemoval(int removedIndex)
        {
            if (!currentTags.Any())
            {
                ClearSelection();
                return;
            }

            if (!selectedTagIndex.HasValue)
            {
                ClearSelection();
                return;
            }

            if (IsRangeSelected)
            {
                SelectAfterRangeRemoval(removedIndex, 1, backwards: false);
                return;
            }

            if (selectedTagIndex.Value > removedIndex)
            {
                SelectTag(selectedTagIndex.Value - 1);
                return;
            }

            SelectTag(Math.Min(selectedTagIndex.Value, currentTags.Count - 1));
        }

        private void SelectAfterRangeRemoval(int startIndex, int count, bool backwards)
        {
            if (!currentTags.Any())
            {
                ClearSelection();
                return;
            }

            var next = backwards ? startIndex - 1 : startIndex;
            next = Math.Clamp(next, 0, currentTags.Count - 1);
            SelectTag(next);
        }

        private void SelectTag(int? index)
        {
            selectedTagIndex = index;
            selectionAnchorIndex = index;
        }

        private void SelectTagRange(int anchorIndex, int activeIndex)
        {
            if (!currentTags.Any())
            {
                ClearSelection();
                return;
            }

            selectionAnchorIndex = Math.Clamp(anchorIndex, 0, currentTags.Count - 1);
            selectedTagIndex = Math.Clamp(activeIndex, 0, currentTags.Count - 1);
        }

        private void ExtendSelectionTo(int index)
        {
            if (!selectedTagIndex.HasValue)
            {
                SelectTag(index);
                return;
            }

            selectionAnchorIndex ??= selectedTagIndex;
            selectedTagIndex = Math.Clamp(index, 0, currentTags.Count - 1);
        }

        private void ClearSelection()
        {
            selectedTagIndex = null;
            selectionAnchorIndex = null;
        }

        private bool IsTagSelected(int index)
        {
            return selectedTagIndex.HasValue
                && selectionAnchorIndex.HasValue
                && index >= SelectionStart
                && index <= SelectionEnd;
        }

        private int SelectionStart => Math.Min(selectionAnchorIndex ?? selectedTagIndex ?? 0, selectedTagIndex ?? 0);

        private int SelectionEnd => Math.Max(selectionAnchorIndex ?? selectedTagIndex ?? 0, selectedTagIndex ?? 0);

        private bool IsRangeSelected => selectedTagIndex.HasValue && selectionAnchorIndex.HasValue && selectedTagIndex != selectionAnchorIndex;

        private static string GetAriaBoolean(bool value) => value ? "true" : "false";

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
