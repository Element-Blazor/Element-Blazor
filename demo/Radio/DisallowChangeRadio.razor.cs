
using Element;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.Demo.Radio
{
    public partial class DisallowChangeRadio : BComponentBase
    {
        protected string selectedValue = "1";

        protected void OnStatusChanging(BChangeEventArgs<RadioStatus> e)
        {
            e.DisallowChange = true;
        }
    }
}
