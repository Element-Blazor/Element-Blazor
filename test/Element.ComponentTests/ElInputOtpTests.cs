using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Xunit;

namespace Element.ComponentTests
{
    public class ElInputOtpTests : BunitContext
    {
        public ElInputOtpTests()
        {
            Services.AddElementServices();
            JSInterop.SetupVoid("execFocus", _ => true);
        }

        [Fact]
        public void InputtingSingleCharactersAdvancesAndCompletes()
        {
            var host = Render<OtpHost>();
            var inputs = host.FindAll("input");

            inputs[0].Input("1");
            inputs = host.FindAll("input");
            Assert.Equal("1", host.Instance.Value);

            inputs[1].Input("2");
            inputs = host.FindAll("input");
            inputs[2].Input("3");
            inputs = host.FindAll("input");
            inputs[3].Input("4");

            Assert.Equal("1234", host.Instance.Value);
            Assert.Equal(new[] { "1234" }, host.Instance.Changes);
        }

        [Fact]
        public void BackspaceClearsCurrentCellThenMovesBackward()
        {
            var cut = Render<ElInputOtp>(parameters => parameters
                .Add(x => x.Value, "1234")
                .Add(x => x.Length, 4));

            var inputs = cut.FindAll("input");
            inputs[3].KeyDown(new KeyboardEventArgs { Key = "Backspace" });
            inputs = cut.FindAll("input");

            Assert.Equal(string.Empty, inputs[3].GetAttribute("value"));

            inputs[3].KeyDown(new KeyboardEventArgs { Key = "Backspace" });
            inputs = cut.FindAll("input");

            Assert.Equal(string.Empty, inputs[2].GetAttribute("value"));
        }

        [Fact]
        public void DeleteClearsCurrentCellWithoutMovingOtherCells()
        {
            var cut = Render<ElInputOtp>(parameters => parameters
                .Add(x => x.Value, "1234")
                .Add(x => x.Length, 4));

            var inputs = cut.FindAll("input");
            inputs[1].KeyDown(new KeyboardEventArgs { Key = "Delete" });
            inputs = cut.FindAll("input");

            Assert.Equal("1", inputs[0].GetAttribute("value"));
            Assert.Equal("3", inputs[1].GetAttribute("value"));
            Assert.Equal("4", inputs[2].GetAttribute("value"));
            Assert.Equal(string.Empty, inputs[3].GetAttribute("value"));
        }

        [Fact]
        public void MultiCharacterInputFillsRemainingCells()
        {
            var cut = Render<ElInputOtp>(parameters => parameters
                .Add(x => x.Value, "1")
                .Add(x => x.Length, 4));

            var inputs = cut.FindAll("input");
            inputs[1].Input("2345");
            inputs = cut.FindAll("input");

            Assert.Equal("1", inputs[0].GetAttribute("value"));
            Assert.Equal("2", inputs[1].GetAttribute("value"));
            Assert.Equal("3", inputs[2].GetAttribute("value"));
            Assert.Equal("4", inputs[3].GetAttribute("value"));
        }

        [Fact]
        public void PartialMultiCharacterInputDoesNotRaiseChangeUntilComplete()
        {
            var host = Render<OtpHost>();

            host.FindAll("input")[0].Input("12");
            Assert.Equal("12", host.Instance.Value);
            Assert.Empty(host.Instance.Changes);

            host.FindAll("input")[2].Input("34");

            Assert.Equal("1234", host.Instance.Value);
            Assert.Equal(new[] { "1234" }, host.Instance.Changes);
        }

        [Fact]
        public void HomeAndEndKeysKeepAllCellsAddressable()
        {
            var host = Render<OtpHost>();
            host.FindAll("input")[0].Input("1");
            host.FindAll("input")[1].Input("2");

            var inputs = host.FindAll("input");
            inputs[2].KeyDown(new KeyboardEventArgs { Key = "Home" });
            inputs = host.FindAll("input");
            inputs[0].Input("9");

            Assert.Equal("92", host.Instance.Value);

            inputs = host.FindAll("input");
            inputs[0].KeyDown(new KeyboardEventArgs { Key = "End" });
            inputs = host.FindAll("input");
            inputs[3].Input("8");

            Assert.Equal("928", host.Instance.Value);
        }

