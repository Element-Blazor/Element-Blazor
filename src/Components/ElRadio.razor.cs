using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElRadio<TValue> : ElementFieldComponentBase<TValue>
    {
        protected ElementReference radioElement;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public TValue Model
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public TValue SelectedValue { get; set; }

        [Parameter]
        public EventCallback<RadioStatus> StatusChanged { get; set; }

        [Parameter]
        public EventCallback<TValue> SelectedValueChanged { get; set; }

        [Parameter]
        public EventCallback<ElementChangeEventArgs<RadioStatus>> StatusChanging { get; set; }

        [CascadingParameter]
        public ElRadioGroup<TValue> RadioGroup { get; set; }

        [Parameter]
        public RadioStatus Status { get; set; } = RadioStatus.UnSelected;

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

        [Parameter]
        public bool IsDisabled { get; set; }

        internal bool EffectiveDisabled => IsDisabled || (RadioGroup?.EffectiveDisabled ?? false) || (FormItem?.Form?.Disabled ?? false);

        internal bool EffectiveBordered => IsBordered || (RadioGroup?.EffectiveBordered ?? false);

        [Parameter]
        public bool Disabled
        {
            get => IsDisabled;
            set => IsDisabled = value;
        }

        internal bool IsChecked => TypeHelper.Equal(Value, CurrentSelectedValue);

        internal TValue CurrentSelectedValue
        {
            get
            {
                if (RadioGroup != null)
                {
                    return RadioGroup.SelectedValue;
                }

                if (FormItem != null)
                {
                    return FormItem.Value;
                }

                return SelectedValue;
            }
        }

        protected string EffectiveName => !string.IsNullOrWhiteSpace(Name)
            ? Name
            : RadioGroup?.Name;

        protected RadioSize EffectiveSize
        {
            get
            {
                if (RadioGroup != null)
                {
                    return RadioGroup.EffectiveSize;
                }

                if (Size != RadioSize.Default)
                {
                    return ResolveRadioSize(Size);
                }

                return ResolveRadioSize(FormItem?.Size ?? FormItem?.Form?.EffectiveSize);
            }
        }

        protected string CheckedClass => IsChecked ? "is-checked" : string.Empty;

        protected string AriaChecked => IsChecked ? "true" : "false";

        protected string AriaDisabled => EffectiveDisabled ? "true" : "false";

        protected string RadioSizeClass => EffectiveSize == RadioSize.Default
            ? string.Empty
            : $"el-radio--{EffectiveSize.ToString().ToLowerInvariant()}";

        protected string RadioButtonSizeClass
        {
            get
            {
                if (EffectiveSize == RadioSize.Default)
                {
                    return RadioGroup == null ? string.Empty : "el-radio-button--default";
                }

                return $"el-radio-button--{EffectiveSize.ToString().ToLowerInvariant()}";
            }
        }

        internal int TabIndex
        {
            get
            {
                if (EffectiveDisabled)
                {
                    return -1;
                }

                if (RadioGroup == null)
                {
                    return 0;
                }

                return RadioGroup.GetTabIndex(this);
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (RadioGroup != null)
            {
                RadioGroup.RegisterRadio(this);
                if (TypeHelper.Equal(RadioGroup.SelectedValue, Value))
                {
                    Status = RadioStatus.Selected;
                    SetFieldValue(RadioGroup.SelectedValue, false);
                }
                else
                {
                    Status = RadioStatus.UnSelected;
                }
            }
            else if (TypeHelper.Equal(SelectedValue, Value))
            {
                Status = RadioStatus.Selected;
                SetFieldValue(SelectedValue, false);
            }
            else
            {
                Status = RadioStatus.UnSelected;
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            SelectedValue = TypeHelper.ChangeType<TValue>(value);
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
                SelectedValue = FormItem.OriginValue;
                SetFieldValue(SelectedValue, false);
            }
        }

        protected async Task ChangeRadio(MouseEventArgs e)
        {
            if (EffectiveDisabled)
            {
                return;
            }
            if (RadioGroup != null)
            {
                await RadioGroup.TrySetValueAsync(Value, !SelectedValueChanged.HasDelegate);
                return;
            }

            var oldStatus = IsChecked ? RadioStatus.Selected : Status;
            var newStatus = RadioStatus.Selected;
            if (StatusChanging.HasDelegate)
            {
                var arg = new ElementChangeEventArgs<RadioStatus>
                {
                    OldValue = oldStatus,
                    NewValue = newStatus
                };
                await StatusChanging.InvokeAsync(arg);
                if (arg.DisallowChange)
                {
                    return;
                }
            }

            var changed = !TypeHelper.Equal(SelectedValue, Value);
            if (changed)
            {
                SelectedValue = Value;
            }

            SetFieldValue(SelectedValue, true);
            Status = newStatus;
            if (changed && StatusChanged.HasDelegate)
            {
                await StatusChanged.InvokeAsync(newStatus);
            }
            if (SelectedValueChanged.HasDelegate)
            {
                await SelectedValueChanged.InvokeAsync(SelectedValue);
            }
        }

        protected async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (EffectiveDisabled)
            {
                return;
            }

            if (RadioGroup == null)
            {
                if (e.Key == " " || e.Key == "Spacebar" || e.Key == "Enter")
                {
                    await ChangeRadio(null);
                }
                return;
            }

            switch (e.Key)
            {
                case " ":
                case "Space":
                case "Spacebar":
                case "Enter":
                    await RadioGroup.TrySetValueAsync(Value, !SelectedValueChanged.HasDelegate);
                    break;
                case "ArrowRight":
                case "ArrowDown":
                    await RadioGroup.MoveSelectionAsync(this, 1);
                    break;
                case "ArrowLeft":
                case "ArrowUp":
                    await RadioGroup.MoveSelectionAsync(this, -1);
                    break;
                case "Home":
                    await RadioGroup.SelectEdgeAsync(selectFirst: true);
                    break;
                case "End":
                    await RadioGroup.SelectEdgeAsync(selectFirst: false);
                    break;
            }
        }

        internal ValueTask FocusAsync()
        {
            return radioElement.Dom(JSRuntime).FocusAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            var oldStatus = Status;
            Status = TypeHelper.Equal(SelectedValue, Value)
                ? RadioStatus.Selected
                : RadioStatus.UnSelected;
            if (oldStatus != Status && StatusChanged.HasDelegate)
            {
                RequireRender = true;
                _ = StatusChanged.InvokeAsync(Status);
            }
        }

        protected override bool ShouldRender()
        {
            return true;
        }

        private RadioSize ResolveRadioSize(InputSize? size)
        {
            return size switch
            {
                InputSize.Large => RadioSize.Medium,
                InputSize.Small => RadioSize.Small,
                _ => ResolveRadioSize(Size)
            };
        }

        public override void Dispose()
        {
            RadioGroup?.UnregisterRadio(this);
            base.Dispose();
        }
    }
}
