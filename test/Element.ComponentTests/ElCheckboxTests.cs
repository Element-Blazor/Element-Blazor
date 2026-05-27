using Bunit;
using Element;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Element.ComponentTests
{
    public class ElCheckboxTests : BunitContext
    {
        public ElCheckboxTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void RendersIndeterminateState()
        {
            var cut = Render<ElCheckbox<bool>>(parameters => parameters
                .Add(x => x.Value, true)
                .Add(x => x.Indeterminate, true)
                .AddChildContent("All"));

            var label = cut.Find("label");
            Assert.Equal("mixed", label.GetAttribute("aria-checked"));
            Assert.Contains("is-indeterminate", label.ClassList);
            Assert.Contains("is-indeterminate", cut.Find(".el-checkbox__input").ClassList);
            Assert.False(cut.Find("input").HasAttribute("checked"));
        }

        [Fact]
        public void GroupMaxDisablesUncheckedOptions()
        {
            var cut = Render<ElCheckboxGroup<string>>(parameters => parameters
                .Add(x => x.Value, new[] { "a" })
                .Add(x => x.Max, 1)
                .AddChildContent(builder =>
                {
                    builder.OpenComponent<ElCheckbox<string>>(0);
                    builder.AddAttribute(1, "Value", "a");
                    builder.AddAttribute(2, "ChildContent", Text("A"));
                    builder.CloseComponent();
                    builder.OpenComponent<ElCheckbox<string>>(3);
                    builder.AddAttribute(4, "Value", "b");
                    builder.AddAttribute(5, "ChildContent", Text("B"));
                    builder.CloseComponent();
                }));

            var inputs = cut.FindAll("input");
            Assert.False(inputs[0].HasAttribute("disabled"));
            Assert.True(inputs[1].HasAttribute("disabled"));
        }

        [Fact]
        public void GroupMinPreventsUncheckingLastRequiredOption()
        {
            var selected = new List<string> { "a" };
            var cut = Render<ElCheckboxGroup<string>>(parameters => parameters
                .Add(x => x.Value, selected)
                .Add(x => x.ValueChanged, values => selected = values.ToList())
                .Add(x => x.Min, 1)
                .AddChildContent(builder =>
                {
                    builder.OpenComponent<ElCheckbox<string>>(0);
                    builder.AddAttribute(1, "Value", "a");
                    builder.AddAttribute(2, "ChildContent", Text("A"));
                    builder.CloseComponent();
                }));

            var input = cut.Find("input");
            Assert.True(input.HasAttribute("disabled"));
            input.Change(false);

            Assert.Equal(new[] { "a" }, selected);
        }

        [Fact]
        public void GroupSizePropagatesToCheckboxes()
        {
            var cut = Render<ElCheckboxGroup<string>>(parameters => parameters
                .Add(x => x.Size, InputSize.Small)
                .AddChildContent(builder =>
                {
                    builder.OpenComponent<ElCheckbox<string>>(0);
                    builder.AddAttribute(1, "Value", "a");
                    builder.AddAttribute(2, "ChildContent", Text("A"));
                    builder.CloseComponent();
                }));

            Assert.Contains("el-checkbox--small", cut.Find("label").ClassList);
        }

        [Fact]
        public void CheckboxButtonUsesGroupSizeAndLimitDisabledState()
        {
            var cut = Render<ElCheckboxGroup<string>>(parameters => parameters
                .Add(x => x.Value, new[] { "a" })
                .Add(x => x.Max, 1)
                .Add(x => x.Size, InputSize.Large)
                .AddChildContent(builder =>
                {
                    builder.OpenComponent<ElCheckboxButton<string>>(0);
                    builder.AddAttribute(1, "Value", "a");
                    builder.AddAttribute(2, "ChildContent", Text("A"));
                    builder.CloseComponent();
                    builder.OpenComponent<ElCheckboxButton<string>>(3);
                    builder.AddAttribute(4, "Value", "b");
                    builder.AddAttribute(5, "ChildContent", Text("B"));
                    builder.CloseComponent();
                }));

            var labels = cut.FindAll("label");
            Assert.Contains("el-checkbox-button--large", labels[0].ClassList);
            Assert.Contains("el-checkbox-button--large", labels[1].ClassList);
            Assert.Contains("is-disabled", labels[1].ClassList);
            Assert.True(labels[1].QuerySelector("input").HasAttribute("disabled"));
        }

        private static Microsoft.AspNetCore.Components.RenderFragment Text(string value)
        {
            return builder => builder.AddContent(0, value);
        }
    }
}
