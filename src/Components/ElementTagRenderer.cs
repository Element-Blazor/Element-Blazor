using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.Generic;

namespace Element
{
    public sealed class ElementTagRenderer : ComponentBase
    {
        [Parameter]
        public string Tag { get; set; } = "span";

        [Parameter]
        public string Class { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> Attributes { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var seq = 0;
            builder.OpenElement(seq++, string.IsNullOrWhiteSpace(Tag) ? "span" : Tag.Trim().ToLowerInvariant());
            if (Attributes != null)
            {
                builder.AddMultipleAttributes(seq++, Attributes);
            }

            builder.AddAttribute(seq++, "class", Class);
            builder.AddAttribute(seq++, "style", Style);
            builder.AddContent(seq++, ChildContent);
            builder.CloseElement();
        }
    }
}
