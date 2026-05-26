
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public abstract class ElFormItemObject : ElementComponentBase
    {
        private IList<IValidationRule> parameterRules = new List<IValidationRule>();
        private IList<IValidationRule> resolvedRules = new List<IValidationRule>();
        /// <summary>
        /// 是否应用样式，如果不应用，则该组件本身不生成任何 HTML
        /// </summary>
        [Parameter]
        public bool ApplyStyle { get; set; } = true;
        /// <summary>
        /// 初始值是否已渲染
        /// </summary>
        public bool OriginValueHasRendered { get; set; } = false;
        /// <summary>
        /// 初始值是否已设置
        /// </summary>
        internal bool OriginValueHasSet { get; set; } = false;

        internal string FieldId => string.IsNullOrWhiteSpace(Name) ? null : $"{Name}-form-item";

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string For { get; set; }

        /// <summary>
        /// 设置字段 Label 为图片地址
        /// </summary>
        [Parameter]
        public string Image { get; set; }

        /// <summary>
        /// 标签宽度
        /// </summary>
        [Parameter]
        public object LabelWidth { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Prop
        {
            get => Name;
            set => Name = value;
        }

        [Parameter]
        public bool Required { get; set; }

        [Parameter]
        public bool IsRequired
        {
            get => Required;
            set => Required = value;
        }

        [Parameter]
        public string Error { get; set; }

        [Parameter]
        public string ValidateStatus { get; set; }

        [Parameter]
        public string LabelPosition { get; set; }

        [Parameter]
        public bool? InlineMessage { get; set; }

        [Parameter]
        public bool ShowMessage { get; set; } = true;

        [Parameter]
        public InputSize? Size { get; set; }

        [Parameter]
        public string RequiredMessage { get; set; }
        internal bool IsShowing { get; set; } = true;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// 占位符
        /// </summary>
        [Parameter]
        public string Placeholder { get; set; }
        [CascadingParameter]
        public ElForm Form { get; set; }

        [Parameter]
        public IList<IValidationRule> Rules
        {
            get => resolvedRules;
            set
            {
                parameterRules = value ?? new List<IValidationRule>();
                resolvedRules = parameterRules;
            }
        }
        public ValidationResult ValidationResult { get; protected set; }

        protected override void OnInitialized()
        {
            Form?.RegisterField(this);
            ResolveRules();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ResolveRules();
        }

        internal void RefreshRules()
        {
            ResolveRules();
        }

        private void ResolveRules()
        {
            if (Form == null)
            {
                return;
            }

            var rules = new List<IValidationRule>();
            var validation = Form.Validations?.FirstOrDefault(x => x.Name == Name);
            if (validation != null)
            {
                rules.AddRange(validation.Rules);
            }
            if (Form.Rules != null && !string.IsNullOrWhiteSpace(Name) && Form.Rules.TryGetValue(Name, out var formRules))
            {
                rules.AddRange(formRules);
            }
            if (parameterRules != null)
            {
                rules.AddRange(parameterRules.Where(x => x != null && !rules.Contains(x)));
            }
            if (Required && !rules.OfType<RequiredRule>().Any())
            {
                var requiredRule = new RequiredRule
                {
                    ErrorMessage = RequiredMessage ?? $"请确认{Label}"
                };
                rules.Add(requiredRule);
            }
            resolvedRules = rules;
        }

        internal abstract object CurrentValue { get; }

        internal abstract void SetInitialValue(object value, bool resetCurrentValue);

        internal void ShowErrorMessage()
        {
            if (ValidationResult == null || ValidationResult.IsValid)
            {
                return;
            }
            IsShowing = true;
            _ = Task.Delay(100).ContinueWith((task) =>
            {
                IsShowing = false;
                MarkAsRequireRender();
                InvokeAsync(StateHasChanged);
            });
        }
        public virtual void Validate()
        {

        }


        public virtual void Reset()
        {

        }

        public virtual void ClearValidate()
        {
            ValidationResult = null;
            ValidateStatus = string.Empty;
            IsShowing = true;
            MarkAsRequireRender();
        }

        public override void Dispose()
        {
            Form?.UnregisterField(this);
            base.Dispose();
        }
    }
}
