using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class FormControlAttribute : Attribute
    {
        public int SortNo { get; set; }
        public float LabelWidth { get; set; }
    }
}
