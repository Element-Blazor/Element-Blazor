using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.EventArgs
{
    public class ChangeEventArgs<T> : UIChangeEventArgs
    {
        public T Target { get; set; }
    }
}
