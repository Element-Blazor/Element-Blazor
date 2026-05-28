using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElRadioGroup<TValue> : ElementFieldComponentBase<TValue>
    {
        private readonly List<ElRadio<TValue>> radios = new List<ElRadio<TValue>>();

        [Parameter]
        public EventCallback<TValue> SelectedValueChanged { get; set; }

        [Parameter]
        public RadioSize Size { get; set; }

        [Parameter]
        public bool IsBordered { get; set; }

        [Parameter]
        public bool Border
        {
            get => IsBordered;
            set => IsBordered = value;
        }

        [Parameter]
        public bool Bordered
        {
            get => IsBordered;
            set => IsBordered = value;
        }

        internal bool EffectiveBordered => IsBordered;

        internal RadioSize EffectiveSize
        {
            get
            {
                if (Size != RadioSize.Default)
                {
                    return ResolveRadioSize(Size);
                }

                return (FormItem?.Size ?? FormItem?.Form?.EffectiveSize) switch
                {
                    InputSize.Large => RadioSize.Medium,
                    InputSize.Small => RadioSize.Small,
                    _ => ResolveRadioSize(Size)
                };
            }
        }

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
        public EventCallback<ElementChangeEventArgs<TValue>> SelectedValueChanging { get; set; }

        [Parameter]
        public TValue SelectedValue { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SetFieldValue(SelectedValue, false);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (FormItem == null)
            {
                return;
            }
            if (FormItem.OriginValueHasRendered)
            {
                return;
            }
            FormItem.OriginValueHasRendered = true;
            if (FormItem.Form.Values.Any())
            {
                SelectedValue = FormItem.OriginValue == null
                    ? default
                    : (TValue)TypeHelper.ChangeType(FormItem.OriginValue, typeof(TValue));
            }
            SetFieldValue(SelectedValue, false);
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            if (value == null)
            {
                SelectedValue = default;
            }
            else
            {
                SelectedValue = (TValue)TypeHelper.ChangeType(value, typeof(TValue));
            }
        }

        internal void RegisterRadio(ElRadio<TValue> radio)
        {
            if (radio == null || radios.Contains(radio))
            {
                return;
            }

            radios.Add(radio);
        }

        internal void UnregisterRadio(ElRadio<TValue> radio)
        {
            if (radio == null)
            {
                return;
            }

            radios.Remove(radio);
        }

        internal int GetTabIndex(ElRadio<TValue> radio)
        {
            if (radio == null || radio.EffectiveDisabled)
            {
                return -1;
            }

            var enabled = radios.Where(x => !x.EffectiveDisabled).ToList();
            if (!enabled.Any())
            {
                return -1;
            }

            var selected = enabled.FirstOrDefault(x => x.IsChecked);
            if (selected != null)
            {
                return ReferenceEquals(selected, radio) ? 0 : -1;
            }

            return ReferenceEquals(enabled[0], radio) ? 0 : -1;
        }

        internal async Task MoveSelectionAsync(ElRadio<TValue> current, int step)
        {
            var enabled = radios.Where(x => !x.EffectiveDisabled).ToList();
            if (!enabled.Any())
            {
                return;
            }

            var index = enabled.IndexOf(current);
            if (index < 0)
            {
                index = enabled.FindIndex(x => x.IsChecked);
            }
            if (index < 0)
            {
                index = 0;
            }

            var target = enabled[(index + step + enabled.Count) % enabled.Count];
            var changed = await TrySetValueAsync(target.Value, requireRefresh: true);
            if (changed)
            {
                await target.FocusAsync();
            }
        }

        internal async Task SelectEdgeAsync(bool selectFirst)
        {
            var enabled = radios.Where(x => !x.EffectiveDisabled).ToList();
            if (!enabled.Any())
            {
                return;
            }

            var target = selectFirst ? enabled[0] : enabled[enabled.Count - 1];
            var changed = await TrySetValueAsync(target.Value, requireRefresh: true);
            if (changed)
            {
                await target.FocusAsync();
            }
        }

        internal async Task<bool> TrySetValueAsync(TValue value, bool requireRefresh)
        {
            if (EffectiveDisabled || TypeHelper.Equal(SelectedValue, value))
            {
                return false;
            }

            var arg = new ElementChangeEventArgs<TValue>
            {
                NewValue = value,
                OldValue = SelectedValue
            };
            if (SelectedValueChanging.HasDelegate)
            {
                await SelectedValueChanging.InvokeAsync(arg);
                if (arg.DisallowChange)
                {
                    return false;
                }
            }
            SelectedValue = value;
            SetFieldValue(SelectedValue, true);
            RequireRender = true;
            if (SelectedValueChanged.HasDelegate)
            {
                await SelectedValueChanged.InvokeAsync(value);
            }
            if (requireRefresh)
            {
                StateHasChanged();
            }
            return true;
        }

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
