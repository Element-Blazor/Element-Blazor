using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public static class ElementExtension
    {
        public static ElementJS Dom(this ElementReference elementReference  , IJSRuntime jSRuntime)
        {
            return new ElementJS(elementReference, jSRuntime);
        }

        public static async Task<IJSRuntime> AlertAsync(this IJSRuntime jSRuntime, string message)
        {
            await jSRuntime.InvokeAsync<object>("alert", message);
            return jSRuntime;
        }
        public static async Task<IJSRuntime> AlertAsync(this IJSRuntime jSRuntime, int message)
        {
            await jSRuntime.InvokeAsync<object>("alert", message.ToString());
            return jSRuntime;
        }
    }
}
