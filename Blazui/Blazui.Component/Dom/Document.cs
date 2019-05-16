using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Dom
{
    public class Document
    {
        private readonly IJSRuntime jSRuntime;

        public Document(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }

        public async Task AppendAsync(ElementRef content)
        {
            await jSRuntime.InvokeAsync<object>("documentAppendChild", content);
        }
    }
}
