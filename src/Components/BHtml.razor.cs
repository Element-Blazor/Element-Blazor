using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public partial class BHtml
    {
        [Parameter]
        public string Html { get; set; }

        public void SetHtml(string htmlString)
        {
            Html = htmlString;
        }

        public void Refresh()
        {
            StateHasChanged();
        }
    }
}
