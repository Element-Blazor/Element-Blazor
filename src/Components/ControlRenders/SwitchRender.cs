using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component.ControlRenders
{
    internal class SwitchRender : ISwitchRender
    {
        public object Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Render(RenderTreeBuilder renderTreeBuilder, FormItemConfig config)
        {
            renderTreeBuilder.OpenComponent(0, config.InputControl);
            renderTreeBuilder.AddAttribute(1, nameof(BFormItemObject.EnableAlwaysRender), true);
            renderTreeBuilder.AddAttribute(2, nameof(BSwitch<bool>.ActiveValue), true);
            renderTreeBuilder.AddAttribute(3, nameof(BSwitch<bool>.InactiveValue), false);
            renderTreeBuilder.CloseComponent();
        }
    }
}
