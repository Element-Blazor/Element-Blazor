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
        internal HtmlPropertyBuilder wrapperClsBuilder;
        /// <summary>
        /// 输入框类型
        /// </summary>
        [Parameter]
        public InputType Type { get; set; } = InputType.Text;

        /// <summary>
        /// 输入框尺寸
        /// </summary>
        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        /// <summary>
        /// 如何格式化
        /// </summary>
        [Parameter]
        public virtual Func<TValue, string> Formatter { get; set; } = v => Convert.ToString(v);

        /// <summary>
        /// 输入框的值
        /// </summary>
        [Parameter]
        public TValue Value { get; set; }

        /// <summary>
        /// 值改变时触发
        /// </summary>
        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        /// <summary>
        /// Placeholder
        /// </summary>
        [Parameter]
        public virtual string Placeholder { get; set; } = "请输入内容";

        /// <summary>
        /// 是否禁用输入框
        /// </summary>
        [Parameter]
        public bool IsDisabled { get; set; } = false;

        /// <summary>
        /// 是否可清空
        /// </summary>
        [Parameter]
        public bool IsClearable { get; set; } = false;

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
            wrapperClsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-input", Cls, $"el-input--{Size.ToString().ToLower()}")
                .AddIf(IsClearable || !string.IsNullOrWhiteSpace(SuffixIcon), "el-input--suffix")
                .AddIf(!string.IsNullOrWhiteSpace(PrefixIcon), "el-input--prefix")
                .AddIf(IsDisabled, "is-disabled");
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
