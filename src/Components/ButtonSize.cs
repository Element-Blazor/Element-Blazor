using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public enum ButtonSize
    {
        Default = 0,

        Large = 1,

        Small = 2,

        [Obsolete("Element Plus 2.x uses Large/Default/Small. Medium is kept as a compatibility alias and renders as Large.")]
        Medium = Large,

        [Obsolete("Element Plus 2.x uses Large/Default/Small. Mini is kept as a compatibility alias and renders as Small.")]
        Mini = Small
    }
}
