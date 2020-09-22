using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class BFormItem<TValue> : BFormItemObject
    {
        /// <summary>
        /// 是否隐藏该表单项
        /// </summary>
        [Parameter]
        public bool IsHidden { get; set; }
        public TValue OriginValue { get; set; }
        public TValue Value { get; set; }

        internal HtmlPropertyBuilder formItemCssBuilder;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            formItemCssBuilder = HtmlPropertyBuilder.CreateCssStyleBuilder().AddIf(IsHidden, "display:none")
                .Add(Style);

            if (!Form.Values.Any())
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(Name))
            {
                if (GetType() != typeof(BFormActionItem))
                {
                    ExceptionHelper.Throw(ExceptionHelper.FormItemMustHaveName, "BFormItem 组件必须指定 Name 属性");
                }
                return;
            }
            if (OriginValueHasSet)
            {
                return;
            }
            OriginValueHasSet = true;
            if (Form.Values.TryGetValue(Name, out var value))
            {
                OriginValue = (TValue)value;
                Console.WriteLine($"设置 FormItem {Name} 值:" + value);
                Value = (TValue)value;
            }
            OriginValueHasRendered = false;
        }

        public override void Validate()
        {
            ValidationResult = new ValidationResult();
            foreach (var item in Rules)
            {
                if (item.Validate(Value))
                {
                    continue;
                }
                ValidationResult.ErrorMessages.Add(item.ErrorMessage);
            }
            ValidationResult.IsValid = !ValidationResult.ErrorMessages.Any();
            StateHasChanged();
        }

        public event Action<object, bool> OnReset;

        public override void Reset()
        {
            Console.WriteLine($"设置 FormItem {Name} 值:" + OriginValue);
            Value = OriginValue;
            if (OnReset != null)
            {
                RequireRender = true;
                OnReset(OriginValue, true);
            }
            ValidationResult = null;
        }
    }
}
