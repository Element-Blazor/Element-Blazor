using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElSpace : ElementComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public SpaceDirection Direction { get; set; } = SpaceDirection.Horizontal;

        [Parameter]
        public SpaceAlignment Alignment { get; set; } = SpaceAlignment.Center;

        [Parameter]
        public SpaceSize Size { get; set; } = SpaceSize.Default;

        [Parameter]
        public int? Spacer { get; set; }

        [Parameter]
        public bool Wrap { get; set; }

        [Parameter]
        public bool Fill { get; set; }

        [Parameter]
        public string FillRatio { get; set; }

        protected string SpaceClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-space", Cls)
            .Add($"el-space--{Direction.ToString().ToLowerInvariant()}")
            .AddIf(Wrap, "is-wrap")
            .AddIf(Fill, "is-fill")
            .ToString();

        protected string SpaceStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add($"align-items:{AlignmentValue}")
            .Add($"gap:{GapValue}")
            .AddIf(!string.IsNullOrWhiteSpace(FillRatio), $"--el-space-fill-ratio:{FillRatio}")
            .Add(Style)
            .ToString();

        private string GapValue => Spacer.HasValue
            ? $"{Spacer.Value}px"
            : Size switch
            {
                SpaceSize.Small => "8px",
                SpaceSize.Large => "16px",
                _ => "12px"
            };

        private string AlignmentValue => Alignment switch
        {
            SpaceAlignment.Start => "flex-start",
            SpaceAlignment.End => "flex-end",
            SpaceAlignment.Baseline => "baseline",
            SpaceAlignment.Stretch => "stretch",
            _ => "center"
        };
    }
}
