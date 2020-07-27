using Blazui.Component.ControlConfigs;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component.ControlRenders
{
    public class CheckBoxRender : ICheckBoxRender
    {
        public object Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Render(RenderTreeBuilder renderTreeBuilder, FormItemConfig config)
        {
            var inputConfig = (CheckBoxAttribute)config.Config;
            renderTreeBuilder.OpenComponent(0, config.InputControl);
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
