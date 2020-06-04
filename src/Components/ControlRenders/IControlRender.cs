using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component.ControlRender
{
    interface IControlRender
    {
        object Data { get; set; }
        void Render(RenderTreeBuilder renderTreeBuilder, FormItemConfig config);
    }
}
