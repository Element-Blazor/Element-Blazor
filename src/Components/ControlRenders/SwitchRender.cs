using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Component.ControlRenders
{
    internal class SwitchRender : ISwitchRender
    {
        public void Render(RenderTreeBuilder renderTreeBuilder, RenderConfig config)
        {
            renderTreeBuilder.OpenComponent(0, config.InputControlType);
            renderTreeBuilder.AddAttribute(1, nameof(BFormItemObject.EnableAlwaysRender), true);
            renderTreeBuilder.AddAttribute(2, nameof(BSwitch<bool>.ActiveValue), true);
            renderTreeBuilder.AddAttribute(3, nameof(BSwitch<bool>.InactiveValue), false);
            renderTreeBuilder.CloseComponent();
        }
    }
}
