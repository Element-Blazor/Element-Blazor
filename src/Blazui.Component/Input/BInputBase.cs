using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component.CheckBox;
using Blazui.Component.EventArgs;
using Blazui.Component.Dom;
using Microsoft.JSInterop;
using Blazui.Component.Form;
using Microsoft.AspNetCore.Components.Web;

namespace Blazui.Component.Input
{
    public class BInputBase<TValue> : BFieldComponentBase<TValue>, IDisposable
    {
        [Parameter]
        public InputType Type { get; set; } = InputType.Text;

        [Parameter]
        public virtual Func<TValue, string> Formatter { get; set; } = v => Convert.ToString(v);

        [Parameter]
        public bool IsClearButtonClick { get; set; }

        [Parameter]
        public EventCallback<bool> IsClearButtonClickChanged { get; set; }
        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter] public virtual string Placeholder { get; set; } = "请输入内容";
        [Parameter] public bool IsDisabled { get; set; } = false;
        [Parameter] public bool IsClearable { get; set; } = false;

        internal bool IsFocus { get; set; }

        internal bool IsMouseOver { get; set; }

        [Parameter]
        public virtual string PrefixIcon { get; set; }

        [Parameter]
        public virtual EventCallback<MouseEventArgs> OnSuffixIconClick { get; set; }
        [Parameter]
        public virtual EventCallback<MouseEventArgs> OnPrefixIconClick { get; set; }
        [Parameter]
        public virtual EventCallback<MouseEventArgs> OnInputClick { get; set; }
        [Parameter]
        public virtual EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter]
        public virtual string SuffixIcon { get; set; }
        [Parameter]
        public virtual string AdditionalClearIcon { get; set; }

        [Parameter]
        public virtual string Cls { get; set; }

        internal ElementReference Content { get; set; }
        internal ElementReference InputElement { get; set; }

        protected void ClearOnClick()
        {
            IsClearButtonClick = true;
            if (IsClearButtonClickChanged.HasDelegate)
            {
                _ = IsClearButtonClickChanged.InvokeAsync(IsClearButtonClick);
            }
            Value = default;
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            SetFieldValue(Value);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SetInitilizeFieldValue(Value);
        }

        protected virtual Task OnFocusAsync()
        {
            if (Value == null)
            {
                Value = default;
            }
            IsFocus = true;
            return Task.CompletedTask;
        }

        protected void OnChangeEventArgs(ChangeEventArgs input)
        {
            Value = (TValue)TypeHelper.ChangeType(input.Value, typeof(TValue));
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            SetFieldValue(Value);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsDisabled)
            {
                await InputElement.Dom(JSRuntime).SetDisabledAsync(IsDisabled);
            }

            if (FormItem != null)
            {
                FormItem.OnReset += FormItem_OnReset;
            }
        }

        protected override void FormItem_OnReset()
        {
            this.Value = default;
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
        }
    }
}
