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

        public Element(ElementRef elementRef, IJSRuntime jSRuntime)
        {
            this.elementRef = elementRef;
            this.jSRuntime = jSRuntime;
        }

        public async Task<int> GetClientWidthAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getClientWidth", elementRef);
        }

        public async Task<int> GetOffsetLeftAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getOffsetLeft", elementRef);
        }
        public int ClientHeight
        {
            get
            {
                return jSRuntime.InvokeAsync<int>("getClientHeight", elementRef).GetAwaiter().GetResult();
            }
        }
    }
}
