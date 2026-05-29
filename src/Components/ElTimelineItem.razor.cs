using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElTimelineItem : ElementComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Timestamp { get; set; }

        [Parameter]
        public RenderFragment TimestampContent { get; set; }

        [Parameter]
        public TimelinePlacement Placement { get; set; } = TimelinePlacement.Bottom;

        [Parameter]
        public string Type { get; set; }

        [Parameter]
        public string Color { get; set; }

        [Parameter]
        public string Size { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public RenderFragment Dot { get; set; }

        [Parameter]
        public bool HideTail { get; set; }

        protected string ItemClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-timeline-item", Cls)
            .ToString();

        protected string TailClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-timeline-item__tail")
            .AddIf(HideTail, "is-hidden")
            .ToString();

        protected string NodeClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-timeline-item__node")
            .AddIf(!string.IsNullOrWhiteSpace(Type), $"el-timeline-item__node--{Type}")
            .AddIf(!string.IsNullOrWhiteSpace(Size), $"el-timeline-item__node--{Size}")
            .AddIf(!string.IsNullOrWhiteSpace(Icon), "el-timeline-item__node--icon")
            .ToString();

        protected string NodeStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Color), $"background-color:{Color}")
            .ToString();

        protected string TimestampClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-timeline-item__timestamp", $"is-{Placement.ToString().ToLower()}")
            .ToString();

        protected string IconClass => NormalizeIcon(Icon);

        private static string NormalizeIcon(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var trimmed = value.Trim();
            return trimmed.StartsWith("el-icon-") ? trimmed : $"el-icon-{trimmed}";
        }
    }
}
