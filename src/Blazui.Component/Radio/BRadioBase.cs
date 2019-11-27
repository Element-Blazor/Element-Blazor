using Blazui.Component.EventArgs;
using Blazui.Component.Form;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Radio
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

        [Parameter]
        public EventCallback<RadioStatus> StatusChanged { get; set; }

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
                    SetFieldValue(RadioGroup.SelectedValue);
                }
            }
            else
            {
                if (TypeHelper.Equal(SelectedValue, Value))
                {
                    Status = RadioStatus.Selected;
                    SetFieldValue(SelectedValue);
                }
            }
        }

        protected override void FormItem_OnReset()
        {
            SelectedValue = default;
        }

        protected void OnRadioChanged(MouseEventArgs e)
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
                SetFieldValue(SelectedValue);
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
    }
}
