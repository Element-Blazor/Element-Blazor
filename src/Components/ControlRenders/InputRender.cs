using Blazui.Component.ControlConfigs;
using Blazui.Component.ControlRender;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component.ControlRenders
{
    internal class InputRender : IInputRender
    {
        public object Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Render(RenderTreeBuilder renderTreeBuilder, FormItemConfig config)
        {
            var inputConfig = (InputAttribute)config.Config;
            renderTreeBuilder.OpenComponent(0, config.InputControl);
            renderTreeBuilder.AddAttribute(1, nameof(BFormItemObject.EnableAlwaysRender), true);
            if (inputConfig != null)
            {
                renderTreeBuilder.AddAttribute(2, nameof(inputConfig.IsClearable), inputConfig.IsClearable);
                renderTreeBuilder.AddAttribute(3, nameof(inputConfig.IsDisabled), inputConfig.IsDisabled);
                renderTreeBuilder.AddAttribute(4, nameof(inputConfig.Placeholder), inputConfig.Placeholder);
                renderTreeBuilder.AddAttribute(5, nameof(inputConfig.PrefixIcon), inputConfig.PrefixIcon);
                renderTreeBuilder.AddAttribute(6, nameof(inputConfig.SuffixIcon), inputConfig.SuffixIcon);
                renderTreeBuilder.AddAttribute(7, nameof(inputConfig.Type), inputConfig.Type);
                renderTreeBuilder.AddAttribute(8, nameof(inputConfig.Image), inputConfig.Image);
                renderTreeBuilder.AddAttribute(9, nameof(inputConfig.Style), inputConfig.Style);
            }
            renderTreeBuilder.CloseComponent();
        }
    }
}
