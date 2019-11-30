using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Dom
{
    internal class Document
    {
        private readonly IJSRuntime jSRuntime;

        public Document(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }

        public async Task AppendAsync(ElementReference  content)
        {
            await jSRuntime.InvokeAsync<object>("documentAppendChild", content);
        }

        public async Task DisableXScrollAsync()
        {
            await jSRuntime.InvokeAsync<object>("disableXScroll");
        }

        public async Task DisableYScrollAsync()
        {
            await jSRuntime.InvokeAsync<object>("disableYScroll");
        }

        public async Task DisableScrollAsync()
        {
            await DisableXScrollAsync();
            await DisableYScrollAsync();
        }

        public async Task RemoveAsync(ElementReference content)
        {
            await jSRuntime.InvokeAsync<object>("remove", content);
        }

        public async Task<int> GetClientWidthAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getClientWidth");
        }
    }
}
