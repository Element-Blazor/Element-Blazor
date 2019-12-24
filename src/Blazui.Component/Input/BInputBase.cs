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
        public bool EnableClearButton { get; set; }

        [Parameter]
        public EventCallback<bool> EnableClearButtonChanged { get; set; }
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
            EnableClearButton = true;
            if (EnableClearButtonChanged.HasDelegate)
            {
                _ = EnableClearButtonChanged.InvokeAsync(EnableClearButton);
            }
            Value = default;
            Console.WriteLine($"ClearOnClick 设置输入框 {Name} 值:" + Value);
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            SetFieldValue(Value, true);
        }

        protected virtual Task OnFocusAsync()
        {
            if (Value == null)
            {
                Value = default;
            }
            Console.WriteLine($"OnFocusAsync 设置输入框 {Name} 值:" + Value);
            IsFocus = true;
            return Task.CompletedTask;
        }

        protected virtual void OnChangeEventArgs(ChangeEventArgs input)
        {
            Value = (TValue)TypeHelper.ChangeType(input.Value, typeof(TValue));
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            SetFieldValue(Value, true);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsDisabled)
            {
                await InputElement.Dom(JSRuntime).SetDisabledAsync(IsDisabled);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            if (value == null)
            {
                Value = default;
            }
            else
            {
                Value = (TValue)TypeHelper.ChangeType(value, typeof(TValue));
            }
            Console.WriteLine($"FormItem_OnReset 设置输入框 {Name} 值:" + Value);
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            else
            {
                StateHasChanged();
            }
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
                Value = FormItem.OriginValue;
            }
            Console.WriteLine($"OnParametersSet 设置输入框 {Name} 值:" + Value);
            SetFieldValue(Value, false);
        }

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
