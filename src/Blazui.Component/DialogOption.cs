using Blazui.Component.Popup;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class DialogOption
    {
        public string Title { get; set; }
        public object Content { get; set; }
        public bool IsDialog { get; set; }
        public float Width { get; set; }
        public bool IsShow { get; set; }
        public IList<RenderFragment> Buttons { get; set; } = new List<RenderFragment>();
        internal int ZIndex { get; set; }
        internal int ShadowZIndex { get; set; }
        internal TaskCompletionSource<DialogResult> TaskCompletionSource { get; set; }
        internal BPopupBase Instance { get; set; }
        internal ElementReference Element { get; set; }
        internal ElementReference ShadowElement { get; set; }
        internal bool IsNew { get; set; }
    }
}
