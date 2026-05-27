

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElCheckbox<TValue> : ElementFieldComponentBase<TValue>
    {
        internal string _isChecked = string.Empty;
        internal string _isIndeterminate = string.Empty;
        internal string isDisabled;

        /// <summary>
        /// 如果该 <seealso cref="ElCheckbox{TValue}"/> 在 <seealso cref="ElCheckboxGroup{TValue}"/> 中，则此属性获取 <seealso cref="ElCheckboxGroup{TValue}"/> 的值
        /// </summary>
        [CascadingParameter]
        public ElCheckboxGroup<TValue> CheckBoxGroup { get; set; }

        [Parameter]
        public Status Status { get; set; }

        [Parameter]
        public bool Indeterminate
        {
            get => Status == Status.Indeterminate;
            set
            {
                if (value)
                {
                    Status = Status.Indeterminate;
                }
                else if (Status == Status.Indeterminate)
                {
                    Status = Status.UnChecked;
                }
            }
        }

        [Parameter]
        public TValue Value { get; set; }
        [Parameter]
        public TValue Model
        {
            get => Value;
            set => Value = value;
        }
        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }
        [Parameter]
        public EventCallback<Status> StatusChanged { get; set; }

        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        [Parameter]
        public bool Border { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
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

        protected async Task ChangeStatus(ChangeEventArgs uIMouseEvent)
        {
            if (EffectiveDisabled)
            {
                return;
            }
            if (CheckBoxGroup != null)
            {
                await CheckBoxGroup.TryToggleAsync(Value);
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
            if (newValue.Status != Status.Checked)
            {
                checkBoxValue = default;
            }
            Status = newValue.Status;

            SetFieldValue(checkBoxValue, true);
            RequireRender = true;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(checkBoxValue);
            }
            if (StatusChanged.HasDelegate)
            {
                await StatusChanged.InvokeAsync(newValue.Status);
            }
        }

        [Parameter]
        public bool IsDisabled
        {
            get
            {
                return isDisabled == "is-disabled" || (CheckBoxGroup?.IsLimitDisabled(Value) ?? false) || (FormItem?.Form?.Disabled ?? false);
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
        public bool Disabled
        {
            get => IsDisabled;
            set => IsDisabled = value;
        }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        internal bool EffectiveDisabled => IsDisabled;

        internal InputSize EffectiveSize => CheckBoxGroup == null
            ? Size == InputSize.Normal
                ? FormItem?.Size ?? FormItem?.Form?.EffectiveSize ?? ResolveInputSize(InputSize.Normal)
                : Size
            : CheckBoxGroup.EffectiveSize;

        internal string SizeClass
        {
            get
            {
                var size = EffectiveSize switch
                {
                    InputSize.Large => "large",
                    InputSize.Small => "small",
                    _ => null
                };
                return size == null ? string.Empty : $"el-checkbox--{size}";
            }
        }

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
