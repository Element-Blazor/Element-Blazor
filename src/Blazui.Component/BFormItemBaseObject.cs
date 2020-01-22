using Blazui.Component.ValidationRules;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public abstract class BFormItemBaseObject : BComponentBase
    {
        /// <summary>
        /// 初始值是否已渲染
        /// </summary>
        public bool OriginValueHasRendered { get; set; } = false;
        /// <summary>
        /// 初始值是否已设置
        /// </summary>
        internal bool OriginValueHasSet { get; set; } = false;
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public bool IsRequired { get; set; }

        [Parameter]
        public string RequiredMessage { get; set; }
        internal bool IsShowing { get; set; } = true;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [CascadingParameter]
        public BForm Form { get; set; }

        public IList<IValidationRule> Rules { get; set; } = new List<IValidationRule>();
        public ValidationResult ValidationResult { get; protected set; }

        protected override void OnInitialized()
        {
            Form.Items.Add(this);
            var validation = Form.Validations.FirstOrDefault(x => x.Name == Name);
            if (validation != null)
            {
                Rules = validation.Rules;
            }
            if (IsRequired && !Rules.OfType<RequiredRule>().Any())
            {
                var requiredRule = new RequiredRule();
                requiredRule.ErrorMessage = RequiredMessage ?? $"请确认{Label}";
                Rules.Add(requiredRule);
            }
        }

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
        public abstract void Validate();
        public abstract void Reset();
    }
}
