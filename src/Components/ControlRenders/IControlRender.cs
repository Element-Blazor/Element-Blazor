using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Component.ControlRender
{
    public interface IControlRender
    {
        void Render(RenderTreeBuilder renderTreeBuilder, RenderConfig config);
    }
}
