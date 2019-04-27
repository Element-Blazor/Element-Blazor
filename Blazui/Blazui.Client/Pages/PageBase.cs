using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Client.Pages
{
    public class PageBase : ComponentBase
    {
        [Inject]
        protected IJSRuntime jSRuntime { get; set; }
        protected override async Task OnAfterRenderAsync()
        {
        }
    }
}
