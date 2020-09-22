
using Element;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.ClientRender.Demo.BasicRadio
{
    public class DisallowChangeRadioBase : ComponentBase
    {
        protected string selectedValue = "1";

        protected void OnStatusChanging(BChangeEventArgs<RadioStatus> e)
        {
            e.DisallowChange = true;
        }
    }
}
