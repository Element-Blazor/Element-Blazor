

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BRadioBase<TValue> : BFieldComponentBase<TValue>
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// 每个单选组件自己的值
        /// </summary>
        [Parameter]
        public TValue Value { get; set; }

        /// <summary>
        /// 多个单选组件选择的值
        /// </summary>
        [Parameter]
        public TValue SelectedValue { get; set; }

        /// <summary>
        /// 当选择状态发生改变时触发
        /// </summary>
        [Parameter]
        public EventCallback<RadioStatus> StatusChanged { get; set; }

        /// <summary>
        /// 当选择的值发生改变时触发
        /// </summary>
        [Parameter]
        public EventCallback<TValue> SelectedValueChanged { get; set; }

        [Parameter]
        public EventCallback<BChangeEventArgs<RadioStatus>> StatusChanging { get; set; }

        [CascadingParameter]
        public BRadioGroup<TValue> RadioGroup { get; set; }

        [Parameter]
        public RadioStatus Status { get; set; } = RadioStatus.UnSelected;

        [Parameter]
        public RadioSize Size { get; set; }
        [Parameter]
        public bool IsBordered { get; set; }
        [Parameter]
        public bool IsDisabled { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (RadioGroup != null)
            {
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
            else
            {
                if (TypeHelper.Equal(SelectedValue, Value))
                {
                    Status = RadioStatus.Selected;
                    SetFieldValue(SelectedValue, false);
                }
                else
                {
                    Status = RadioStatus.UnSelected;
                }
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
            if (IsDisabled)
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
                var arg = new BChangeEventArgs<RadioStatus>();
                arg.OldValue = Status;
                arg.NewValue = newStatus;
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

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            var oldStatus = Status;
            if (TypeHelper.Equal(SelectedValue, Value))
            {
                Status = RadioStatus.Selected;
            }
            else
            {
                Status = RadioStatus.UnSelected;
            }
            if (oldStatus != Status)
            {
                if (StatusChanged.HasDelegate)
                {
                    RequireRender = true;
                    _ = StatusChanged.InvokeAsync(Status);
                }
            }
        }

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
