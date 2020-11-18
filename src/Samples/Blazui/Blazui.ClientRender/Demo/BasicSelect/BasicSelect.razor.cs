using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.ClientRender.Demo.BasicSelect
{
    public partial class BasicSelect : ComponentBase
    {
        protected int Value;

        protected Option Option;
        protected Option? NullableOption;
    }
}
