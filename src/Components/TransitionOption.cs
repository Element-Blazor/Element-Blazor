using System;
using System.Collections.Generic;
using System.Text;

namespace Element
{
    class TransitionOption
    {
        public int Delay { get; internal set; }
        public string Style { get; internal set; }
        public bool Increment { get; internal set; }
        public int Duration { get; internal set; }
        public bool? Pause { get; internal set; }
    }
}
