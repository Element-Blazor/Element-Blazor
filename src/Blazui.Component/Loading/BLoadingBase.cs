using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Loading
{
    public class BLoadingBase : ComponentBase
    {
        [Parameter]
        public string IconClass { get; set; }
        [Parameter]
        public string Text { get; set; } = "拼命加载中";
        [Parameter]
        public string Background { get; set; }
    }
}
