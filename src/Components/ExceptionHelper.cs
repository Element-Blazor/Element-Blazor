using System;
using System.Collections.Generic;
using System.Text;

namespace Element
{
    internal class ExceptionHelper
    {
        public static readonly int CascadingValueNotFound = 1;
        public static readonly int ExpandOnlyHasUrl = 3;

        public static readonly int DialogResultIsNull = 4;

        public static readonly int TabNameNotFound = 5;
        public static readonly int ClearChildFailure = 6;

        public static readonly int FormItemMustHaveName = 7;

        public static void Throw(int errCode, string message)
        {
            throw new BlazuiException(errCode, message);
        }
    }
}
