using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElAnchor : ElementComponentBase
    {
        private DotNetObjectReference<ElAnchor> objectReference;
        private string activeHref;
        private string optionsKey;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Container { get; set; }

        [Parameter]
        public int Offset { get; set; }

        [Parameter]
        public int Bound { get; set; } = 15;

        [Parameter]
        public int Duration { get; set; } = 300;

        [Parameter]
        public bool Marker { get; set; } = true;

        [Parameter]
        public AnchorType Type { get; set; }

        [Parameter]
        public AnchorDirection Direction { get; set; }

        [Parameter]
        public string ModelValue { get; set; }

        [Parameter]
        public EventCallback<string> ModelValueChanged { get; set; }

        [Parameter]
        public string Value
        {
            get => ModelValue;
            set => ModelValue = value;
        }

        [Parameter]
        public EventCallback<string> ValueChanged
        {
            get => ModelValueChanged;
            set => ModelValueChanged = value;
        }

        [Parameter]
        public EventCallback<string> OnChange { get; set; }

        [Parameter]
        public EventCallback<AnchorClickEventArgs> OnClick { get; set; }

        public ElementReference RootElement { get; set; }

        protected string AnchorClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-anchor", Cls)
            .Add($"el-anchor--{Direction.ToString().ToLowerInvariant()}")
            .AddIf(Type == AnchorType.Underline, "el-anchor--underline")
            .ToString();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (!string.IsNullOrWhiteSpace(ModelValue))
            {
                activeHref = ModelValue;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            var nextOptionsKey = $"{Container}|{Offset}|{Bound}|{Duration}";
            if (optionsKey == nextOptionsKey)
            {
                return;
            }

            optionsKey = nextOptionsKey;
            objectReference ??= DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("elementAnchorInit", RootElement, objectReference, new
            {
                container = Container,
                offset = Offset,
                bound = Bound,
                duration = Duration
            });
        }

        internal bool IsActive(string href)
        {
            return !string.IsNullOrWhiteSpace(href) && href == activeHref;
        }

        internal async Task HandleLinkClickAsync(ElAnchorLink link, MouseEventArgs mouseEventArgs)
        {
            await SetActiveHrefAsync(link.Href, true);
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(new AnchorClickEventArgs
                {
                    Href = link.Href,
                    Title = link.EffectiveTitle,
                    MouseEventArgs = mouseEventArgs
                });
            }

            await JSRuntime.InvokeVoidAsync("elementAnchorScrollTo", link.Href, Container, Offset, Duration);
        }

        [JSInvokable]
        public Task SetActiveHref(string href)
        {
            return SetActiveHrefAsync(href, true);
        }

        private async Task SetActiveHrefAsync(string href, bool notify)
        {
            if (activeHref == href)
            {
                return;
            }

            activeHref = href;
            ModelValue = href;
            if (ModelValueChanged.HasDelegate)
            {
                await ModelValueChanged.InvokeAsync(href);
            }
            if (notify && OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(href);
            }
            await InvokeAsync(StateHasChanged);
        }

        public override void Dispose()
        {
            objectReference?.Dispose();
            base.Dispose();
        }
    }
}
