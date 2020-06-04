using Blazui.Component.ControlRender;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component
{
    internal class FormItemConfig
    {
        public Type FormItem { get; set; }
        public bool IsRequired { get; set; }
        public string RequiredMessage { get; set; }
        public Type InputControl { get; internal set; }
        public IControlRender InputControlRender { get; set; }
        public string Label { get; internal set; }
        public string Image { get; internal set; }
        public float LabelWidth { get; internal set; }
        public string Placeholder { get; internal set; }
        public string Name { get; internal set; }
        public Type PropertyType { get; set; }

        public object Config { get; set; }
    }
}
