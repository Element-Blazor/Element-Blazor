using Element.ControlRender;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Element.ControlRenders
{
    internal class EmptyRender : IControlRender, IDatePickerRender
    {
        public void Render(RenderTreeBuilder renderTreeBuilder, RenderConfig formItemConfig)
        {
            renderTreeBuilder.OpenComponent(0, formItemConfig.InputControlType);
            renderTreeBuilder.AddAttribute(1, nameof(BFormItemObject.EnableAlwaysRender), true);
            renderTreeBuilder.CloseComponent();
        }
    }
}
