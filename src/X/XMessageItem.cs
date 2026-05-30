using Element;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Element.X
{
    public class XMessageItem
    {
        public string Id { get; set; }

        public XMessageRole Role { get; set; } = XMessageRole.Assistant;

        public string Content { get; set; }

        public string Header { get; set; }

        public string Footer { get; set; }

        public string Avatar { get; set; }

        public string AvatarText { get; set; }

        public string AvatarIcon { get; set; }

        public string Placement { get; set; }

        public bool Loading { get; set; }

        public bool Typing { get; set; }

        public IEnumerable<XAttachmentItem> Attachments { get; set; }

        public RenderFragment ContentTemplate { get; set; }

        public RenderFragment Actions { get; set; }
    }
}
