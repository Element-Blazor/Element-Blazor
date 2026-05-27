using Microsoft.AspNetCore.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElCheckboxGroup<TValue> : ElementFieldComponentBase<IEnumerable<TValue>>
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public IEnumerable<TValue> Value { get; set; }

        [Parameter]
        public IEnumerable<TValue> ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<IEnumerable<TValue>> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<IEnumerable<TValue>> ModelValueChanged { get; set; }

        [Parameter]
        public EventCallback<IEnumerable<TValue>> OnChange { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        internal bool EffectiveDisabled => Disabled || (FormItem?.Form?.Disabled ?? false);

        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        [Parameter]
        public int? Min { get; set; }

        [Parameter]
        public int? Max { get; set; }

        internal InputSize EffectiveSize => Size == InputSize.Normal
            ? FormItem?.Size ?? FormItem?.Form?.EffectiveSize ?? ResolveInputSize(InputSize.Normal)
            : Size;

        internal ObservableCollection<TValue> SelectedItems { get; set; } = new ObservableCollection<TValue>();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
            SelectedItems = new ObservableCollection<TValue>();
            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Value != null && !SelectedItems.SequenceEqual(Value))
            {
                ReplaceSelectedItems(Value);
            }

            if (FormItem == null)
            {
                return;
            }

            if (FormItem.OriginValueHasRendered)
            {
                return;
            }

            FormItem.OriginValueHasRendered = true;
            if (FormItem.Value == null)
            {
                ReplaceSelectedItems(Enumerable.Empty<TValue>());
            }
            else
            {
                ReplaceSelectedItems(NormalizeValues(FormItem.OriginValue));
            }
            SetFieldValue(SelectedItems.ToList(), false);
        }

        private async void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetFieldValue(SelectedItems.ToList(), true);
            var values = SelectedItems.ToList();
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(values);
            }
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(values);
            }
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(values);
            }
        }

        internal async Task<bool> TryToggleAsync(TValue value)
        {
            if (EffectiveDisabled)
            {
                return false;
            }

            var selected = SelectedItems.Contains(value);
            if (selected)
            {
                if (Min.HasValue && SelectedItems.Count <= Min.Value)
                {
                    return false;
                }

                SelectedItems.Remove(value);
            }
            else
            {
                if (Max.HasValue && SelectedItems.Count >= Max.Value)
                {
                    return false;
                }

                SelectedItems.Add(value);
            }

            await Task.CompletedTask;
            return true;
        }

        internal bool IsLimitDisabled(TValue value)
        {
            if (EffectiveDisabled)
            {
                return true;
            }

            var selected = SelectedItems.Contains(value);
            return selected
                ? Min.HasValue && SelectedItems.Count <= Min.Value
                : Max.HasValue && SelectedItems.Count >= Max.Value;
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            if (value != null)
            {
                ReplaceSelectedItems(NormalizeValues(value));
            }
            else
            {
                ReplaceSelectedItems(Enumerable.Empty<TValue>());
            }
            SetFieldValue(SelectedItems.ToList(), false);
            StateHasChanged();
        }

        private void ReplaceSelectedItems(IEnumerable<TValue> values)
        {
            SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
            SelectedItems = new ObservableCollection<TValue>(values ?? Enumerable.Empty<TValue>());
            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
        }

        private static IEnumerable<TValue> NormalizeValues(object value)
        {
            if (value == null)
            {
                return Enumerable.Empty<TValue>();
            }
            if (value is IEnumerable<TValue> typedValues)
            {
                return typedValues;
            }
            if (value is IEnumerable values && value is not string)
            {
                return values.Cast<object>()
                    .Where(x => x != null)
                    .Select(x => (TValue)TypeHelper.ChangeType(x, typeof(TValue)));
            }
            return new[] { (TValue)TypeHelper.ChangeType(value, typeof(TValue)) };
        }

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
