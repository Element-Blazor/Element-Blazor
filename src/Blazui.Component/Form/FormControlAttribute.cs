using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Form
{
    public class FormControlAttribute : Attribute
    {
        public FormControlAttribute(Type controlType)
        {
            ControlType = controlType;
        }

        public Type ControlType { get; }
    }
}
