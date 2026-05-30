using Element;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Element.X
{
    public partial class ElXTypewriter : ElementComponentBase, IDisposable
    {
        private CancellationTokenSource cancellationTokenSource;
        private string lastContent;
        private int visibleLength;

        [Parameter]
        public string Content { get; set; }

        [Parameter]
        public int Speed { get; set; } = 24;

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool ShowCursor { get; set; } = true;

        [Parameter]
        public EventCallback OnComplete { get; set; }

        protected string DisplayText => string.IsNullOrEmpty(Content)
            ? string.Empty
            : Content.Substring(0, Math.Min(visibleLength, Content.Length));

        protected bool IsComplete => visibleLength >= (Content?.Length ?? 0);

        protected string TypewriterClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-typewriter", Cls)
            .AddIf(!IsComplete, "is-typing")
            .ToString();

        protected override void OnParametersSet()
        {
            if (lastContent == Content && !Disabled)
            {
                return;
            }

            lastContent = Content;
            visibleLength = Disabled ? Content?.Length ?? 0 : 0;
            cancellationTokenSource?.Cancel();
            if (!Disabled && !string.IsNullOrEmpty(Content))
            {
                cancellationTokenSource = new CancellationTokenSource();
                _ = TypeAsync(cancellationTokenSource.Token);
            }
        }

        public async Task CompleteAsync()
        {
            cancellationTokenSource?.Cancel();
            visibleLength = Content?.Length ?? 0;
            await InvokeAsync(StateHasChanged);
            if (OnComplete.HasDelegate)
            {
                await OnComplete.InvokeAsync();
            }
        }

        private async Task TypeAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && visibleLength < (Content?.Length ?? 0))
            {
                visibleLength++;
                await InvokeAsync(StateHasChanged);
                try
                {
                    await Task.Delay(Math.Max(1, Speed), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }

            if (!cancellationToken.IsCancellationRequested && OnComplete.HasDelegate)
            {
                await OnComplete.InvokeAsync();
            }
        }

        public override void Dispose()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            base.Dispose();
        }
    }
}
