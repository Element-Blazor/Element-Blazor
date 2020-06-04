using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Blazui.Component.ControlRenders
{
    internal class SelectRender : ISelectRender
    {
        public object Data { get; set; }

        public void Render(RenderTreeBuilder renderTreeBuilder, FormItemConfig formItemConfig)
        {
            var valueType = formItemConfig.PropertyType;
            var nullValueType = Nullable.GetUnderlyingType(valueType);
            var finalValueType = nullValueType ?? valueType;
            if (!valueType.IsEnum)
            {
                throw new BlazuiException("下拉框自动生成表单只支持枚举类型");
            }
            renderTreeBuilder.OpenComponent(0, formItemConfig.InputControl);
            renderTreeBuilder.AddAttribute(1, nameof(BFormItemObject.ChildContent), (RenderFragment)(contentBuilder =>
            {
                var names = Enum.GetNames(valueType);
                for (int i = 0; i < names.Length; i++)
                {
                    var name = names[i];
                    contentBuilder.OpenComponent(2, typeof(BSelectOption<>).MakeGenericType(formItemConfig.PropertyType));
                    var field = finalValueType.GetField(name);
                    var descAttr = field.GetCustomAttribute<DescriptionAttribute>();
                    var text = string.Empty;
                    if (descAttr != null)
                    {
                        text = descAttr.Description;
                    }
                    else
                    {
                        var displayAttr = field.GetCustomAttribute<DisplayAttribute>();
                        text = displayAttr?.Name ?? displayAttr?.Description;
                    }
                    contentBuilder.AddAttribute(3, nameof(BSelectOption<int>.Text), text ?? name);
                    contentBuilder.AddAttribute(4, nameof(BSelectOption<int>.Value), Enum.Parse(valueType, name));
                    contentBuilder.CloseComponent();
                }
            }));
            renderTreeBuilder.AddAttribute(5, nameof(BFormItemObject.EnableAlwaysRender), true);
            if (!string.IsNullOrWhiteSpace(formItemConfig.Placeholder))
            {
                renderTreeBuilder.AddAttribute(6, nameof(BFormItemObject.Placeholder), formItemConfig.Placeholder);
            }
            renderTreeBuilder.CloseComponent();




            
        }
    }
}
