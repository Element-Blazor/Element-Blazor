using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public enum BadgeType
    {
        Primary = 0,
        Success = 1,
        Warning = 2,
        [Obsolete("Use Danger instead.")]
        Dnger = 3,
        Danger = 3,
        Info = 4
    }
}
