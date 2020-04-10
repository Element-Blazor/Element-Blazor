using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BClosingEventArgs<T>
    {
        public T Target { get; set; }
        public bool Cancel { get; set; }
    }
}