        [Fact]
        public void CtrlArrowKeysJumpToEdges()
        {
            var host = Render<OtpHost>();
            var inputs = host.FindAll("input");

            inputs[2].KeyDown(new KeyboardEventArgs { Key = "ArrowLeft", CtrlKey = true });
            inputs = host.FindAll("input");
            inputs[0].Input("9");

            Assert.Equal("9", host.Instance.Value);

            inputs = host.FindAll("input");
            inputs[0].KeyDown(new KeyboardEventArgs { Key = "ArrowRight", MetaKey = true });
            inputs = host.FindAll("input");
            inputs[3].Input("8");

            Assert.Equal("98", host.Instance.Value);
        }

        [Fact]
        public void CtrlBackspaceClearsThroughCurrentCell()
        {
            string value = "1234";
            var cut = Render<ElInputOtp>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.Length, 4));

            cut.FindAll("input")[2].KeyDown(new KeyboardEventArgs
            {
                Key = "Backspace",
                CtrlKey = true
            });

            Assert.Equal("4", value);
            var inputs = cut.FindAll("input");
            Assert.Equal("4", inputs[0].GetAttribute("value"));
            Assert.Equal(string.Empty, inputs[1].GetAttribute("value"));
        }

        [Fact]
        public void MetaDeleteClearsFromCurrentCell()
        {
            string value = "1234";
            var cut = Render<ElInputOtp>(parameters => parameters
                .Add(x => x.Value, value)
                .Add(x => x.ValueChanged, next => value = next)
                .Add(x => x.Length, 4));

            cut.FindAll("input")[1].KeyDown(new KeyboardEventArgs
            {
                Key = "Delete",
                MetaKey = true
            });

            Assert.Equal("1", value);
            var inputs = cut.FindAll("input");
            Assert.Equal("1", inputs[0].GetAttribute("value"));
            Assert.Equal(string.Empty, inputs[1].GetAttribute("value"));
        }

        [Fact]
        public void DisabledAndReadonlyKeyboardDoesNotChangeValue()
        {
            string disabledValue = "1234";
            var disabled = Render<ElInputOtp>(parameters => parameters
                .Add(x => x.Value, disabledValue)
                .Add(x => x.ValueChanged, next => disabledValue = next)
                .Add(x => x.Length, 4)
                .Add(x => x.Disabled, true));

            disabled.FindAll("input")[1].KeyDown(new KeyboardEventArgs { Key = "Delete" });
            Assert.Equal("1234", disabledValue);

            string readonlyValue = "1234";
            var readOnly = Render<ElInputOtp>(parameters => parameters
                .Add(x => x.Value, readonlyValue)
                .Add(x => x.ValueChanged, next => readonlyValue = next)
                .Add(x => x.Length, 4)
                .Add(x => x.Readonly, true));

            readOnly.FindAll("input")[1].Input("9");
            readOnly.FindAll("input")[1].KeyDown(new KeyboardEventArgs { Key = "Backspace" });

            Assert.Equal("1234", readonlyValue);
        }

        [Fact]
        public void CellsExposeAriaAndMaskDisplay()
        {
            var cut = Render<ElInputOtp>(parameters => parameters
                .Add(x => x.Value, "12")
                .Add(x => x.Length, 4)
                .Add(x => x.Mask, true));

            var inputs = cut.FindAll("input");

            Assert.Equal("*", inputs[0].GetAttribute("value"));
            Assert.Equal("OTP character 1 of 4", inputs[0].GetAttribute("aria-label"));
            Assert.Equal("OTP character 4 of 4", inputs[3].GetAttribute("aria-label"));
            Assert.Equal("false", inputs[0].GetAttribute("aria-invalid"));
        }

        private class OtpHost : ComponentBase
        {
            public string Value { get; set; } = string.Empty;

            public List<string> Changes { get; } = new List<string>();

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<ElInputOtp>(0);
                builder.AddAttribute(1, nameof(ElInputOtp.Value), Value);
                builder.AddAttribute(2, nameof(ElInputOtp.Length), 4);
                builder.AddAttribute(3, nameof(ElInputOtp.ValueChanged), EventCallback.Factory.Create<string>(this, OnValueChanged));
                builder.AddAttribute(4, nameof(ElInputOtp.OnChange), EventCallback.Factory.Create<string>(this, OnChange));
                builder.CloseComponent();
            }

            private void OnValueChanged(string value)
            {
                Value = value;
                StateHasChanged();
            }

            private void OnChange(string value)
            {
                Changes.Add(value);
            }
        }
    }
}
