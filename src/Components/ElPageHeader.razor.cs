using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElPageHeader : ElementComponentBase
    {
        [Parameter]
        public string Title { get; set; } = "Back";

        [Parameter]
        public string Content { get; set; }

        [Parameter]
        public string Icon { get; set; } = "back";

        [Parameter]
        public RenderFragment IconContent { get; set; }

        [Parameter]
        public RenderFragment TitleContent { get; set; }

        [Parameter]
        public RenderFragment ContentContent { get; set; }

        [Parameter]
        public RenderFragment ExtraContent { get; set; }

        [Parameter]
        public RenderFragment BreadcrumbContent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnBack { get; set; }

        protected string PageHeaderClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-page-header", Cls)
            .ToString();

        protected string IconClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add($"el-icon-{Icon}")
            .ToString();

        protected Task OnBackAsync(MouseEventArgs e)
        {
            return OnBack.InvokeAsync(e);
        }

        protected async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" || e.Key == " ")
            {
                await OnBackAsync(null);
            }
        }
    }
}
