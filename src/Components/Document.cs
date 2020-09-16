using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    internal class Document
    {
        private readonly IJSRuntime jSRuntime;

        public Document(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }

        public async Task AppendAsync(ElementReference content)
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
        public async Task EnableYScrollAsync()
        {
            await jSRuntime.InvokeAsync<object>("enableYScroll");
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
        public async Task<int> GetClientHeightAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getClientHeight");
        }

        internal ValueTask RegisterPasteUploadAsync(BUpload upload, string url)
        {
            return jSRuntime.InvokeVoidAsync("registerPasteUpload", DotNetObjectReference.Create(upload), url);
        }

        internal ValueTask UnRegisterPasteUploadAsync()
        {
            return jSRuntime.InvokeVoidAsync("unRegisterPasteUpload");
        }
    }
}
