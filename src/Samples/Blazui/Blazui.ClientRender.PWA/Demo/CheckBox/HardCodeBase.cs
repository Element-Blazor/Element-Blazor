

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blazui.Component;
namespace Blazui.ClientRender.PWA.Demo.CheckBox
{
    public class HardCodeBase : ComponentBase
    {
        public object Value { get; set; }
        public Status Status = Status.Checked;
        public Status Status2 = Status.Checked;
    }
}
