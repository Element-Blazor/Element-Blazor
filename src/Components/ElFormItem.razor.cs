using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElFormItem<TValue> : ElFormItemObject
    {
        /// <summary>
        /// 是否隐藏该表单项
        /// </summary>
        [Parameter]
        public bool IsHidden { get; set; }
        public TValue OriginValue { get; set; }
        private TValue value;
        public TValue Value
        {
            get => value;
            set
            {
                this.value = value;
                NotifyValueChanged(value, validate: false);
            }
        }

        internal override object CurrentValue => Value;

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
                if (GetType() != typeof(ElFormActionItem))
                {
                    ExceptionHelper.Throw(ExceptionHelper.FormItemMustHaveName, "ElFormItem 组件必须指定 Prop 或 Name 属性");
                }
                return;
            }
            if (OriginValueHasSet)
            {
                return;
            }
            if (Form.Values.TryGetValue(Name, out var value))
            {
                SetInitialValue(value, resetCurrentValue: true);
            }
        }

        internal override void SetInitialValue(object value, bool resetCurrentValue)
        {
            if (value == null)
            {
                OriginValue = default;
            }
            else if (value is TValue typedValue)
            {
                OriginValue = typedValue;
            }
            else
            {
                OriginValue = (TValue)TypeHelper.ChangeType(value, typeof(TValue));
            }
            if (resetCurrentValue || !OriginValueHasSet)
            {
                this.value = OriginValue;
            }
            OriginValueHasSet = true;
            OriginValueHasRendered = false;
        }

        public override void Validate()
        {
            if (!string.IsNullOrWhiteSpace(Error))
            {
                ValidateStatus = "error";
                ValidationResult = new ValidationResult();
                ValidationResult.ErrorMessages.Add(Error);
                ValidationResult.IsValid = false;
                if (Form.OnValidate.HasDelegate)
                {
                    _ = Form.OnValidate.InvokeAsync(new FormValidateEventArgs
                    {
                        Prop = Name,
                        IsValid = false,
                        Message = Error
                    });
                }
                StateHasChanged();
                return;
            }
            ValidationResult = new ValidationResult();
            foreach (var item in Rules)
            {
                if (item == null)
                {
                    continue;
                }
                if (item.Validate(Value))
                {
                    continue;
                }
                ValidationResult.ErrorMessages.Add(item.ErrorMessage);
            }
            ValidationResult.IsValid = !ValidationResult.ErrorMessages.Any();
            ValidateStatus = ValidationResult.IsValid ? "success" : "error";
            if (Form.OnValidate.HasDelegate)
            {
                _ = Form.OnValidate.InvokeAsync(new FormValidateEventArgs
                {
                    Prop = Name,
                    IsValid = ValidationResult.IsValid,
                    Message = ValidationResult.ErrorMessages.FirstOrDefault() ?? string.Empty
                });
            }
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
            ValidateStatus = string.Empty;
        }
    }
}
