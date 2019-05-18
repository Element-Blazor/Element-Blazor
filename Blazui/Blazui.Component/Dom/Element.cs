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
        private readonly ElementRef elementRef;
        private readonly IJSRuntime jSRuntime;
        public Style Style { get; }

        public Element(ElementRef elementRef, IJSRuntime jSRuntime)
        {
            this.elementRef = elementRef;
            this.jSRuntime = jSRuntime;
            this.Style = new Style(elementRef, jSRuntime);
        }

        public async Task<int> GetClientWidthAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getClientWidth", elementRef);
        }

        public async Task<int> GetOffsetLeftAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getOffsetLeft", elementRef);
        }

        public async Task<int> GetOffsetTopAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getOffsetTop", elementRef);
        }

        public async Task ChildMoveToBodyAsync()
        {
            await jSRuntime.InvokeAsync<object>("childMoveToBody", elementRef);
        }

        public async Task<ElementRef> GetChildAsync(int index)
        {
            return await jSRuntime.InvokeAsync<ElementRef>("getChild", elementRef, index);
        }

        public async Task<BoundingClientRect> GetBoundingClientRectAsync()
        {
            return await jSRuntime.InvokeAsync<BoundingClientRect>("getBoundingClientRect", elementRef);
        }

        public int ClientHeight
        {
            get
            {
                return jSRuntime.InvokeAsync<int>("getClientHeight", elementRef).GetAwaiter().GetResult();
            }
        }

        public IReadOnlyList<ElementRef> Children
        {
            get
            {
                return jSRuntime.InvokeAsync<IReadOnlyList<ElementRef>>("getChildren", elementRef).GetAwaiter().GetResult();
            }
        }

        public async Task RemoveAsync()
        {
            await jSRuntime.InvokeAsync<int>("removeSelf", elementRef);
        }
    }
}
