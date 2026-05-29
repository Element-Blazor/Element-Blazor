using Microsoft.AspNetCore.Components;
using System;

namespace Element
{
    public partial class ElDescriptionsItem : ElementComponentBase
    {
        [CascadingParameter]
        public ElDescriptions Descriptions { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public RenderFragment LabelContent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public int Span { get; set; } = 1;

        [Parameter]
        public int Order { get; set; }

        [Parameter]
        public string Class { get; set; }

        [Parameter]
        public string LabelClass { get; set; }

        [Parameter]
        public string ContentClass { get; set; }

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public string MinWidth { get; set; }

        internal int NormalizedSpan => Math.Max(1, Math.Min(Span, Descriptions?.EffectiveColumn ?? Span));

        internal string CellClassValue => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-descriptions__cell", Class)
            .ToString();

        internal string CellStyleValue => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .Add($"grid-column:span {NormalizedSpan}")
            .AddIf(!string.IsNullOrWhiteSpace(Width), $"width:{ElementCssUtility.NormalizeCssSize(Width)}")
            .AddIf(!string.IsNullOrWhiteSpace(MinWidth), $"min-width:{ElementCssUtility.NormalizeCssSize(MinWidth)}")
            .ToString();

        internal string LabelClassValue => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-descriptions__label", LabelClass)
            .ToString();

        internal string ContentClassValue => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-descriptions__content", ContentClass)
            .ToString();

        internal RenderFragment RenderLabel()
        {
            return LabelContent ?? (builder => builder.AddContent(0, Label));
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Descriptions?.AddItem(this);
        }

        public override void Dispose()
        {
            Descriptions?.RemoveItem(this);
            base.Dispose();
        }
    }
}
