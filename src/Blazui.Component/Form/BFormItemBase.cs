using Blazui.Component.Form.ValidationRules;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Form
{
    public class BFormItemBase<TValue> : BFormItemBaseObject
    {
        /// <summary>
        /// 是否隐藏该表单项
        /// </summary>
        [Parameter]
        public bool IsHidden { get; set; }
        internal TValue OriginValue { get; set; }
        public TValue Value { get; set; }

        internal HtmlPropertyBuilder formItemCssBuilder;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            formItemCssBuilder = HtmlPropertyBuilder.Create().AddIf(IsHidden, "display:none");

            if (!Form.Values.Any())
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(Name))
            {
                return;
            }
            if (OriginValueHasSet)
            {
                return;
            }
            OriginValueHasSet = true;
            Form.Values.TryGetValue(Name, out var value);
            OriginValue = (TValue)value;
            Value = (TValue)value;
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
