using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Timers;

namespace Element
{
    public partial class ElCountdown : ElementComponentBase
    {
        private Timer timer;
        private TimeSpan remain;

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment TitleContent { get; set; }

        [Parameter]
        public DateTime Value { get; set; }

        [Parameter]
        public string Format { get; set; } = "HH:mm:ss";

        [Parameter]
        public string PrefixText { get; set; }

        [Parameter]
        public RenderFragment Prefix { get; set; }

        [Parameter]
        public string SuffixText { get; set; }

        [Parameter]
        public RenderFragment Suffix { get; set; }

        [Parameter]
        public EventCallback OnFinish { get; set; }

        protected string DisplayValue => FormatRemain();

        protected string CountdownClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-statistic", "el-countdown", Cls)
            .ToString();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            UpdateRemain();
            timer = new Timer(1000);
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = true;
            if (remain > TimeSpan.Zero)
            {
                timer.Start();
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            UpdateRemain();
            if (timer != null && remain > TimeSpan.Zero && !timer.Enabled)
            {
                timer.Start();
            }
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            UpdateRemain();
            await InvokeAsync(StateHasChanged);
            if (remain <= TimeSpan.Zero)
            {
                timer?.Stop();
                if (OnFinish.HasDelegate)
                {
                    await OnFinish.InvokeAsync(null);
                }
            }
        }

        private void UpdateRemain()
        {
            remain = Value - DateTime.Now;
            if (remain < TimeSpan.Zero)
            {
                remain = TimeSpan.Zero;
            }
        }

        private string FormatRemain()
        {
            if (string.IsNullOrWhiteSpace(Format))
            {
                return remain.ToString();
            }

            var totalHours = ((int)remain.TotalHours).ToString(CultureInfo.InvariantCulture);
            return Format
                .Replace("DD", ((int)remain.TotalDays).ToString(CultureInfo.InvariantCulture))
                .Replace("HH", totalHours.PadLeft(2, '0'))
                .Replace("mm", remain.Minutes.ToString("00", CultureInfo.InvariantCulture))
                .Replace("ss", remain.Seconds.ToString("00", CultureInfo.InvariantCulture))
                .Replace("S", remain.Milliseconds.ToString("000", CultureInfo.InvariantCulture));
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
