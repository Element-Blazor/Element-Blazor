using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElAnchorLink : ElementComponentBase
    {
        [CascadingParameter]
        public ElAnchor Anchor { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Href { get; set; }

        [Parameter]
        public RenderFragment TitleContent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        internal string EffectiveTitle => string.IsNullOrWhiteSpace(Title) ? Href : Title;

        protected string ItemClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-anchor__item", Cls)
            .AddIf(Anchor?.IsActive(Href) == true, "is-active")
            .ToString();

        protected async Task OnClickAsync(MouseEventArgs e)
        {
            if (Anchor != null)
            {
                await Anchor.HandleLinkClickAsync(this, e);
                return;
            }

            if (!string.IsNullOrWhiteSpace(Href))
            {
                NavigationManager.NavigateTo(Href);
            }
        }
    }
}
