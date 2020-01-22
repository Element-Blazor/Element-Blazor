using Blazui.Component.EventArgs;
using Blazui.Component;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.CheckBox
{
    public class BCheckBoxBase<TValue> : BFieldComponentBase<TValue>
    {
        internal string _isChecked = string.Empty;
        internal string _isIndeterminate = string.Empty;
        internal string isDisabled;

        /// <summary>
        /// 如果该 <seealso cref="BCheckBoxBase{TValue}"/> 在 <seealso cref="BCheckBoxGroup{TValue}"/> 中，则此属性获取 <seealso cref="BCheckBoxGroup{TValue}"/> 的值
        /// </summary>
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
                Status = Status.UnChecked;
                return;
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
            Value = FormItem.OriginValue;
            if (TypeHelper.Equal(Value, default))
            {
                Status = Status.Checked;
            }
            else
            {
                Status = Status.UnChecked;
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            RequireRender = true;
            if (CheckBoxGroup != null)
            {
                if (CheckBoxGroup.SelectedItems.Contains(TypeHelper.ChangeType<TValue>(value)))
                {
                    Status = Status.Checked;
                }
                else
                {
                    Status = Status.UnChecked;
                }
                if (StatusChanged.HasDelegate)
                {
                    _ = StatusChanged.InvokeAsync(Status);
                }
                else
                {
                    CheckBoxGroup.MarkAsRequireRender();
                }
                return;
            }

            if (TypeHelper.Equal(Value, TypeHelper.ChangeType<TValue>(value)))
            {
                Status = Status.Checked;
            }
            else
            {
                Status = Status.UnChecked;
            }
            if (StatusChanged.HasDelegate)
            {
                _ = StatusChanged.InvokeAsync(Status);
            }
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
            RequireRender = true;
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

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
