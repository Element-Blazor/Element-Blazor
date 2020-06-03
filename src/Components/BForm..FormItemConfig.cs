using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component
{
    internal class FormItemConfig
    {
        public Type FormItem { get; set; }
        public bool IsRequired { get; set; }
        public int Index { get; internal set; }
        public Type InputControl { get; internal set; }
        public object Label { get; internal set; }
        public object Image { get; internal set; }
        public float LabelWidth { get; internal set; }
        public string Placeholder { get; internal set; }
        public string Name { get; internal set; }
    }
}
