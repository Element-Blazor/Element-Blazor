using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class ElementException : Exception
    {
        public ElementException(string message)
            : base(message)
        {

        }

        public ElementException(int code, string message):base(message)
        {
            Code = code;
        }

        public ElementException(string message, Exception exception)
            : base(message, exception)
        {
        }

        public int Code { get; }
    }
}
