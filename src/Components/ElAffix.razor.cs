using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElAffix : ElementComponentBase
    {
        private DotNetObjectReference<ElAffix> objectReference;
        private string optionsKey;
        private bool isFixed;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public int Offset { get; set; }

        [Parameter]
        public AffixPosition Position { get; set; } = AffixPosition.Top;

        [Parameter]
        public string Target { get; set; }

        [Parameter]
        public int ZIndex { get; set; } = 100;

        [Parameter]
        public EventCallback<bool> OnChange { get; set; }

        [Parameter]
        public EventCallback<AffixScrollEventArgs> OnScroll { get; set; }

        public ElementReference RootElement { get; set; }

        protected string AffixClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-affix", Cls)
            .AddIf(isFixed, "is-fixed")
            .ToString();

        protected string AffixContentClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-affix__content")
            .AddIf(isFixed, "el-affix--fixed")
            .ToString();

        protected string AffixContentStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(isFixed, $"z-index:{ZIndex}")
            .ToString();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            var nextOptionsKey = $"{Offset}|{Position}|{Target}|{ZIndex}";
            if (optionsKey == nextOptionsKey)
            {
                return;
            }

            optionsKey = nextOptionsKey;
            objectReference ??= DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("elementAffixInit", RootElement, objectReference, new
            {
                offset = Offset,
                position = Position.ToString().ToLowerInvariant(),
                target = Target,
                zIndex = ZIndex
            });
        }

        [JSInvokable]
        public async Task SetFixed(bool nextFixed, int scrollTop)
        {
            var changed = isFixed != nextFixed;
            isFixed = nextFixed;

            if (OnScroll.HasDelegate)
            {
                await OnScroll.InvokeAsync(new AffixScrollEventArgs
                {
                    Fixed = nextFixed,
                    ScrollTop = scrollTop
                });
            }

            if (changed && OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(nextFixed);
            }

            if (changed)
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        public override void Dispose()
        {
            objectReference?.Dispose();
            base.Dispose();
        }
    }
}
