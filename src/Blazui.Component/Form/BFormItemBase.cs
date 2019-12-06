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
        public TValue OriginValue { get; set; }

        /// <summary>
        /// 初始值是否已渲染
        /// </summary>
        public bool OriginValueRendered { get; set; }
        public TValue Value { get; set; }

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
                OnReset(OriginValue, true);
            }
            ValidationResult = null;
        }
    }
}
