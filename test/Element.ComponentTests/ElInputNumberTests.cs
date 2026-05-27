using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Element.ComponentTests
{
    public class ElInputNumberTests : BunitContext
    {
        public ElInputNumberTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void KeyboardSupportsStrictSpinbuttonNavigation()
        {
            var cut = Render<InputNumberKeyboardHost>();

            var input = cut.Find("input");

            input.KeyDown(new KeyboardEventArgs { Key = "ArrowUp" });
            Assert.Equal(7, cut.Instance.Value);

            input.KeyDown(new KeyboardEventArgs { Key = "PageUp" });
            Assert.Equal(20, cut.Instance.Value);

            input.KeyDown(new KeyboardEventArgs { Key = "Home" });
            Assert.Equal(0, cut.Instance.Value);

            input.KeyDown(new KeyboardEventArgs { Key = "End" });
            Assert.Equal(20, cut.Instance.Value);

            input.KeyDown(new KeyboardEventArgs { Key = "PageDown" });
            Assert.Equal(0, cut.Instance.Value);
        }

        [Fact]
        public void EnterCommitsPendingTypedValue()
        {
            decimal? value = 1;
            decimal? changed = null;
            var cut = Render<ElInputNumber>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.StepStrictly, true)
                .Add(x => x.Step, 0.5m)
                .Add(x => x.Precision, 1)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.OnChange, next => changed = next));

            var input = cut.Find("input");
            input.Input("1.24");
            input.KeyDown(new KeyboardEventArgs { Key = "Enter" });

            Assert.Equal(1.0m, value);
            Assert.Equal(1.0m, changed);
            Assert.Equal("1.0", cut.Find("input").GetAttribute("value"));
        }

        [Fact]
        public void RendersSpinbuttonAriaAttributesOnInput()
        {
            var cut = Render<ElInputNumber>(parameters => parameters
                .Add(x => x.Value, 2.5m)
                .Add(x => x.Min, 1m)
                .Add(x => x.Max, 5m)
                .Add(x => x.Step, 0.5m)
                .Add(x => x.Precision, 1)
                .Add(x => x.Readonly, true));

            var input = cut.Find("input");

            Assert.Equal("spinbutton", input.GetAttribute("role"));
            Assert.Equal("1", input.GetAttribute("aria-valuemin"));
            Assert.Equal("5", input.GetAttribute("aria-valuemax"));
            Assert.Equal("2.5", input.GetAttribute("aria-valuenow"));
            Assert.Equal("2.5", input.GetAttribute("aria-valuetext"));
            Assert.Equal("true", input.GetAttribute("aria-readonly"));
        }

        [Fact]
        public void FormItemConnectsLabelAndValidationAria()
        {
            var cut = Render<ElForm>(parameters => parameters
                .AddChildContent<ElFormItem<decimal?>>(item => item
                    .Add(x => x.Name, "count")
                    .Add(x => x.Label, "Count")
                    .Add(x => x.Required, true)
                    .AddChildContent<ElInputNumber>(input => input
                        .Add(x => x.Value, (decimal?)null))));

            cut.InvokeAsync(() => cut.Instance.Validate());
            cut.Render();

            var label = cut.Find("label.el-form-item__label");
            var input = cut.Find("input");
            var error = cut.Find(".el-form-item__error");

            Assert.Equal(input.Id, label.GetAttribute("for"));
            Assert.Equal("true", input.GetAttribute("aria-invalid"));
            Assert.Equal(error.Id, input.GetAttribute("aria-describedby"));
            Assert.Equal("spinbutton", input.GetAttribute("role"));
        }

        [Fact]
        public void ControlsExposeAriaDisabledAndControlTargetInput()
        {
            var cut = Render<ElInputNumber>(parameters => parameters
                .Add(x => x.Value, 10m)
                .Add(x => x.Min, 0m)
                .Add(x => x.Max, 10m));

            var input = cut.Find("input");
            var increase = cut.Find(".el-input-number__increase");
            var decrease = cut.Find(".el-input-number__decrease");

            Assert.Equal(input.Id, increase.GetAttribute("aria-controls"));
            Assert.Equal(input.Id, decrease.GetAttribute("aria-controls"));
            Assert.Equal("true", increase.GetAttribute("aria-disabled"));
            Assert.Equal("false", decrease.GetAttribute("aria-disabled"));
        }

        private class InputNumberKeyboardHost : ComponentBase
        {
            public decimal? Value { get; set; } = 5m;

            protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
            {
                builder.OpenComponent<ElInputNumber>(0);
                builder.AddAttribute(1, nameof(ElInputNumber.Value), Value);
                builder.AddAttribute(2, nameof(ElInputNumber.ValueChanged), EventCallback.Factory.Create<decimal?>(this, OnValueChanged));
                builder.AddAttribute(3, nameof(ElInputNumber.Min), 0m);
                builder.AddAttribute(4, nameof(ElInputNumber.Max), 20m);
                builder.AddAttribute(5, nameof(ElInputNumber.Step), 2m);
                builder.CloseComponent();
            }

            private void OnValueChanged(decimal? value)
            {
                Value = value;
                StateHasChanged();
            }
        }
    }
}
