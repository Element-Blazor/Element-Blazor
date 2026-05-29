using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElAlert : ElementComponentBase
    {
        private bool isClosed;

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment TitleContent { get; set; }

        [Parameter]
        public string Description { get; set; }

        [Parameter]
        public RenderFragment DescriptionContent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public MessageType Type { get; set; } = MessageType.Info;

        [Parameter]
        public AlertEffect Effect { get; set; } = AlertEffect.Light;

        [Parameter]
        public bool Closable { get; set; } = true;

        [Parameter]
        public string CloseText { get; set; }

        [Parameter]
        public bool ShowIcon { get; set; }

        [Parameter]
        public bool Center { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        private bool HasDescription => DescriptionContent != null || ChildContent != null || !string.IsNullOrWhiteSpace(Description);

        private string AlertClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-alert", $"el-alert--{Type.ToString().ToLowerInvariant()}", $"is-{Effect.ToString().ToLowerInvariant()}")
            .AddIf(Center, "is-center")
            .AddIf(ShowIcon, "is-icon")
            .AddIf(HasDescription, "is-description")
            .Add(Cls)
            .ToString();

        private string IconClass
        {
            get
            {
                var icon = Type == MessageType.Error ? "error" : Type.ToString().ToLowerInvariant();
                return HtmlPropertyBuilder.CreateCssClassBuilder()
                    .Add("el-alert__icon", $"el-icon-{icon}")
                    .AddIf(HasDescription, "is-big")
                    .ToString();
            }
        }

        private string CloseClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-alert__closebtn")
            .AddIf(string.IsNullOrWhiteSpace(CloseText), "el-icon-close")
            .AddIf(!string.IsNullOrWhiteSpace(CloseText), "is-customed")
            .ToString();

        private async Task CloseAsync()
        {
            isClosed = true;
            if (OnClose.HasDelegate)
            {
                await OnClose.InvokeAsync(null);
            }
        }
    }
}
