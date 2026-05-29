using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElSteps : ElementComponentBase
    {
        private int nextStepIndex;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public int Active { get; set; }

        [Parameter]
        public StepDirection Direction { get; set; }

        [Parameter]
        public bool AlignCenter { get; set; }

        [Parameter]
        public bool Simple { get; set; }

        [Parameter]
        public StepStatus FinishStatus { get; set; } = StepStatus.Finish;

        [Parameter]
        public StepStatus ProcessStatus { get; set; } = StepStatus.Process;

        [Parameter]
        public string Space { get; set; }

        protected string StepsClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-steps", Cls)
            .Add($"el-steps--{Direction.ToString().ToLowerInvariant()}")
            .AddIf(Simple, "el-steps--simple")
            .ToString();

        protected string StepsStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add(Style)
            .ToString();

        internal int RegisterStep()
        {
            return nextStepIndex++;
        }

        internal StepStatus GetStepStatus(int index, StepStatus? status)
        {
            if (status.HasValue)
            {
                return status.Value;
            }

            if (index < Active)
            {
                return FinishStatus;
            }

            return index == Active ? ProcessStatus : StepStatus.Wait;
        }

        internal bool ShouldFlex => Direction == StepDirection.Horizontal && string.IsNullOrWhiteSpace(Space);
    }
}
