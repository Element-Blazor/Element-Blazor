using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElTour : ElementComponentBase
    {
        [Parameter]
        public bool ModelValue { get; set; }

        [Parameter]
        public bool Value
        {
            get => ModelValue;
            set => ModelValue = value;
        }

        [Parameter]
        public EventCallback<bool> ModelValueChanged { get; set; }

        [Parameter]
        public EventCallback<bool> ValueChanged
        {
            get => ModelValueChanged;
            set => ModelValueChanged = value;
        }

        [Parameter]
        public IList<TourStep> Steps { get; set; }

        [Parameter]
        public int Current { get; set; }

        [Parameter]
        public EventCallback<int> CurrentChanged { get; set; }

        [Parameter]
        public bool Mask { get; set; } = true;

        [Parameter]
        public RenderFragment<TourStep> Header { get; set; }

        [Parameter]
        public EventCallback<int> OnChange { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        protected IReadOnlyList<TourStep> StepList => Steps?.Where(x => x != null).ToList() ?? new List<TourStep>();

        protected TourStep ActiveStep => StepList.Count == 0 ? null : StepList[Math.Max(0, Math.Min(Current, StepList.Count - 1))];

        protected string TourClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-tour", Cls)
            .ToString();

        protected string PanelClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-tour__content")
            .AddIf(!string.IsNullOrWhiteSpace(ActiveStep?.Placement), $"is-{ActiveStep.Placement}")
            .ToString();

        protected async Task PreviousAsync()
        {
            await SetCurrentAsync(Current - 1);
        }

        protected async Task NextAsync()
        {
            await SetCurrentAsync(Current + 1);
        }

        protected async Task SetCurrentAsync(int value)
        {
            if (StepList.Count == 0)
            {
                Current = 0;
                return;
            }

            Current = Math.Max(0, Math.Min(value, StepList.Count - 1));
            if (CurrentChanged.HasDelegate)
            {
                await CurrentChanged.InvokeAsync(Current);
            }

            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(Current);
            }
        }

        protected async Task CloseAsync()
        {
            ModelValue = false;
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(false);
            }

            if (OnClose.HasDelegate)
            {
                await OnClose.InvokeAsync(null);
            }
        }
    }
}
