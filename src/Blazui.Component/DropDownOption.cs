using Blazui.Component.Popup;
using Blazui.Component.Select;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class DropDownOption : PopupOption
    {
        public Action Refresh { get; set; }
        public object Select { get; set; }
        public RenderFragment OptionContent { get; set; }
        internal float Width { get; set; }
    }
}
