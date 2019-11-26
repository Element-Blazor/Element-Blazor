using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Dom
{
    public class Element
    {
        private readonly ElementReference elementReference;
        private readonly IJSRuntime jSRuntime;
        public Style Style { get; }

        public Element(ElementReference elementReference, IJSRuntime jSRuntime)
        {
            this.elementReference = elementReference;
            this.jSRuntime = jSRuntime;
            this.Style = new Style(elementReference, jSRuntime);
        }

        public async Task<int> GetClientWidthAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getClientWidth", elementReference);
        }
        public async Task<int> GetClientHeightAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getClientHeight", elementReference);
        }

        public async Task<int> GetOffsetLeftAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getOffsetLeft", elementReference);
        }

        public async Task<int> GetOffsetTopAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getOffsetTop", elementReference);
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
