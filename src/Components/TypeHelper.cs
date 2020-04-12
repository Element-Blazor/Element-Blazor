using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazui.Component
{
    internal static class TypeHelper
    {
        static ComparisonConfig config = new ComparisonConfig();
        static CompareLogic compareLogic = new CompareLogic(config);
        static TypeHelper()
        {

        }
        public static bool Equal<TValue>(TValue value1, TValue value2)
        {
            if (value1 == null && value2 != null)
            {
                return false;
            }
            if (value1 != null && value2 == null)
            {
                return false;
            }
            if (value1 == null && value2 == null)
            {
                return true;
            }
            var valueType = typeof(TValue);
            if (valueType == typeof(string))
            {
                if (value1?.ToString() == value2?.ToString())
                {
                    return true;
                }
                return false;
            }
            if (valueType.IsValueType)
            {
                return compareLogic.Compare(value1, value2).AreEqual;
            }
            return ReferenceEquals(value1, value2);
        }

        public static TValue ChangeType<TValue>(object value)
        {
            return (TValue)ChangeType(value, typeof(TValue));
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
                        destValue = Convert.ChangeType(value, nulltype);
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
