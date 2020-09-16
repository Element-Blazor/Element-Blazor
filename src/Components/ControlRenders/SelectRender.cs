using Blazui.Component.ControlConfigs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Blazui.Component.ControlRenders
{
    internal class SelectRender : RenderBase, ISelectRender
    {
        protected override void CreateBind(RenderConfig config, RenderTreeBuilder builder, int startIndex)
        {
            base.CreateBind(config, builder, startIndex);
            builder.AddAttribute(startIndex + 3, nameof(BSelect<string>.LabelChanged), (Action<string>)(label => config.RawLabel = label));
            builder.AddAttribute(startIndex + 4, nameof(BSelect<string>.Label), config.RawLabel?.ToString());
        }
        public void Render(RenderTreeBuilder renderTreeBuilder, RenderConfig renderConfig)
        {
            var valueType = renderConfig.Property.PropertyType;
            var nullValueType = Nullable.GetUnderlyingType(valueType);
            var finalValueType = nullValueType ?? valueType;
            if (renderConfig.InputControlType.IsGenericTypeDefinition && !finalValueType.IsEnum)
            {
                throw new BlazuiException("下拉框生成只支持List或枚举类型");
            }
            else if (renderConfig.InputControlType.IsGenericTypeDefinition)
            {
                renderConfig.InputControlType = renderConfig.InputControlType.MakeGenericType(valueType);
            }
            renderTreeBuilder.OpenComponent(0, renderConfig.InputControlType);
            try
            {
                var selectAttr = renderConfig.Property.GetCustomAttribute<SelectAttribute>();
                if (selectAttr != null)
                {
                    var dataSource = renderConfig.DataSource;
                    var dataSourceType = dataSource.GetType().GetGenericArguments()[0];
                    var valueProperty = dataSourceType.GetProperty(selectAttr.Value);
                    var textProperty = dataSourceType.GetProperty(selectAttr.Display);
                    renderTreeBuilder.AddAttribute(1, nameof(BSelect<string>.ChildContent), (RenderFragment)(builder =>
                    {
                        foreach (var item in dataSource as IEnumerable)
                        {
                            builder.OpenComponent(2, typeof(BSelectOption<>).MakeGenericType(valueProperty.PropertyType));
                            builder.AddAttribute(3, nameof(BSelectOption<string>.Value), valueProperty.GetValue(item));
                            builder.AddAttribute(4, nameof(BSelectOption<string>.Text), textProperty.GetValue(item));
                            builder.CloseComponent();
                        }
                    }));

                    CreateBind(renderConfig, renderTreeBuilder, 6);
                    renderTreeBuilder.AddComponentReferenceCapture(9, e => renderConfig.InputControl = e);
                    return;
                }
                if (!valueType.IsEnum)
                {
                    throw new BlazuiException("下拉框生成只支持List或枚举类型");
                }

                renderTreeBuilder.AddAttribute(6, nameof(BFormItemObject.ChildContent), (RenderFragment)(contentBuilder =>
                {
                    var names = Enum.GetNames(valueType);
                    for (int i = 0; i < names.Length; i++)
                    {
                        var name = names[i];
                        contentBuilder.OpenComponent(7, typeof(BSelectOption<>).MakeGenericType(renderConfig.Property.PropertyType));
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
                        contentBuilder.AddAttribute(8, nameof(BSelectOption<int>.Text), text ?? name);
                        contentBuilder.AddAttribute(9, nameof(BSelectOption<int>.Value), Enum.Parse(valueType, name));
                        contentBuilder.CloseComponent();
                    }
                }));
                renderTreeBuilder.AddAttribute(10, nameof(BFormItemObject.EnableAlwaysRender), true);
                if (!string.IsNullOrWhiteSpace(renderConfig.Placeholder))
                {
                    renderTreeBuilder.AddAttribute(11, nameof(BFormItemObject.Placeholder), renderConfig.Placeholder);
                }
                CreateBind(renderConfig, renderTreeBuilder, 13);
                renderTreeBuilder.AddComponentReferenceCapture(16, e => renderConfig.InputControl = e);

            }
            finally
            {
                renderTreeBuilder.CloseComponent();
            }


        }
    }
}
