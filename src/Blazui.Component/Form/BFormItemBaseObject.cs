using Blazui.Component.Form.ValidationRules;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Form
{
    public abstract class BFormItemBaseObject : BComponentBase
    {
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
            _ = Task.Delay(10).ContinueWith((task) =>
            {
                IsShowing = false;
                InvokeAsync(StateHasChanged);
            });
        }
        public abstract void Validate();
        public abstract void Reset();
    }
}
