using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElStep : ElementComponentBase
    {
        [CascadingParameter]
        public ElSteps Steps { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Description { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public StepStatus? Status { get; set; }

        [Parameter]
        public RenderFragment TitleContent { get; set; }

        [Parameter]
        public RenderFragment DescriptionContent { get; set; }

        [Parameter]
        public RenderFragment IconContent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        internal int Index { get; private set; }

        protected int DisplayIndex => Index + 1;

        protected StepDirection Direction => Steps?.Direction ?? StepDirection.Horizontal;

        protected bool IsSimple => Steps?.Simple == true;

        protected StepStatus EffectiveStatus => Steps?.GetStepStatus(Index, Status) ?? (Status ?? StepStatus.Wait);

        protected string StatusName => EffectiveStatus.ToString().ToLowerInvariant();

        protected bool HasDescription => DescriptionContent != null || !string.IsNullOrWhiteSpace(Description);

        protected string StepClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-step", Cls)
            .Add($"is-{Direction.ToString().ToLowerInvariant()}")
            .AddIf(Steps?.AlignCenter == true, "is-center")
            .AddIf(IsSimple, "is-simple")
            .AddIf(Steps?.ShouldFlex == true, "is-flex")
            .ToString();

        protected string StepStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(Steps?.Direction == StepDirection.Horizontal && !string.IsNullOrWhiteSpace(Steps?.Space), $"flex-basis:{ElementCssUtility.NormalizeCssSize(Steps.Space)}")
            .AddIf(Steps?.Direction == StepDirection.Horizontal && !string.IsNullOrWhiteSpace(Steps?.Space), $"max-width:{ElementCssUtility.NormalizeCssSize(Steps.Space)}")
            .AddIf(Steps?.ShouldFlex == true, "flex:1")
            .Add(Style)
            .ToString();

        protected string HeadClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-step__head")
            .Add($"is-{StatusName}")
            .ToString();

        protected string TitleClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-step__title")
            .Add($"is-{StatusName}")
            .ToString();

        protected string DescriptionClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-step__description")
            .Add($"is-{StatusName}")
            .ToString();

        protected string IconBoxClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-step__icon")
            .AddIf(HasIcon, "is-icon")
            .AddIf(!HasIcon, "is-text")
            .ToString();

        protected bool HasIcon => IconContent != null || !string.IsNullOrWhiteSpace(Icon) || !string.IsNullOrWhiteSpace(StatusIcon);

        protected string IconInnerClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Icon), "el-step__icon-inner", $"el-icon-{Icon}")
            .AddIf(string.IsNullOrWhiteSpace(Icon) && !string.IsNullOrWhiteSpace(StatusIcon), "el-step__icon-inner", "is-status", StatusIcon)
            .ToString();

        private string StatusIcon => EffectiveStatus switch
        {
            StepStatus.Success => "el-icon-check",
            StepStatus.Error => "el-icon-close",
            StepStatus.Finish => "el-icon-check",
            _ => null
        };

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Index = Steps?.RegisterStep() ?? 0;
        }
    }
}
