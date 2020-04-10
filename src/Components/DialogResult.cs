using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class DialogResult
    {
        public object Result { get; set; }
    }
    public class DialogResult<TResult>
    {
        public TResult Result { get; set; }
    }
}
