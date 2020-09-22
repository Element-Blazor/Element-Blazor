using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class BChangeEventArgs<T> : ChangeEventArgs
    {
        public T OldValue { get; set; }
        public T NewValue { get; set; }
        public bool DisallowChange { get; set; }
    }
}
