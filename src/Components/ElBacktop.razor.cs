using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElBacktop : ElementComponentBase
    {
        private DotNetObjectReference<ElBacktop> objectReference;
        private string optionsKey;
        private bool visible;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Target { get; set; }

        [Parameter]
        public int VisibilityHeight { get; set; } = 200;

        [Parameter]
        public int Right { get; set; } = 40;

        [Parameter]
        public int Bottom { get; set; } = 40;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        public ElementReference RootElement { get; set; }

        protected string BacktopClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-backtop", Cls)
            .AddIf(visible, "is-visible")
            .ToString();

        protected string BacktopStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add($"right:{Right}px")
            .Add($"bottom:{Bottom}px")
            .AddIf(!visible, "display:none")
            .Add(Style)
            .ToString();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            var nextOptionsKey = $"{Target}|{VisibilityHeight}";
            if (optionsKey == nextOptionsKey)
            {
                return;
            }

            optionsKey = nextOptionsKey;
            objectReference ??= DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("elementBacktopInit", RootElement, objectReference, new
            {
                target = Target,
                visibilityHeight = VisibilityHeight
            });
        }

        protected async Task OnInternalClickAsync(MouseEventArgs e)
        {
            await JSRuntime.InvokeVoidAsync("elementBacktopScrollTo", Target);
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(e);
            }
        }

        protected async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" || e.Key == " ")
            {
                await OnInternalClickAsync(null);
            }
        }

        [JSInvokable]
        public async Task SetVisible(bool nextVisible)
        {
            if (visible == nextVisible)
            {
                return;
            }

            visible = nextVisible;
            await InvokeAsync(StateHasChanged);
        }

        public override void Dispose()
        {
            objectReference?.Dispose();
            base.Dispose();
        }
    }
}
