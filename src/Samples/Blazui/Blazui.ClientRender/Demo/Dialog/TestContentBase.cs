
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Blazui.Component;
using System.Threading.Tasks;

namespace Blazui.ClientRender.Demo.Dialog
{
    public class TestContentBase:BDialogBase
    {
        [Parameter]
        public string Name { get; set; }
    }
}
