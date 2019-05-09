using Blazui.Component.Input.Category;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Input.InputRazor
{
    public class EInputBase : ComponentBase
    {

        [Parameter]
        public InputType IType { get; set; }

        [Parameter]
        public bool IsDisable { get; set; }

        [Parameter]
        public bool Maxlength { get; set; }
    }
}
