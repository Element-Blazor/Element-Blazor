using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;

namespace Element
{
    public partial class BInput<TValue> : IDisposable
    {
        internal HtmlPropertyBuilder wrapperClsBuilder;
        /// <summary>
        /// 输入框类型
        /// </summary>
        [Parameter]
        public InputType Type { get; set; } = InputType.Text;

        /// <summary>
        /// 当输入值错误时是否抛出异常
        /// </summary>
        protected virtual bool ThrowOnInvalidValue { get; } = false;

        /// <summary>
        /// 在前方加入元素
        /// </summary>
        [Parameter]
        public RenderFragment Prepend { get; set; }

        /// <summary>
        /// 是否启用清空按钮
        /// </summary>
        [Parameter]
        public bool IsClearButtonClick { get; set; }

        [Parameter]
        public EventCallback<bool> EnableClearButtonChanged { get; set; }

        /// <summary>
        /// 按钮背景图片
        /// </summary>
        [Parameter]
        public string Image { get; set; }

        /// <summary>
        /// 输入框尺寸
        /// </summary>
        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;
        [Parameter]
        public bool EnableClearButton { get; set; }

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

        internal ElementReference Content { get; set; }

        /// <summary>
        /// HTML 元素引用
        /// </summary>
        public ElementReference InputElement { get; set; }

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
            try
            {
                Value = (TValue)TypeHelper.ChangeType(input.Value, typeof(TValue));
            }
            catch (FormatException e)
            {
                if (ThrowOnInvalidValue)
                {
                    throw e;
                }
                Value = default;
            }
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            SetFieldValue(Value, true);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (IsDisabled)
            {
                await InputElement.Dom(JSRuntime).SetDisabledAsync(IsDisabled);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRender)
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
                .AddIf(IsDisabled, "is-disabled")
                .AddIf(Prepend != null, "el-input-group", "el-input-group--append", "el-input-group--prepend");
            if (FormItem == null)
            {
                return;
            }
            if (FormItem.OriginValueHasRendered)
            {
                SetFieldValue(Value ?? FormItem.Value, false);
                Value = Value ?? FormItem.OriginValue;
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
