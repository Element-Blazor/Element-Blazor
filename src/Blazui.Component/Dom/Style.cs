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
        private ElementReference elementReference;
        private IJSRuntime jSRuntime;

        public Style(ElementReference elementReference, IJSRuntime jSRuntime)
        {
            this.elementReference = elementReference;
            this.jSRuntime = jSRuntime;
        }

        public async Task<int> GetPaddingLeftAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getPaddingLeft", elementReference);
        }

        public async Task<int> GetPaddingRightAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getPaddingRight", elementReference);
        }

        public async Task<int> GetLeftAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getLeft", elementReference);
        }

        public async Task<int> GetTopAsync()
        {
            return await jSRuntime.InvokeAsync<int>("getTop", elementReference);
        }

        public async Task SetTransformAsync(string value)
        {
            await jSRuntime.InvokeAsync<object>("setTransform", elementReference, value);
        }
        public async Task<Style> SetAsync(string key, string value)
        {
            await jSRuntime.InvokeAsync<object>("setStyle", elementReference, key, value);
            return this;
        }

        public async Task<Style> ClearAsync(string key)
        {
            await jSRuntime.InvokeAsync<object>("clearStyle", elementReference, key);
            return this;
        }

        public async Task<Style> SetTransitionAsync(string value)
        {
            await jSRuntime.InvokeAsync<object>("setTransitionAsync", elementReference, value);
            return this;
        }

        public async Task<string> GetMarginTopAsync()
        {
            return await jSRuntime.InvokeAsync<string>("getMarginTop", elementReference);
        }
    }
}
