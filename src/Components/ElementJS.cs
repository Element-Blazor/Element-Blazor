using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class ElementJS
    {
        private readonly ElementReference elementReference;
        private readonly IJSRuntime jSRuntime;
        public Style Style { get; }

        public ElementJS(ElementReference elementReference, IJSRuntime jSRuntime)
        {
            this.elementReference = elementReference;
            this.jSRuntime = jSRuntime;
            this.Style = new Style(elementReference, jSRuntime);
        }

        public async Task<string[]> UploadFileAsync(string fileName, string url)
        {
            return await jSRuntime.InvokeAsync<string[]>("uploadFile", elementReference, fileName, url);
        }

        public async Task<string[][]> ScanFilesAsync()
        {
            var files = await jSRuntime.InvokeAsync<dynamic>("scanFiles", elementReference);
            return JsonSerializer.Deserialize<string[][]>(files.ToString());
        }

        public async Task ClickAsync()
        {
            await jSRuntime.InvokeVoidAsync("click", elementReference);
        }

        public async Task TriggerAsync(string eventName)
        {
            await jSRuntime.InvokeVoidAsync("trigger", elementReference, eventName);
        }

        public async Task<int> GetClientWidthAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getClientWidth", elementReference);
        }
        public async Task<int> GetClientHeightAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getClientHeight", elementReference);
        }

        public async Task SubmitAsync(string url)
        {
            await jSRuntime.InvokeVoidAsync("submitForm", elementReference, url);
        }

        internal void RegisterAnimationEndAsync()
        {
            
        }

        public async Task<int> GetOffsetLeftAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getOffsetLeft", elementReference);
        }

        public ValueTask ClearAsync()
        {
            return jSRuntime.InvokeVoidAsync("clear", elementReference);
        }

        public async Task<int> GetOffsetTopAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getOffsetTop", elementReference);
        }

        public ValueTask FocusAsync()
        {
            return jSRuntime.InvokeVoidAsync("execFocus", elementReference);
        }

        public async Task ChildMoveToBodyAsync()
        {
            await jSRuntime.InvokeAsync<object>("childMoveToBody", elementReference);
        }

        public async Task<ElementReference> GetChildAsync(int index)
        {
            return await jSRuntime.InvokeAsync<ElementReference>("getChild", elementReference, index);
        }

        public async Task<BoundingClientRect> GetBoundingClientRectAsync()
        {
            return await jSRuntime.InvokeAsync<BoundingClientRect>("getBoundingClientRect", elementReference);
        }

        public async Task<float> GetTopRelativeBodyAsync()
        {
            return await jSRuntime.InvokeAsync<float>("getTopRelativeBody", elementReference);
        }

        public async Task SetDisabledAsync(bool isDisabled)
        {
            await jSRuntime.InvokeVoidAsync("setDisabled", elementReference, isDisabled);
        }

        public async Task RemoveAsync()
        {
            await jSRuntime.InvokeAsync<int>("removeSelf", elementReference);
        }

        public async Task AppendAsync(ElementReference element)
        {
            await jSRuntime.InvokeAsync<object>("elementAppendChild", elementReference, element);
        }
    }
}
