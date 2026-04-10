using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.Demo.Select
{
    public partial class BasicSelect :BComponentBase
    {
        protected int Value;

        protected Option Option;
        protected Option? NullableOption;
    }
}
