using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Element
{
    internal class PopupLayerOption
    {
        public int ShadowZIndex { get; internal set; }
        public BPopup Instance { get; internal set; }
        public bool IsNew { get; internal set; }
        public int ZIndex { get; internal set; }

        public PointF Position { get; set; }
        public RenderFragment Content { get; internal set; }
        public bool IsShow { get; set; }
        public AnimationStatus ShowStatus { get; set; }
        public ElementReference Element { get; set; }
        public int Width { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public ElementReference ShadowElement { get; set; }
    }
}
