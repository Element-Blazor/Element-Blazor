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
