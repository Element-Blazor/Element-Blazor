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
        private ElementReference radioElement;

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
        public bool IsDisabled { get; set; }

        internal bool EffectiveDisabled => IsDisabled || (RadioGroup?.EffectiveDisabled ?? false) || (FormItem?.Form?.Disabled ?? false);

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

        protected void ChangeRadio(MouseEventArgs e)
        {
            if (EffectiveDisabled)
            {
                return;
            }
            if (RadioGroup != null)
            {
                _ = RadioGroup.TrySetValueAsync(Value, !SelectedValueChanged.HasDelegate);
                return;
            }
            var newStatus = Status == RadioStatus.Selected ? RadioStatus.UnSelected : RadioStatus.Selected;
            if (StatusChanging.HasDelegate)
            {
                var arg = new ElementChangeEventArgs<RadioStatus>
                {
                    OldValue = Status,
                    NewValue = newStatus
                };
                StatusChanging.InvokeAsync(arg).Wait();
                if (arg.DisallowChange)
                {
                    return;
                }
            }

            if (newStatus == RadioStatus.Selected && !TypeHelper.Equal(SelectedValue, Value))
            {
                SelectedValue = Value;
            }

            if (RadioGroup == null)
            {
                SetFieldValue(SelectedValue, true);
            }
            if (StatusChanged.HasDelegate)
            {
                _ = StatusChanged.InvokeAsync(newStatus);
            }
            if (SelectedValueChanged.HasDelegate)
            {
                _ = SelectedValueChanged.InvokeAsync(SelectedValue);
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
                    ChangeRadio(null);
                }
                return;
            }

            switch (e.Key)
            {
                case " ":
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

        public override void Dispose()
        {
            RadioGroup?.UnregisterRadio(this);
            base.Dispose();
        }
    }
}
