using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Dom
{
    public class Element
    {
        private readonly ElementRef elementRef;

        public Element(ElementRef elementRef)
        {
            this.elementRef = elementRef;
        }
    }
}
