using Blazui.Component;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.Dialog
{
    public class TestContentBase:BDialogBase
    {
        [Parameter]
        public string Name { get; set; }
    }
}
