using Blazui.Component.ControlConfigs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Component.ControlRenders
{
    public class CheckBoxRender : ICheckBoxRender
    {
        public void Render(RenderTreeBuilder renderTreeBuilder, RenderConfig config)
        {
            var inputConfig = (CheckBoxAttribute)config.ControlAttribute;
            renderTreeBuilder.OpenComponent(0, config.InputControlType);
            renderTreeBuilder.AddAttribute(1, nameof(BFormItemObject.EnableAlwaysRender), true);
            if (inputConfig != null)
            {
                renderTreeBuilder.AddAttribute(2, nameof(inputConfig.Image), inputConfig.Image);
                renderTreeBuilder.AddAttribute(3, nameof(inputConfig.Style), inputConfig.Style);
                renderTreeBuilder.AddAttribute(4, nameof(BFormItemObject.ChildContent), inputConfig.Text);
            }
            renderTreeBuilder.CloseComponent();
        }
    }
}
