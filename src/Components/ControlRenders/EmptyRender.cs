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
            renderTreeBuilder.AddAttribute(1, nameof(ElFormItemObject.EnableAlwaysRender), true);
            CreateDatePickerBind(renderTreeBuilder, formItemConfig);
            renderTreeBuilder.CloseComponent();
        }

        private static void CreateDatePickerBind(RenderTreeBuilder renderTreeBuilder, RenderConfig formItemConfig)
        {
            if (formItemConfig.InputControlType != typeof(ElDatePicker))
            {
                return;
            }

            var value = formItemConfig.EditingValue ?? formItemConfig.RawValue;
            renderTreeBuilder.AddAttribute(2, nameof(ElDatePicker.Value), value as DateTime?);
            if (formItemConfig.Page == null)
            {
                return;
            }

            renderTreeBuilder.AddAttribute(3, nameof(ElDatePicker.ValueChanged), EventCallback.Factory.Create<DateTime?>(formItemConfig.Page, value =>
            {
                formItemConfig.EditingValue = value;
            }));
        }
    }
}
