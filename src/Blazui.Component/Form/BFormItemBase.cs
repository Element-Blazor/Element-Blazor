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
        public TValue Value { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (!Form.Values.Any())
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(Name))
            {
                return;
            }
            Form.Values.TryGetValue(Name, out var value);
            OriginValue = (TValue)value;
            Value = (TValue)value;
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
                OnReset(OriginValue, true);
            }
            ValidationResult = null;
        }
    }
}
