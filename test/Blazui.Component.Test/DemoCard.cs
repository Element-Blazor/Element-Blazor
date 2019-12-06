using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component.Test
{
    public class DemoCard
    {
        public string Title { get; set; }
        public ElementHandle Body { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
