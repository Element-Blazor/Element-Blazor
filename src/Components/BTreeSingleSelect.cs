using Blazui.Component.Model;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BTreeSingleSelect : BSelect<int?>

    {
        public override Task SetParametersAsync(ParameterView parameters)
        {
            isTree = true;
            return base.SetParametersAsync(parameters);
        }
    }
}
