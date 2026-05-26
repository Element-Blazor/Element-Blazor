using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Element.ControlRenders
{
    internal class SwitchRender : ISwitchRender
    {
        public void Render(RenderTreeBuilder renderTreeBuilder, RenderConfig config)
        {
            var valueType = config.ValueType ?? config.InputControlType.GetGenericArguments()[0];
            renderTreeBuilder.OpenComponent(0, config.InputControlType);
            renderTreeBuilder.AddAttribute(1, nameof(ElFormItemObject.EnableAlwaysRender), true);
            renderTreeBuilder.AddAttribute(2, nameof(ElSwitch<bool>.ActiveValue), ConvertSwitchValue(true, valueType));
            renderTreeBuilder.AddAttribute(3, nameof(ElSwitch<bool>.InactiveValue), ConvertSwitchValue(false, valueType));
            var value = config.EditingValue ?? config.RawValue;
            renderTreeBuilder.AddAttribute(4, nameof(ElSwitch<bool>.Value), ConvertSwitchValue(value, valueType));
            if (config.Page != null)
            {
                renderTreeBuilder.AddAttribute(5, nameof(ElSwitch<bool>.ValueChanged), CreateValueChanged(config, valueType));
            }
            renderTreeBuilder.CloseComponent();
        }

        private static object ConvertSwitchValue(object value, Type valueType)
        {
            var finalType = Nullable.GetUnderlyingType(valueType) ?? valueType;
            if (value == null)
            {
                return valueType == finalType ? Activator.CreateInstance(finalType) : null;
            }
            return TypeHelper.ChangeType(value, finalType);
        }

        private static object CreateValueChanged(RenderConfig config, Type valueType)
        {
            var createMethod = typeof(EventCallbackFactory).GetMethods().FirstOrDefault(x =>
            {
                if (!x.IsGenericMethod || !x.IsPublic)
                {
                    return false;
                }
                var parameters = x.GetParameters();
                if (parameters.Length < 2)
                {
                    return false;
                }
                return parameters[1].ParameterType.IsGenericType
                    && parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(Action<>);
            }).MakeGenericMethod(valueType);
            var parameter = Expression.Parameter(valueType);
            var body = Expression.Assign(Expression.Property(Expression.Constant(config), nameof(RenderConfig.EditingValue)), Expression.Convert(parameter, typeof(object)));
            var setter = Expression.Lambda(typeof(Action<>).MakeGenericType(valueType), body, parameter).Compile();
            return createMethod.Invoke(EventCallback.Factory, new object[] { config.Page, setter });
        }
    }
}
