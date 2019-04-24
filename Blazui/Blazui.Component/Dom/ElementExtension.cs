using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Dom
{
    public static class ElementExtension
    {
        public static Element Dom(this ElementRef elementRef, IJSRuntime jSRuntime)
        {
            return new Element(elementRef, jSRuntime);
        }
    }
}
