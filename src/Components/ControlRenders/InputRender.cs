using Element.ControlConfigs;
using Element.ControlRender;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Element.ControlRenders
{
    internal class InputRender : RenderBase, IInputRender
    {
        public void Render(RenderTreeBuilder builder, RenderConfig config)
        {
            var inputConfig = (InputAttribute)config.ControlAttribute;
            Console.WriteLine(config.InputControlType);
            builder.OpenComponent(0, config.InputControlType);
            if (inputConfig != null)
            {
                builder.AddAttribute(2, nameof(inputConfig.IsClearable), inputConfig.IsClearable);
                builder.AddAttribute(3, nameof(inputConfig.IsDisabled), inputConfig.IsDisabled);
                builder.AddAttribute(4, nameof(inputConfig.Placeholder), inputConfig.Placeholder);
                builder.AddAttribute(5, nameof(inputConfig.PrefixIcon), inputConfig.PrefixIcon);
                builder.AddAttribute(6, nameof(inputConfig.SuffixIcon), inputConfig.SuffixIcon);
                builder.AddAttribute(7, nameof(inputConfig.Type), inputConfig.Type);
                builder.AddAttribute(8, nameof(inputConfig.Image), inputConfig.Image);
                builder.AddAttribute(9, nameof(inputConfig.Style), inputConfig.Style);
            }
            builder.AddAttribute(10, nameof(BFormItemObject.EnableAlwaysRender), true);
            CreateBind(config, builder, 11);
            builder.CloseComponent();
        }
    }
}
