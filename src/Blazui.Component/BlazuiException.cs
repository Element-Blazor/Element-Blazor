using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BlazuiException : Exception
    {
        public BlazuiException(string message)
            : base(message)
        {

        }

        public BlazuiException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
