using Blazui.Component.EventArgs;
using Blazui.Component.Form;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.CheckBox
{
    public class SimpleCheckBoxBase<TValue> : BFieldComponentBase<TValue>
    {
        protected string _isChecked = string.Empty;
        protected string _isIndeterminate = string.Empty;
        protected string isDisabled;

        [CascadingParameter]
        public BCheckBoxGroup<TValue> CheckBoxGroup { get; set; }

        [Parameter]
        public Status Status { get; set; }

        private TValue originValue;
        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }
        [Parameter]
        public EventCallback<Status> StatusChanged { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            originValue = Value;
        }

        protected override void FormItem_OnReset()
        {
            CheckBoxGroup?.SelectedItems?.Remove(Value);
            Value = default;
            Status = Status.UnChecked;
        }

        protected void ChangeStatus(ChangeEventArgs uIMouseEvent)
        {
            if (IsDisabled)
            {
                return;
            }
            var oldValue = new CheckBoxValue()
            {
                Status = Status
            };
            var newValue = new CheckBoxValue();
            switch (Status)
            {
                case Status.UnChecked:
                    newValue.Status = Status.Checked;
                    break;
                case Status.Checked:
                    newValue.Status = Status.UnChecked;
                    break;
                case Status.Indeterminate:
                    newValue.Status = Status.Checked;
                    break;
            }
            if (newValue.Status == Status.Checked)
            {
                CheckBoxGroup?.SelectedItems?.Add(Value);
                Value = originValue;
            }
            else
            {
                CheckBoxGroup?.SelectedItems?.Remove(Value);
                Value = default;
            }
            Status = newValue.Status;

            //有 CheckBoxGroup 时，视整个CheckBoxGroup为一个字段
            if (CheckBoxGroup == null)
            {
                SetFieldValue(Value);
            }
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            if (StatusChanged.HasDelegate)
            {
                var args = new BChangeEventArgs<CheckBoxValue>();
                args.OldValue = oldValue;
                args.NewValue = newValue;
                _ = StatusChanged.InvokeAsync(newValue.Status);
            }
        }

        [Parameter]
        public bool IsDisabled
        {
            get
            {
                return isDisabled == "is-disabled";
            }
            set
            {
                if (value)
                {
                    isDisabled = "is-disabled";
                }
                else
                {
                    isDisabled = null;
                }
            }
        }

        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
