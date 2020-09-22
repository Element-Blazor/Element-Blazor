using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Element.ControlRenders
{
    public class RenderBase
    {
        protected virtual void CreateBind(RenderConfig config, RenderTreeBuilder builder, int startIndex)
        {
            Type valueType = CreateTwoWayBinding(config, builder, startIndex);
            var finalType = Nullable.GetUnderlyingType(valueType) ?? valueType;
            var editingValue = config.EditingValue?.ToString();
            if (finalType.IsEnum)
            {
                if (config.RawValue == null && finalType != valueType)
                {
                    builder.AddAttribute(startIndex + 1, nameof(BInput<string>.Value), (object)null);
                }
                else if (config.RawValue == null)
                {
                    builder.AddAttribute(startIndex + 1, nameof(BInput<string>.Value), Activator.CreateInstance(finalType));
                }
                else
                {
                    builder.AddAttribute(startIndex + 1, nameof(BInput<string>.Value), Enum.Parse(finalType, editingValue));
                }
            }
            else
            {
                builder.AddAttribute(startIndex + 1, nameof(BInput<string>.Value), Convert.ChangeType(editingValue, valueType));
            }
        }

        protected Type CreateTwoWayBinding(RenderConfig config, RenderTreeBuilder builder, int startIndex, string propertyName = nameof(BInput<string>.ValueChanged), Type valueType = null)
        {
            valueType = valueType ?? config.InputControlType.GetGenericArguments()[0];
            if (config.Page == null)
            {
                Console.WriteLine("没有双向绑定");
                return valueType;
            }
            Console.WriteLine("开始双向绑定");
            var createMethod = typeof(EventCallbackFactory).GetMethods().FirstOrDefault(x =>
            {
                if (!x.IsGenericMethod || !x.IsPublic)
                {
                    return false;
                };
                var parameters = x.GetParameters();
                if (parameters.Length < 2)
                {
                    return false;
                }
                var parameterType = parameters[1].ParameterType;
                if (!parameterType.IsGenericType)
                {
                    return false;
                }
                return parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Action<>);
            }).MakeGenericMethod(valueType);
            var parameterExp = Expression.Parameter(valueType);
            var body = Expression.Assign(Expression.Property(Expression.Constant(config), nameof(RenderConfig.EditingValue)), Expression.Convert(parameterExp, typeof(object)));
            var settter = Expression.Lambda(typeof(Action<>).MakeGenericType(valueType), body, parameterExp).Compile();
            var setterMethod = createMethod.Invoke(EventCallback.Factory, new object[] { config.Page, settter });
            builder.AddAttribute(startIndex, propertyName, Convert.ChangeType(setterMethod, typeof(EventCallback<>).MakeGenericType(valueType)));
            return valueType;
        }
    }
}
