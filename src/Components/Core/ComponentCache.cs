using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element.Core
{
    public class ComponentCache : ConcurrentDictionary<Type, object>
    {
    }
}
