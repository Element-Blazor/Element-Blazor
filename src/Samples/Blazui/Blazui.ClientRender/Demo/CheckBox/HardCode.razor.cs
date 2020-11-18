

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Element;
namespace Element.ClientRender.Demo.CheckBox
{
    public partial class HardCode : ComponentBase
    {
        public object Value { get; set; }
        public Status Status = Status.Checked;
        public Status Status2 = Status.Checked;
    }
}
