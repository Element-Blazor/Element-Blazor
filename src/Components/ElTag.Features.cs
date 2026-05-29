using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElTag
    {
        [Parameter]
        public TagEffect Effect
        {
            get => Theme switch
            {
                TagTheme.Dark => TagEffect.Dark,
                TagTheme.Plain => TagEffect.Plain,
                _ => TagEffect.Light
            };
            set => Theme = value switch
            {
                TagEffect.Dark => TagTheme.Dark,
                TagEffect.Plain => TagTheme.Plain,
                _ => TagTheme.Light
            };
        }

        [Parameter]
        public bool Round { get; set; }

        [Parameter]
        public bool Hit { get; set; }

        [Parameter]
        public bool DisableTransitions { get; set; }

        [Parameter]
        public EventCallback OnClose
        {
            get => OnAfteClose;
            set => OnAfteClose = value;
        }

        private string FeatureClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add(Cls)
            .AddIf(Round, "is-round")
            .AddIf(Hit, "is-hit")
            .AddIf(DisableTransitions, "el-tag--no-transition")
            .ToString();
    }
}
