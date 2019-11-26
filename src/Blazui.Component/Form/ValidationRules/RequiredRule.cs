using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Form.ValidationRules
{
    public class RequiredRule : IValidationRule
    {
        public string ErrorMessage { get; set; }

        public bool Validate(object value)
        {
            if (value == null)
            {
                return false;
            }
            if (value is IEnumerable enumerable)
            {
                if (!enumerable.Cast<object>().Any())
                {
                    return false;
                }
            }
            var str = value?.ToString();
            return !string.IsNullOrWhiteSpace(str);
        }
    }
}
