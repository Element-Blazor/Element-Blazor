using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace Element.ComponentTests
{
    public class ElFormTests : BunitContext
    {
        public ElFormTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void ModelSupportsNestedPropValueBindingAndReset()
        {
            var model = new ProfileFormModel
            {
                User = new ProfileUser
                {
                    Name = "Alice"
                }
            };

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.Model, model)
                .AddChildContent<ElFormItem<string>>(item => item
                    .Add(x => x.Prop, "User.Name")
                    .Add(x => x.Label, "Name")
                    .AddChildContent<ElInput<string>>()));

            var input = cut.Find("input");
            Assert.Equal("Alice", input.GetAttribute("value"));

            input.Input("Bob");

            Assert.Equal("Bob", model.User.Name);
            Assert.Equal("Bob", cut.Instance.Values["User.Name"]);

            cut.InvokeAsync(() => cut.Instance.ResetFields("User.Name"));
            cut.Render();

            Assert.Equal("Alice", model.User.Name);
            Assert.Equal("Alice", cut.Find("input").GetAttribute("value"));
        }

        [Fact]
        public async Task ValidateUsesDataAnnotationsRulesFromModelAndClearValidate()
        {
            var model = new DataAnnotationFormModel();

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.Model, model)
                .AddChildContent<ElFormItem<string>>(item => item
                    .Add(x => x.Prop, nameof(DataAnnotationFormModel.Name))
                    .Add(x => x.Label, "Name")
                    .AddChildContent<ElInput<string>>()));

            Assert.False(await cut.InvokeAsync(() => cut.Instance.Validate()));
            cut.Render();

            var formItem = cut.Find(".el-form-item");
            Assert.Contains("is-error", formItem.ClassList);
            Assert.Contains("Name is required", cut.Markup);

            await cut.InvokeAsync(() => cut.Instance.ClearValidate(nameof(DataAnnotationFormModel.Name)));
            cut.Render();

            Assert.DoesNotContain("is-error", cut.Find(".el-form-item").ClassList);
            Assert.DoesNotContain("Name is required", cut.Markup);
        }

        [Fact]
        public async Task SubmitAwaitsAsyncValidationAndRaisesTypedSubmitEvents()
        {
            var invalidSubmitCount = 0;
            var typedInvalidCount = 0;
            ElFormSubmitEventArgs submitArgs = null;
            var rules = new Dictionary<string, IList<IValidationRule>>
            {
                [nameof(DataAnnotationFormModel.Name)] = new List<IValidationRule>
                {
                    new DelayedInvalidRule()
                }
            };

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.Rules, rules)
                .Add(x => x.OnInvalidSubmit, () => invalidSubmitCount++)
                .Add(x => x.OnInvalidSubmitForm, args =>
                {
                    typedInvalidCount++;
                    submitArgs = args;
                })
                .AddChildContent<ElFormItem<string>>(item => item
                    .Add(x => x.Prop, nameof(DataAnnotationFormModel.Name))
                    .Add(x => x.Label, "Name")
                    .AddChildContent<ElInput<string>>()));

            await cut.Find("form").SubmitAsync();
            cut.Render();

            Assert.Equal(1, invalidSubmitCount);
            Assert.Equal(1, typedInvalidCount);
            Assert.NotNull(submitArgs);
            Assert.False(submitArgs.IsValid);
            Assert.Same(cut.Instance, submitArgs.Form);
            Assert.Contains("Async invalid", cut.Markup);
        }

        [Fact]
        public async Task AsyncRulesRunOnlyThroughAsyncValidationPath()
        {
            var rule = new AsyncOnlyRule();
            var rules = new Dictionary<string, IList<IValidationRule>>
            {
                [nameof(DataAnnotationFormModel.Name)] = new List<IValidationRule>
                {
                    rule
                }
            };

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.Rules, rules)
                .AddChildContent<ElFormItem<string>>(item => item
                    .Add(x => x.Prop, nameof(DataAnnotationFormModel.Name))
                    .AddChildContent<ElInput<string>>()));

            Assert.True(await cut.InvokeAsync(() => cut.Instance.Validate()));
            Assert.Equal(0, rule.SyncCalls);
            Assert.Equal(0, rule.AsyncCalls);

            Assert.False(await cut.InvokeAsync(() => cut.Instance.ValidateAsync()));
            Assert.Equal(0, rule.SyncCalls);
            Assert.Equal(1, rule.AsyncCalls);
        }

        [Fact]
        public void SupportsLabelAndErrorSlots()
        {
            RenderFragment label = builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "custom-label");
                builder.AddContent(2, "Custom label");
                builder.CloseElement();
            };
            RenderFragment error = builder =>
            {
                builder.OpenElement(0, "strong");
                builder.AddAttribute(1, "class", "custom-error");
                builder.AddContent(2, "Custom error");
                builder.CloseElement();
            };

            var cut = Render<ElForm>(parameters => parameters
                .AddChildContent<ElFormItem<string>>(item => item
                    .Add(x => x.Prop, "Name")
                    .Add(x => x.LabelContent, label)
                    .Add(x => x.Error, "Ignored when slot is set")
                    .Add(x => x.ErrorContent, error)
                    .AddChildContent<ElInput<string>>()));

            cut.InvokeAsync(() => cut.Instance.Validate());
            cut.Render();

            Assert.Equal("Custom label", cut.Find(".custom-label").TextContent);
            Assert.Equal("Custom error", cut.Find(".custom-error").TextContent);
        }

        [Fact]
        public async Task GeneratedFormUsesDataAnnotationLabelsAndRequiredMetadata()
        {
            var model = new GeneratedAnnotationModel();

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.EntityType, typeof(GeneratedAnnotationModel))
                .Add(x => x.Model, model));

            var formItem = cut.Find(".el-form-item");
            Assert.Contains("is-required", formItem.ClassList);
            Assert.Equal("Display name", cut.Find("label.el-form-item__label").TextContent.Trim());

            Assert.False(await cut.InvokeAsync(() => cut.Instance.Validate()));
            cut.Render();

            Assert.Contains("Generated name is required", cut.Markup);
        }

        [Fact]
        public void GeneratedFormKeepsLegacyRequiredDefault()
        {
            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.EntityType, typeof(PlainFormModel))
                .Add(x => x.Model, new PlainFormModel()));

            Assert.Contains("is-required", cut.Find(".el-form-item").ClassList);
        }

        [Fact]
        public async Task CanUseCascadedEditContextModel()
        {
            var model = new DataAnnotationFormModel
            {
                Name = "Alice"
            };
            var editContext = new EditContext(model);

            var cut = Render<CascadingValue<EditContext>>(parameters => parameters
                .Add(x => x.Value, editContext)
                .AddChildContent<ElForm>(form => form
                    .AddChildContent<ElFormItem<string>>(item => item
                        .Add(x => x.Prop, nameof(DataAnnotationFormModel.Name))
                        .AddChildContent<ElInput<string>>())));

            var form = cut.FindComponent<ElForm>();
            Assert.Equal("Alice", cut.Find("input").GetAttribute("value"));

            cut.Find("input").Input("Carol");

            Assert.Equal("Carol", model.Name);
            Assert.True(await form.InvokeAsync(() => form.Instance.Validate()));
        }

        [Fact]
        public async Task RebuildsDataAnnotationRulesWhenModelChanges()
        {
            var cut = Render<FormModelSwapHost>();

            var form = cut.FindComponent<ElForm>();
            Assert.False(await form.InvokeAsync(() => form.Instance.Validate()));

            await cut.InvokeAsync(() => cut.Instance.Model = new PlainFormModel());
            cut.Render();

            form = cut.FindComponent<ElForm>();
            Assert.True(await form.InvokeAsync(() => form.Instance.Validate()));
        }

        private class ProfileFormModel
        {
            public ProfileUser User { get; set; }
        }

        private class ProfileUser
        {
            public string Name { get; set; }
        }

        private class DataAnnotationFormModel
        {
            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }
        }

        private class PlainFormModel
        {
            public string Name { get; set; }
        }

        private class FormModelSwapHost : ComponentBase
        {
            public object Model { get; set; } = new DataAnnotationFormModel();

            protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
            {
                builder.OpenComponent<ElForm>(0);
                builder.AddAttribute(1, nameof(ElForm.Model), Model);
                builder.AddAttribute(2, nameof(ElForm.ChildContent), (RenderFragment)(childBuilder =>
                {
                    childBuilder.OpenComponent<ElFormItem<string>>(0);
                    childBuilder.AddAttribute(1, nameof(ElFormItem<string>.Prop), nameof(DataAnnotationFormModel.Name));
                    childBuilder.AddAttribute(2, nameof(ElFormItem<string>.ChildContent), (RenderFragment)(inputBuilder =>
                    {
                        inputBuilder.OpenComponent<ElInput<string>>(0);
                        inputBuilder.CloseComponent();
                    }));
                    childBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            }
        }

        private class GeneratedAnnotationModel
        {
            [Display(Name = "Display name")]
            [Required(ErrorMessage = "Generated name is required")]
            public string Name { get; set; }
        }

        private class DelayedInvalidRule : IAsyncValidationRule
        {
            public string ErrorMessage { get; set; } = "Async invalid";

            public bool Validate(object value)
            {
                return false;
            }

            public async Task<bool> ValidateAsync(object value)
            {
                await Task.Delay(1);
                return false;
            }
        }

        private class AsyncOnlyRule : IAsyncValidationRule
        {
            public int SyncCalls { get; private set; }

            public int AsyncCalls { get; private set; }

            public string ErrorMessage { get; set; } = "Async only invalid";

            public bool Validate(object value)
            {
                SyncCalls++;
                return true;
            }

            public Task<bool> ValidateAsync(object value)
            {
                AsyncCalls++;
                return Task.FromResult(false);
            }
        }
    }
}
