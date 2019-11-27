using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazui.Component
{
    internal static class TypeHelper
    {
        public static bool Equal<TValue>(TValue value1, TValue value2)
        {
            if (value1 == default && value2 != default)
            {
                return false;
            }
            if (value1 != default && value2 == default)
            {
                return false;
            }
            if (value1 == default && value2 == default)
            {
                return true;
            }
            var valueType = typeof(TValue);
            if (valueType == typeof(string))
            {
                if (value1.ToString() == value2.ToString())
                {
                    return true;
                }
                return false;
            }
            if (valueType.IsValueType)
            {
                object result = default;
                var equalsMethod = typeof(Nullable).GetMethods().FirstOrDefault(x => x.IsGenericMethod && x.Name == "Equals");
                if (valueType.IsGenericType)
                {
                    if (valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var nullType = Nullable.GetUnderlyingType(valueType);
                        result = equalsMethod.MakeGenericMethod(nullType).Invoke(null, new object[] { value1, value2 });
                    }
                }
                else
                {
                    result = equalsMethod.MakeGenericMethod(valueType).Invoke(null, new object[] { value1, value2 });
                }
                return Convert.ToBoolean(result);
            }
            return ReferenceEquals(value1, value2);
        }

        public static object ChangeType(object value, Type type)
        {
            object destValue = null;
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var nulltype = Nullable.GetUnderlyingType(type);
                    if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        destValue = Convert.ChangeType(value, type);
                    }
                }
                else
                {
                    destValue = Convert.ChangeType(value, type);
                }
            }
            else
            {
                destValue = Convert.ChangeType(value, type);
            }
            return destValue;
        }
    }
}
