using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Element
{
    public partial class ElBreadcrumbItem : ElementComponentBase
    {
        [CascadingParameter]
        public ElBreadcrumb Breadcrumb { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string To { get; set; }

        [Parameter]
        public string Href
        {
            get => To;
            set => To = value;
        }

        [Parameter]
        public bool Replace { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected bool HasRoute => !string.IsNullOrWhiteSpace(To);

        protected string EffectiveSeparator => Breadcrumb?.Separator ?? "/";

        protected async Task OnClickAsync(MouseEventArgs e)
        {
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(e);
            }

            if (HasRoute)
            {
                NavigationManager.NavigateTo(To, replace: Replace);
            }
        }

        protected static string NormalizeIcon(string icon)
        {
            if (string.IsNullOrWhiteSpace(icon))
            {
                return null;
            }

            return icon.StartsWith("el-icon-") ? icon : $"el-icon-{icon}";
        }

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
