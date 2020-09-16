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

        public BlazuiException(int code, string message):base(message)
        {
            Code = code;
        }

        public BlazuiException(string message, Exception exception)
            : base(message, exception)
        {
        }

        public int Code { get; }
    }
}
