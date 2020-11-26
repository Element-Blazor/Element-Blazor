

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Element;
namespace Element.Demo.CheckBox
{
    public partial class HardCode : BComponentBase
    {
        public object Value { get; set; }
        public Status Status = Status.Checked;
        public Status Status2 = Status.Checked;
    }
}
