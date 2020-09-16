using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Markdown.IconHandlers
{
    public class HorizontalRuleHandler : IIconHandler
    {
        private readonly IJSRuntime jSRuntime;

        public HorizontalRuleHandler(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }

        public async Task HandleAsync(BMarkdownEditor editor)
        {
            await jSRuntime.InvokeVoidAsync("append", editor.Textarea, "---\n");
        }
    }
}
