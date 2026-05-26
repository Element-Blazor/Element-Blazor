using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElDivider : ElementComponentBase
    {
        private HtmlPropertyBuilder dividerCssBuilder;
        private HtmlPropertyBuilder dividerStyleBuilder;
        private HtmlPropertyBuilder contentCssBuilder;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public DividerDirection Direction { get; set; } = DividerDirection.Horizontal;

        [Parameter]
        public DividerContentPosition ContentPosition { get; set; } = DividerContentPosition.Center;

        [Parameter]
        public string BorderStyle { get; set; }

        protected bool IsHorizontal => Direction == DividerDirection.Horizontal;

        protected string AriaOrientation => IsHorizontal ? "horizontal" : "vertical";

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            var direction = Direction.ToString().ToLowerInvariant();
            var contentPosition = ContentPosition.ToString().ToLowerInvariant();

            dividerCssBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-divider", $"el-divider--{direction}", Cls);
            dividerStyleBuilder = HtmlPropertyBuilder.CreateCssStyleBuilder()
                .Add(Style)
                .AddIf(!string.IsNullOrWhiteSpace(BorderStyle), IsHorizontal ? $"border-top-style:{BorderStyle}" : $"border-left-style:{BorderStyle}");
            contentCssBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-divider__text", $"is-{contentPosition}");
        }
    }
}
