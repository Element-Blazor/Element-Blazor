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

        private TValue rawValue;
        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }
        [Parameter]
        public EventCallback<Status> StatusChanged { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            rawValue = Value;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (CheckBoxGroup != null)
            {
                if (CheckBoxGroup.SelectedItems.Contains(Value))
                {
                    Status = Status.Checked;
                    return;
                }
            }
        }

        //private void CheckBoxGroup_FormItem_OnReset(object value, bool requireRerender)
        //{
        //    if (CheckBoxGroup.SelectedItems.Contains(Value))
        //    {
        //        Status = Status.Checked;
        //        return;
        //    }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            if (CheckBoxGroup != null)
            {
                if (CheckBoxGroup.SelectedItems.Contains(TypeHelper.ChangeType<TValue>(value)))
                {
                    Status = Status.Checked;
                    return;
                }
            }
            CheckBoxGroup?.SelectedItems?.Remove(Value);
            Status = Status.UnChecked;
        }

        protected void ChangeStatus(ChangeEventArgs uIMouseEvent)
        {
            if (IsDisabled)
            {
                return;
            }
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

            var checkBoxValue = Value;
            if (newValue.Status == Status.Checked)
            {
                CheckBoxGroup?.SelectedItems?.Add(Value);
            }
            else
            {
                CheckBoxGroup?.SelectedItems?.Remove(Value);
                checkBoxValue = default;
            }
            Status = newValue.Status;

            //有 CheckBoxGroup 时，视整个CheckBoxGroup为一个字段
            if (CheckBoxGroup == null)
            {
                SetFieldValue(checkBoxValue, true);
            }
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(checkBoxValue);
            }
            if (StatusChanged.HasDelegate)
            {
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
