using Blazui.Component.ControlRender;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component
{
    internal class FormItemConfig : RenderConfig
    {
        public Type FormItem { get; set; }
        public IControlRender InputControlRender { get; set; }
        public string Label { get; internal set; }
        public string Image { get; internal set; }
        public float LabelWidth { get; internal set; }
        public string Name { get; internal set; }
    }
}
