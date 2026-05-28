using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Element.ControlConfigs;
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
            var switchAttribute = config.ControlAttribute as SwitchAttribute;
            renderTreeBuilder.OpenComponent(0, config.InputControlType);
            renderTreeBuilder.AddAttribute(1, nameof(ElFormItemObject.EnableAlwaysRender), true);
            renderTreeBuilder.AddAttribute(2, nameof(ElSwitch<bool>.ActiveValue), ConvertSwitchValue(
                string.IsNullOrWhiteSpace(switchAttribute?.ActiveValue) ? true : switchAttribute.ActiveValue,
                valueType));
            renderTreeBuilder.AddAttribute(3, nameof(ElSwitch<bool>.InactiveValue), ConvertSwitchValue(
                string.IsNullOrWhiteSpace(switchAttribute?.InactiveValue) ? false : switchAttribute.InactiveValue,
                valueType));
            if (switchAttribute != null)
            {
                renderTreeBuilder.AddAttribute(7, nameof(ElSwitch<bool>.IsDisabled), switchAttribute.IsDisabled);
                renderTreeBuilder.AddAttribute(8, nameof(ElSwitch<bool>.ActiveText), switchAttribute.ActiveText);
                renderTreeBuilder.AddAttribute(9, nameof(ElSwitch<bool>.InactiveText), switchAttribute.InactiveText);
                renderTreeBuilder.AddAttribute(10, nameof(ElSwitch<bool>.ActiveColor), switchAttribute.ActiveColor);
                renderTreeBuilder.AddAttribute(11, nameof(ElSwitch<bool>.InactiveColor), switchAttribute.InactiveColor);
                renderTreeBuilder.AddAttribute(12, nameof(ElSwitch<bool>.Loading), switchAttribute.Loading);
                renderTreeBuilder.AddAttribute(13, nameof(ElSwitch<bool>.LoadingIcon), switchAttribute.LoadingIcon);
            }
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
                return valueType.IsValueType && Nullable.GetUnderlyingType(valueType) == null
                    ? Activator.CreateInstance(valueType)
                    : null;
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
