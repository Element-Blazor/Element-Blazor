using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Element.ComponentTests
{
    public class ElRadioTests : BunitContext
    {
        public ElRadioTests()
        {
            Services.AddElementServices();
            JSInterop.SetupVoid("execFocus", _ => true);
        }

        [Fact]
        public void RadioRespectsDisabledStateAndBorderedSizeClasses()
        {
            var cut = Render<ElRadio<string>>(parameters => parameters
                .Add(x => x.Value, "a")
                .Add(x => x.SelectedValue, "a")
                .Add(x => x.IsDisabled, true)
                .Add(x => x.IsBordered, true)
                .Add(x => x.Size, RadioSize.Small)
                .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Alpha"))));

            var label = cut.Find("label.el-radio");

            Assert.Contains("is-checked", label.ClassList);
            Assert.Contains("is-disabled", label.ClassList);
            Assert.Contains("is-bordered", label.ClassList);
            Assert.Contains("el-radio--small", label.ClassList);
            Assert.Equal("-1", label.GetAttribute("tabindex"));
            Assert.Equal("true", label.GetAttribute("aria-checked"));
            Assert.Equal("true", label.GetAttribute("aria-disabled"));
        }

        [Fact]
        public void StandaloneRadioSupportsEnterAndSpaceSelection()
        {
            string selected = null;
            var cut = Render<ElRadio<string>>(parameters => parameters
                .Add(x => x.Value, "a")
                .Add(x => x.SelectedValueChanged, next => selected = next)
                .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Alpha"))));

            var label = cut.Find("label.el-radio");
            label.KeyDown(new KeyboardEventArgs { Key = "Enter" });
            Assert.Equal("a", selected);

            selected = null;
            label.KeyDown(new KeyboardEventArgs { Key = " " });
            Assert.Equal("a", selected);
        }

        [Fact]
        public void RadioGroupKeyboardNavigationSkipsDisabledOptions()
        {
            var host = Render<RadioGroupHost>();
            var radios = host.FindAll("label.el-radio");

            Assert.Equal("left", host.Instance.Value);
            Assert.Equal("0", radios[0].GetAttribute("tabindex"));
            Assert.Equal("-1", radios[2].GetAttribute("tabindex"));

            radios[0].KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

            host.WaitForAssertion(() =>
            {
                Assert.Equal("right", host.Instance.Value);
                var updated = host.FindAll("label.el-radio");
                Assert.Equal("-1", updated[0].GetAttribute("tabindex"));
                Assert.Equal("0", updated[2].GetAttribute("tabindex"));
                Assert.Equal("true", updated[2].GetAttribute("aria-checked"));
            });
        }

        [Fact]
        public void RadioGroupCascadesBorderAndSize()
        {
            var cut = Render<ElRadioGroup<string>>(parameters => parameters
                .Add(x => x.SelectedValue, "a")
                .Add(x => x.Bordered, true)
                .Add(x => x.Size, RadioSize.Mini)
                .AddChildContent<ElRadio<string>>(child => child
                    .Add(x => x.Value, "a")
                    .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Alpha"))))
                .AddChildContent<ElRadio<string>>(child => child
                    .Add(x => x.Value, "b")
                    .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Beta")))));

            var radios = cut.FindAll("label.el-radio");

            Assert.All(radios, radio =>
            {
                Assert.Contains("is-bordered", radio.ClassList);
                Assert.Contains("el-radio--mini", radio.ClassList);
            });
        }

        [Fact]
        public void RadioGroupHomeAndEndSelectEdges()
        {
            var host = Render<RadioGroupHost>();
            var radios = host.FindAll("label.el-radio");

            radios[0].KeyDown(new KeyboardEventArgs { Key = "End" });
            host.WaitForAssertion(() => Assert.Equal("right", host.Instance.Value));

            radios = host.FindAll("label.el-radio");
            radios[2].KeyDown(new KeyboardEventArgs { Key = "Home" });
            host.WaitForAssertion(() => Assert.Equal("left", host.Instance.Value));
        }

        [Fact]
        public void DisabledGroupDoesNotReactToKeyboard()
        {
            string value = "a";
            var cut = Render<ElRadioGroup<string>>(parameters => parameters
                .Add(x => x.SelectedValue, value)
                .Add(x => x.IsDisabled, true)
                .Add(x => x.SelectedValueChanged, next => value = next)
                .AddChildContent<ElRadio<string>>(child => child
                    .Add(x => x.Value, "a")
                    .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Alpha"))))
                .AddChildContent<ElRadio<string>>(child => child
                    .Add(x => x.Value, "b")
                    .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Beta")))));

            var radios = cut.FindAll("label.el-radio");
            radios[0].KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

            Assert.Equal("a", value);
            Assert.Equal("true", cut.Find("[role='radiogroup']").GetAttribute("aria-disabled"));
            Assert.Equal("-1", radios[0].GetAttribute("tabindex"));
            Assert.Equal("-1", radios[1].GetAttribute("tabindex"));
        }

        [Fact]
        public void RadioButtonRespectsGroupDisabledSizeAndKeyboard()
        {
            var host = Render<RadioButtonGroupHost>();
            var buttons = host.FindAll("label.el-radio-button");

            Assert.Equal("left", host.Instance.Value);
            Assert.Contains("is-active", buttons[0].ClassList);
            Assert.Contains("el-radio-button--small", buttons[0].ClassList);
            Assert.Contains("is-disabled", buttons[1].ClassList);
            Assert.Equal("true", buttons[1].GetAttribute("aria-disabled"));
            Assert.Equal("-1", buttons[1].GetAttribute("tabindex"));
            Assert.Equal("0", buttons[0].GetAttribute("tabindex"));
            Assert.Equal("-1", buttons[2].GetAttribute("tabindex"));

            buttons[0].KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

            host.WaitForAssertion(() =>
            {
                Assert.Equal("right", host.Instance.Value);
                var updated = host.FindAll("label.el-radio-button");
                Assert.Equal("-1", updated[0].GetAttribute("tabindex"));
                Assert.Equal("0", updated[2].GetAttribute("tabindex"));
                Assert.Equal("true", updated[2].GetAttribute("aria-checked"));
                Assert.Contains("is-active", updated[2].ClassList);
            });
        }

        [Fact]
        public void DisabledRadioButtonGroupDoesNotReactToKeyboardOrClick()
        {
            string value = "a";
            var cut = Render<ElRadioGroup<string>>(parameters => parameters
                .Add(x => x.SelectedValue, value)
                .Add(x => x.IsDisabled, true)
                .Add(x => x.SelectedValueChanged, next => value = next)
                .AddChildContent<ElRadioButton<string>>(child => child
                    .Add(x => x.Value, "a")
                    .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Alpha"))))
                .AddChildContent<ElRadioButton<string>>(child => child
                    .Add(x => x.Value, "b")
                    .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Beta")))));

            var buttons = cut.FindAll("label.el-radio-button");
            buttons[0].KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
            buttons[1].Click();

            Assert.Equal("a", value);
            Assert.All(buttons, button =>
            {
                Assert.Contains("is-disabled", button.ClassList);
                Assert.Equal("-1", button.GetAttribute("tabindex"));
                Assert.Equal("true", button.GetAttribute("aria-disabled"));
                Assert.True(button.QuerySelector("input").HasAttribute("disabled"));
            });
        }

        [Fact]
        public void RadioGroupWithNoSelectionMakesFirstEnabledRadioTabbable()
        {
            var cut = Render<ElRadioGroup<string>>(parameters => parameters
                .AddChildContent<ElRadio<string>>(child => child
                    .Add(x => x.Value, "a")
                    .Add(x => x.IsDisabled, true)
                    .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Alpha"))))
                .AddChildContent<ElRadio<string>>(child => child
                    .Add(x => x.Value, "b")
                    .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Beta"))))
                .AddChildContent<ElRadio<string>>(child => child
                    .Add(x => x.Value, "c")
                    .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Gamma")))));

            var radios = cut.FindAll("label.el-radio");

            Assert.Equal("-1", radios[0].GetAttribute("tabindex"));
            Assert.Equal("0", radios[1].GetAttribute("tabindex"));
            Assert.Equal("-1", radios[2].GetAttribute("tabindex"));
        }

        [Fact]
        public void FormItemSizeCascadesToRadioGroup()
        {
            var cut = Render<ElForm>(parameters => parameters
                .AddChildContent<ElFormItem<string>>(item => item
                    .Add(x => x.Name, "choice")
                    .Add(x => x.Size, InputSize.Small)
                    .AddChildContent<ElRadioGroup<string>>(group => group
                        .Add(x => x.SelectedValue, "a")
                        .AddChildContent<ElRadio<string>>(child => child
                            .Add(x => x.Value, "a")
                            .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Alpha")))))));

            var radio = cut.Find("label.el-radio");

            Assert.Contains("el-radio--small", radio.ClassList);
        }

        private class RadioGroupHost : ComponentBase
        {
            public string Value { get; set; } = "left";

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<ElRadioGroup<string>>(0);
                builder.AddAttribute(1, nameof(ElRadioGroup<string>.SelectedValue), Value);
                builder.AddAttribute(2, nameof(ElRadioGroup<string>.Size), RadioSize.Medium);
                builder.AddAttribute(3, nameof(ElRadioGroup<string>.SelectedValueChanged), EventCallback.Factory.Create<string>(this, OnValueChanged));
                builder.AddAttribute(4, nameof(ElRadioGroup<string>.ChildContent), (RenderFragment)(content =>
                {
                    content.OpenComponent<ElRadio<string>>(0);
                    content.AddAttribute(1, nameof(ElRadio<string>.Value), "left");
                    content.AddAttribute(2, nameof(ElRadio<string>.IsBordered), true);
                    content.AddAttribute(3, nameof(ElRadio<string>.ChildContent), (RenderFragment)(builder => builder.AddContent(0, "Left")));
                    content.CloseComponent();

                    content.OpenComponent<ElRadio<string>>(4);
                    content.AddAttribute(5, nameof(ElRadio<string>.Value), "center");
                    content.AddAttribute(6, nameof(ElRadio<string>.IsDisabled), true);
                    content.AddAttribute(7, nameof(ElRadio<string>.IsBordered), true);
                    content.AddAttribute(8, nameof(ElRadio<string>.ChildContent), (RenderFragment)(builder => builder.AddContent(0, "Center")));
                    content.CloseComponent();

                    content.OpenComponent<ElRadio<string>>(9);
                    content.AddAttribute(10, nameof(ElRadio<string>.Value), "right");
                    content.AddAttribute(11, nameof(ElRadio<string>.IsBordered), true);
                    content.AddAttribute(12, nameof(ElRadio<string>.ChildContent), (RenderFragment)(builder => builder.AddContent(0, "Right")));
                    content.CloseComponent();
                }));
                builder.CloseComponent();
            }

            private void OnValueChanged(string value)
            {
                Value = value;
                StateHasChanged();
            }
        }

        private class RadioButtonGroupHost : ComponentBase
        {
            public string Value { get; set; } = "left";

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<ElRadioGroup<string>>(0);
                builder.AddAttribute(1, nameof(ElRadioGroup<string>.SelectedValue), Value);
                builder.AddAttribute(2, nameof(ElRadioGroup<string>.Size), RadioSize.Small);
                builder.AddAttribute(3, nameof(ElRadioGroup<string>.SelectedValueChanged), EventCallback.Factory.Create<string>(this, OnValueChanged));
                builder.AddAttribute(4, nameof(ElRadioGroup<string>.ChildContent), (RenderFragment)(content =>
                {
                    content.OpenComponent<ElRadioButton<string>>(0);
                    content.AddAttribute(1, nameof(ElRadioButton<string>.Value), "left");
                    content.AddAttribute(2, nameof(ElRadioButton<string>.ChildContent), (RenderFragment)(builder => builder.AddContent(0, "Left")));
                    content.CloseComponent();

                    content.OpenComponent<ElRadioButton<string>>(3);
                    content.AddAttribute(4, nameof(ElRadioButton<string>.Value), "center");
                    content.AddAttribute(5, nameof(ElRadioButton<string>.IsDisabled), true);
                    content.AddAttribute(6, nameof(ElRadioButton<string>.ChildContent), (RenderFragment)(builder => builder.AddContent(0, "Center")));
                    content.CloseComponent();

                    content.OpenComponent<ElRadioButton<string>>(7);
                    content.AddAttribute(8, nameof(ElRadioButton<string>.Value), "right");
                    content.AddAttribute(9, nameof(ElRadioButton<string>.ChildContent), (RenderFragment)(builder => builder.AddContent(0, "Right")));
                    content.CloseComponent();
                }));
                builder.CloseComponent();
            }

            private void OnValueChanged(string value)
            {
                Value = value;
                StateHasChanged();
            }
        }
    }
}
