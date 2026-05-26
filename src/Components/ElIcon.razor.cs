using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElIcon : ElementComponentBase
    {
        private string icon;

        [Parameter]
        public Icon Icon { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Size { get; set; }

        [Parameter]
        public string Color { get; set; }

        [Parameter]
        public bool Spin { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback OnClick { get; set; }

        protected string IconClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-icon", Cls)
            .AddIf(!string.IsNullOrWhiteSpace(icon), $"el-icon-{icon}")
            .AddIf(Spin, "is-loading")
            .ToString();

        protected string IconStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Size), $"font-size:{ElementCssUtility.NormalizeCssSize(Size)}")
            .AddIf(!string.IsNullOrWhiteSpace(Color), $"color:{Color}")
            .Add(Style)
            .ToString();

        protected override void OnParametersSet()
        {
            icon = !string.IsNullOrWhiteSpace(Name)
                ? NormalizeIconName(Name)
                : ChildContent == null
                    ? Icon.GetType().GetField(Icon.ToString()).GetCustomAttribute<DisplayAttribute>()?.Prompt
                    : null;
        }

        private async Task OnElClickAsync(MouseEventArgs e)
        {
            if (!OnClick.HasDelegate)
            {
                return;
            }

            await OnClick.InvokeAsync(e);
        }

        private static string NormalizeIconName(string value)
        {
            var trimmed = value.Trim();
            return trimmed.StartsWith("el-icon-", StringComparison.OrdinalIgnoreCase)
                ? trimmed.Substring("el-icon-".Length)
                : trimmed;
        }
    }
}
