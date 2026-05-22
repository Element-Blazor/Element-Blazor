using Element;
using Markdig;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Element.Markdown
{
    public partial class ElMarkdown : ElementComponentBase
    {
        private MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        /// <summary>
        /// Markdown ÎÄ±¾
        /// </summary>
        [Parameter]
        public string Text { get; set; }

        internal MarkupString html;
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            html = (MarkupString)Markdig.Markdown.ToHtml(Text ?? string.Empty, pipeline);
        }
    }
}
