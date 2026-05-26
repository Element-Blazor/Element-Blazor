using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElLink : ElementComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public LinkType Type { get; set; } = LinkType.Default;

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool Underline { get; set; } = true;

        [Parameter]
        public string Href { get; set; }

        [Parameter]
        public string Target { get; set; }

        [Parameter]
        public string Rel { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public RenderFragment IconContent { get; set; }

        [Parameter]
        public int? Tabindex { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected string LinkClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-link", Cls)
            .AddIf(Type != LinkType.Default, $"el-link--{Type.ToString().ToLowerInvariant()}")
            .AddIf(Disabled, "is-disabled")
            .AddIf(Underline && !Disabled, "is-underline")
            .ToString();

        protected string LinkStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add(Style)
            .ToString();

        private async Task OnLinkClickAsync(MouseEventArgs args)
        {
            if (Disabled || !OnClick.HasDelegate)
            {
                return;
            }

            await OnClick.InvokeAsync(args);
        }
    }
}
