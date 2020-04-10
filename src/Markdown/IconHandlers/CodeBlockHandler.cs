using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Markdown.IconHandlers
{
    public class CodeBlockHandler : IIconHandler
    {
        private readonly IJSRuntime jSRuntime;

        public CodeBlockHandler(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }

        public async Task HandleAsync(BMarkdownEditorBase editor)
        {
            var selection = await jSRuntime.InvokeAsync<string>("getSelection", editor.textarea);
            var result = $"```{Environment.NewLine}{(string.IsNullOrWhiteSpace(selection) ? "code" : selection)}{Environment.NewLine}```";
            await jSRuntime.InvokeVoidAsync("append", editor.Textarea, result);
        }
    }
}
