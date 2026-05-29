using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElCarousel : ElementComponentBase
    {
        private readonly List<ElCarouselItem> items = new List<ElCarouselItem>();
        private Timer timer;
        private int activeIndex;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public int InitialIndex { get; set; }

        [Parameter]
        public string Height { get; set; } = "300px";

        [Parameter]
        public bool Autoplay { get; set; } = true;

        [Parameter]
        public int Interval { get; set; } = 3000;

        [Parameter]
        public CarouselArrow Arrow { get; set; } = CarouselArrow.Hover;

        [Parameter]
        public CarouselIndicatorPosition IndicatorPosition { get; set; } = CarouselIndicatorPosition.Inside;

        [Parameter]
        public CarouselDirection Direction { get; set; } = CarouselDirection.Horizontal;

        [Parameter]
        public EventCallback<int> ActiveIndexChanged { get; set; }

        [Parameter]
        public EventCallback<int> OnChange { get; set; }

        internal IReadOnlyList<ElCarouselItem> Items => items.OrderBy(x => x.Order).ToList();

        internal int ActiveIndex => activeIndex;

        internal void AddItem(ElCarouselItem item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                NormalizeActiveIndex();
                Refresh();
            }
        }

        internal void RemoveItem(ElCarouselItem item)
        {
            if (items.Remove(item))
            {
                NormalizeActiveIndex();
                Refresh();
            }
        }

        protected string CarouselClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-carousel", $"el-carousel--{Direction.ToString().ToLower()}", Cls)
            .AddIf(Arrow == CarouselArrow.Always, "el-carousel--arrow-always")
            .AddIf(Arrow == CarouselArrow.Never, "el-carousel--arrow-never")
            .ToString();

        protected string CarouselStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add(Style)
            .ToString();

        protected string ContainerStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Height), $"height:{ElementCssUtility.NormalizeCssSize(Height)}")
            .ToString();

        protected bool ShowArrows => Arrow != CarouselArrow.Never && Items.Count > 1;

        protected string IndicatorsClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-carousel__indicators", $"el-carousel__indicators--{Direction.ToString().ToLower()}")
            .AddIf(IndicatorPosition == CarouselIndicatorPosition.Outside, "el-carousel__indicators--outside")
            .ToString();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            activeIndex = Math.Max(0, InitialIndex);
            EnsureTimer();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            EnsureTimer();
        }

        protected string GetIndicatorClass(int index)
        {
            return HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-carousel__indicator", $"el-carousel__indicator--{Direction.ToString().ToLower()}")
                .AddIf(index == activeIndex, "is-active")
                .ToString();
        }

        public async Task SetActiveIndexAsync(int index)
        {
            if (Items.Count == 0)
            {
                activeIndex = 0;
                return;
            }

            activeIndex = (index + Items.Count) % Items.Count;
            if (ActiveIndexChanged.HasDelegate)
            {
                await ActiveIndexChanged.InvokeAsync(activeIndex);
            }

            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(activeIndex);
            }

            Refresh();
        }

        protected async Task ShowPreviousAsync()
        {
            await SetActiveIndexAsync(activeIndex - 1);
        }

        protected async Task ShowNextAsync()
        {
            await SetActiveIndexAsync(activeIndex + 1);
        }

        protected void PauseTimer()
        {
            timer?.Stop();
        }

        protected void ResumeTimer()
        {
            if (Autoplay && Items.Count > 1)
            {
                timer?.Start();
            }
        }

        private void EnsureTimer()
        {
            if (timer == null)
            {
                timer = new Timer(Math.Max(500, Interval));
                timer.Elapsed += OnTimerElapsed;
                timer.AutoReset = true;
            }

            timer.Interval = Math.Max(500, Interval);
            if (Autoplay && Items.Count > 1)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
            }
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            await InvokeAsync(async () => await ShowNextAsync());
        }

        private void NormalizeActiveIndex()
        {
            if (items.Count == 0)
            {
                activeIndex = 0;
                return;
            }

            if (activeIndex >= items.Count)
            {
                activeIndex = items.Count - 1;
            }
        }

        public override void Dispose()
        {
            if (timer != null)
            {
                timer.Elapsed -= OnTimerElapsed;
                timer.Dispose();
            }

            base.Dispose();
        }
    }
}
