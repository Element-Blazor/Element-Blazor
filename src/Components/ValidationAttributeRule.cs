using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Element
{
    public class ValidationAttributeRule : IValidationRule
    {
        public ValidationAttributeRule(ValidationAttribute attribute, Func<ValidationContext> validationContextFactory = null)
        {
            Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            this.validationContextFactory = validationContextFactory;
        }

        private readonly Func<ValidationContext> validationContextFactory;

        public ValidationAttribute Attribute { get; }

        public string ErrorMessage { get; set; }

        public bool Validate(object value)
        {
            var context = validationContextFactory?.Invoke() ?? new ValidationContext(new object());
            var result = Attribute.GetValidationResult(value, context);
            if (result == System.ComponentModel.DataAnnotations.ValidationResult.Success)
            {
                return true;
            }

            ErrorMessage = !string.IsNullOrWhiteSpace(result?.ErrorMessage)
                ? result.ErrorMessage
                : ResolveErrorMessage(value);
            return false;
        }

        private string ResolveErrorMessage(object value)
        {
            if (!string.IsNullOrWhiteSpace(ErrorMessage))
            {
                return ErrorMessage;
            }

            try
            {
                return Attribute.FormatErrorMessage(validationContextFactory?.Invoke()?.DisplayName ?? string.Empty);
            }
            catch (InvalidOperationException)
            {
                return Convert.ToString(Attribute.ErrorMessage, CultureInfo.CurrentCulture) ?? "该字段验证未通过";
            }
        }
    }
}
