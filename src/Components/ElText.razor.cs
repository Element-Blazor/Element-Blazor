using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Element
{
    public partial class ElText : ElementComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TextType Type { get; set; } = TextType.Default;

        [Parameter]
        public TextSize Size { get; set; } = TextSize.Default;

        [Parameter]
        public bool Truncated { get; set; }

        [Parameter]
        public int? LineClamp { get; set; }

        [Parameter]
        public string Tag { get; set; } = "span";

        private Type TagType => typeof(ElementTagRenderer);

        private IDictionary<string, object> TextParameters => new Dictionary<string, object>
        {
            [nameof(ElementTagRenderer.Tag)] = string.IsNullOrWhiteSpace(Tag) ? "span" : Tag,
            [nameof(ElementTagRenderer.ChildContent)] = ChildContent,
            [nameof(ElementTagRenderer.Attributes)] = Attributes,
            [nameof(ElementTagRenderer.Class)] = TextClass,
            [nameof(ElementTagRenderer.Style)] = TextStyle
        };

        private string TextClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-text", Cls)
            .AddIf(Type != TextType.Default, $"el-text--{Type.ToString().ToLowerInvariant()}")
            .AddIf(Size != TextSize.Default, $"el-text--{Size.ToString().ToLowerInvariant()}")
            .AddIf(Truncated, "is-truncated")
            .AddIf(LineClamp.HasValue && LineClamp.Value > 0, "is-line-clamp")
            .ToString();

        private string TextStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(LineClamp.HasValue && LineClamp.Value > 0, $"--el-text-line-clamp:{LineClamp.Value}")
            .Add(Style)
            .ToString();
    }
}
