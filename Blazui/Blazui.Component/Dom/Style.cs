using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazui.Component.Dom
{
    public class Style
    {
        private ElementRef elementRef;
        private IJSRuntime jSRuntime;

        public Style(ElementRef elementRef, IJSRuntime jSRuntime)
        {
            this.elementRef = elementRef;
            this.jSRuntime = jSRuntime;
        }

        public async Task<int> GetPaddingLeftAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getPaddingLeft", elementRef);
        }

        public async Task<int> GetPaddingRightAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getPaddingRight", elementRef);
        }
    }
}
