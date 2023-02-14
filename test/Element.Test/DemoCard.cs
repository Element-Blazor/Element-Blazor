using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Element.Test
{
    public class DemoCard
    {
        public string Title { get; set; }
        public IElementHandle Body { get; set; }

        public IPage Page { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
