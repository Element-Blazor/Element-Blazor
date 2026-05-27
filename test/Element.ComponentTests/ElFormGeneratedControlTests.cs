using Bunit;
using Element;
using Element.ControlConfigs;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace Element.ComponentTests
{
    public class ElFormGeneratedControlTests : BunitContext
    {
        public ElFormGeneratedControlTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void GeneratesAdvancedFormControlsFromAttributes()
        {
            var model = new GeneratedFormModel
            {
                Name = "Element",
                Count = 2,
                Tags = new List<string> { "form" },
                Code = "123456",
                Assignee = "@alice",
                Mode = GeneratedMode.Fast,
                Rating = 4,
                Progress = 60,
                Enabled = true,
                Choice = GeneratedMode.Safe,
                VirtualChoice = GeneratedMode.Fast
            };

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.EntityType, typeof(GeneratedFormModel))
                .Add(x => x.Value, model));

            Assert.NotNull(cut.Find(".el-input"));
            Assert.NotNull(cut.Find(".el-input-number"));
            Assert.NotNull(cut.Find(".el-input-tag"));
            Assert.NotNull(cut.Find(".el-input-otp"));
            Assert.NotNull(cut.Find(".el-mention"));
            Assert.NotNull(cut.Find(".el-radio-group"));
            Assert.NotNull(cut.Find(".el-rate"));
            Assert.NotNull(cut.Find(".el-slider"));
            Assert.NotNull(cut.Find(".el-switch"));
            Assert.NotNull(cut.Find(".el-select"));
        }

        [Fact]
        public void GeneratedMentionSelectsOptionAndUpdatesModel()
        {
            var model = new MentionFormModel();
            Services.AddSingleton(new MentionLoader());

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.EntityType, typeof(MentionFormModel))
                .Add(x => x.Value, model));

            cut.Find("textarea").Input("@a");
            cut.Find(".el-select-dropdown__item").Click();

            Assert.Equal("@alice", model.Assignee);
        }

        [Fact]
        public void GeneratedInputAppliesAttributeConfiguration()
        {
            var model = new InputFormModel
            {
                Description = "Element"
            };

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.EntityType, typeof(InputFormModel))
                .Add(x => x.Value, model));

            var wrapper = cut.Find(".el-textarea");
            var textarea = cut.Find("textarea");

            Assert.Contains("el-textarea--large", wrapper.ClassList);
            Assert.Equal("Describe", textarea.GetAttribute("placeholder"));
            Assert.Equal("7", textarea.GetAttribute("maxlength"));
            Assert.Equal("3", textarea.GetAttribute("minlength"));
            Assert.Equal("4", textarea.GetAttribute("rows"));
            Assert.Equal("input-form", textarea.GetAttribute("form"));
            Assert.Equal("description input", textarea.GetAttribute("aria-label"));
            Assert.Equal("text", textarea.GetAttribute("inputmode"));
            Assert.Equal("2", textarea.GetAttribute("tabindex"));
            Assert.Equal("off", textarea.GetAttribute("autocomplete"));
            Assert.False(textarea.HasAttribute("readonly"));
            Assert.Contains("min-height:48px", textarea.GetAttribute("style"));
            Assert.Contains("resize:none", textarea.GetAttribute("style"));
            Assert.Equal("7 / 7", cut.Find(".el-input__count").TextContent.Trim());
        }

        [Fact]
        public void GeneratedInputUsesEditorPlaceholderWhenAttributePlaceholderIsEmpty()
        {
            var model = new PlaceholderFormModel();

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.EntityType, typeof(PlaceholderFormModel))
                .Add(x => x.Value, model));

            Assert.Equal("Editor placeholder", cut.Find("input").GetAttribute("placeholder"));
        }

        [Fact]
        public void GeneratedInputSupportsIconsAndAliases()
        {
            var model = new IconInputFormModel
            {
                Query = "Element"
            };
            JSInterop.SetupVoid("setDisabled", _ => true);

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.EntityType, typeof(IconInputFormModel))
                .Add(x => x.Value, model));

            var wrapper = cut.Find(".el-input");
            var input = cut.Find("input");

            Assert.Contains("el-input--prefix", wrapper.ClassList);
            Assert.Contains("el-input--suffix", wrapper.ClassList);
            Assert.Contains("el-icon-search", cut.Find(".el-input__prefix .el-input__icon").ClassList);
            Assert.Contains("el-icon-date", cut.Find(".el-input__suffix .el-input__icon").ClassList);
            Assert.Equal("Search", input.GetAttribute("placeholder"));
            Assert.True(input.HasAttribute("disabled"));
            Assert.Equal("email", input.GetAttribute("type"));
            Assert.True(input.HasAttribute("readonly"));
            Assert.Equal("search", input.GetAttribute("inputmode"));
            Assert.Equal("section", input.GetAttribute("style"));
        }

        [Fact]
        public void GeneratedInputTagAppliesDragAndTagConfiguration()
        {
            var model = new GeneratedInputTagFormModel
            {
                Tags = new List<string> { "one", "two" }
            };

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.EntityType, typeof(GeneratedInputTagFormModel))
                .Add(x => x.Value, model));

            var wrapper = cut.Find(".el-input-tag");
            var input = cut.Find("input");

            Assert.Contains("el-input-tag--large", wrapper.ClassList);
            Assert.False(input.HasAttribute("placeholder"));

            input.Input("three");
            input.KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Enter" });

            var tags = cut.FindAll(".el-tag");
            Assert.Equal(3, tags.Count);
            Assert.True(tags[0].HasAttribute("draggable"));
            Assert.Contains("is-draggable", tags[0].ClassList);
            Assert.Equal(new[] { "one", "two", "three" }, model.Tags);
        }

        [Fact]
        public void GeneratedRadioAppliesBorderedSizeAndDisabledConfiguration()
        {
            var model = new GeneratedRadioFormModel
            {
                Choice = GeneratedMode.Fast
            };

            var cut = Render<ElForm>(parameters => parameters
                .Add(x => x.EntityType, typeof(GeneratedRadioFormModel))
                .Add(x => x.Value, model));

            var group = cut.Find(".el-radio-group");
            var radios = cut.FindAll("label.el-radio");

            Assert.Equal("true", group.GetAttribute("aria-disabled"));
            Assert.All(radios, radio =>
            {
                Assert.Contains("is-disabled", radio.ClassList);
                Assert.Contains("is-bordered", radio.ClassList);
                Assert.Contains("el-radio--small", radio.ClassList);
                Assert.Equal("-1", radio.GetAttribute("tabindex"));
            });
        }

        private class GeneratedFormModel
        {
            public string Name { get; set; }

            [InputNumber(Min = 0, Max = 10)]
            public int Count { get; set; }

            [InputTag(Max = 3)]
            public List<string> Tags { get; set; }

            [InputOtp(Length = 6)]
            public string Code { get; set; }

            [Mention]
            public string Assignee { get; set; }

            [Radio]
            public GeneratedMode Mode { get; set; }

            [Rate(ShowScore = true)]
            public double? Rating { get; set; }

            [Slider(ShowInput = true)]
            public double Progress { get; set; }

            [Switch(ActiveText = "On", InactiveText = "Off")]
            public bool Enabled { get; set; }

            [FormControl(Control = typeof(ElSelect<>))]
            public GeneratedMode Choice { get; set; }

            [Select(Virtualized = true)]
            public GeneratedMode VirtualChoice { get; set; }
        }

        private class MentionFormModel
        {
            [Mention(DataSourceLoader = typeof(MentionLoader))]
            public string Assignee { get; set; }
        }

        private class InputFormModel
        {
            [Input(
                Type = InputType.Textarea,
                Size = InputSize.Large,
                Placeholder = "Describe",
                Clearable = true,
                Maxlength = 7,
                Minlength = 3,
                Resize = "none",
                ShowWordLimit = true,
                WordLimitPosition = "outside",
                InputStyle = "min-height:48px",
                Rows = 4,
                Form = "input-form",
                AriaLabel = "description input",
                Inputmode = "text",
                Tabindex = 2)]
            public string Description { get; set; }
        }

        private class PlaceholderFormModel
        {
            [EditorGenerator(Placeholder = "Editor placeholder")]
            [Input(Clearable = true)]
            public string Name { get; set; }
        }

        private class IconInputFormModel
        {
            [Input(
                Type = InputType.Email,
                IsDisabled = true,
                IsClearable = true,
                Readonly = true,
                PrefixIcon = "el-icon-search",
                SuffixIcon = "el-icon-date",
                Placeholder = "Search",
                Inputmode = "search",
                InputStyle = "section")]
            public string Query { get; set; }
        }

        private class GeneratedInputTagFormModel
        {
            [InputTag(
                Placeholder = "Tag item",
                Size = InputSize.Large,
                Max = 4,
                Draggable = true)]
            public List<string> Tags { get; set; }
        }

        private class GeneratedRadioFormModel
        {
            [Radio(Size = RadioSize.Small, Bordered = true, IsDisabled = true)]
            public GeneratedMode Choice { get; set; }
        }

        private class MentionLoader : IDataSourceLoader
        {
            public System.Threading.Tasks.Task<object> LoadAsync()
            {
                return System.Threading.Tasks.Task.FromResult<object>(new[]
                {
                    new MentionOption { Label = "Alice", Value = "alice" }
                });
            }
        }

        private enum GeneratedMode
        {
            [Description("Fast")]
            Fast,

            [Description("Safe")]
            Safe
        }
    }
}
