using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Element
{
    public partial class ElXBubble : ElementComponentBase
    {
        [Parameter]
        public XMessageRole Role { get; set; } = XMessageRole.Assistant;

        [Parameter]
        public string Placement { get; set; }

        [Parameter]
        public string Header { get; set; }

        [Parameter]
        public string Content { get; set; }

        [Parameter]
        public string Footer { get; set; }

        [Parameter]
        public string Avatar { get; set; }

        [Parameter]
        public string AvatarText { get; set; }

        [Parameter]
        public string AvatarIcon { get; set; }

        [Parameter]
        public bool Loading { get; set; }

        [Parameter]
        public bool Typing { get; set; }

        [Parameter]
        public int TypingSpeed { get; set; } = 20;

        [Parameter]
        public bool ShowAvatar { get; set; } = true;

        [Parameter]
        public IEnumerable<XAttachmentItem> Attachments { get; set; }

        [Parameter]
        public RenderFragment HeaderContent { get; set; }

        [Parameter]
        public RenderFragment FooterContent { get; set; }

        [Parameter]
        public RenderFragment AvatarContent { get; set; }

        [Parameter]
        public RenderFragment Actions { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string BubbleClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-bubble", Cls)
            .Add($"el-x-bubble--{Role.ToString().ToLowerInvariant()}")
            .AddIf(IsEndPlacement, "is-end")
            .AddIf(Loading, "is-loading")
            .ToString();

        protected string AvatarTextValue => !string.IsNullOrWhiteSpace(AvatarText)
            ? AvatarText
            : Role == XMessageRole.User ? "U" : "AI";

        private bool IsEndPlacement => string.Equals(Placement, "end", System.StringComparison.OrdinalIgnoreCase)
            || Role == XMessageRole.User;
    }
}
