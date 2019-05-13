using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Input
{
    public class InputBase : ComponentBase
    {
        [Parameter]
        public string InputValue { get; set; }

        [Parameter]
        public string Placeholder { get; set; } = "请输入内容";
        [Parameter]
        public bool IsDisabled { get; set; } = false;
        [Parameter]
        public bool IsClearable { get; set; } = false;
    }
}
