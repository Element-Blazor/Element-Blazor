using Blazui.Component.NavMenu;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class SubMenuOption : PopupOption
    {
        public Action Refresh { get; set; }
        public BSubMenuBase SubMenu { get; set; }
        public MenuOptions Options { get; set; }
        public RenderFragment Content { get; set; }
    }
}
