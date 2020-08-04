using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Blazui.Component
{
    internal class PopupLayerOption
    {
        public int ShadowZIndex { get; internal set; }
        public BPopup Instance { get; internal set; }
        public bool IsNew { get; internal set; }
        public int ZIndex { get; internal set; }

        public PointF Position { get; set; }
        public RenderFragment Content { get; internal set; }
    }
}
