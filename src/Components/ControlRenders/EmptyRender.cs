using Blazui.Component.ControlRender;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component.ControlRenders
{
    internal class EmptyRender : IControlRender, IInputRender, IDatePickerRender
    {
        public object Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Render(RenderTreeBuilder renderTreeBuilder, FormItemConfig formItemConfig)
        {
            renderTreeBuilder.OpenComponent(0, formItemConfig.InputControl);
            renderTreeBuilder.AddAttribute(1, nameof(BFormItemObject.EnableAlwaysRender), true);
            renderTreeBuilder.CloseComponent();
        }
    }
}
