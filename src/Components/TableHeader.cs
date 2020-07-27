using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazui.Component
{
    public class TableHeader
    {
        public string Text { get; set; }
        public IntString? Width { get; set; }
        public PropertyInfo Property { get; set; }
        public Func<object, object> Eval { get; set; }
        public bool IsCheckBox { get; set; }
        public RenderFragment<object> Template { get; set; }
        public string Format { get; set; }
        public int SortNo { get; set; }
        public bool IsTree { get; set; }
    }

    public struct IntString
    {
        private string stringValue;
        private int? intValue;
        private float? numberValue;

        public string StringValue { get { return stringValue; } }
        public int? IntValue { get { return intValue; } }

        public IntString(string value)
        {
            this.stringValue = value;
            intValue = null;
            numberValue = null;
            numberValue = ParseNumberValue();
        }
        public IntString(int? value)
        {
            intValue = value;
            stringValue = null;
            numberValue = intValue;
        }
        public IntString(IntString intString)
        {
            stringValue = intString.StringValue;
            intValue = intString.IntValue;
            numberValue = intString.numberValue;
        }
        public override string ToString()
        {
            return stringValue != null ? stringValue : intValue?.ToString();
        }
        private float? ParseNumberValue()
        {
            if (this.IntValue != null)
                return this.IntValue;
            if (string.IsNullOrEmpty(this.StringValue))
                return null;
            var parseResult = false;
            float parseValue;
            if (this.StringValue.Contains("%"))
            {
                parseResult = float.TryParse(this.StringValue.Replace("%", ""), out parseValue);
                parseValue = parseValue / 100;
            }
            else
            {
                parseResult = float.TryParse(this.StringValue, out parseValue);
            }
            if (parseResult)
                return parseValue;
            else
            {
                return null;
            }
        }
        public float? GetNumberValue()
        {
            return numberValue;
        }

        public static bool operator <(IntString x, IntString y)
        {
            var valueX = x.GetNumberValue();
            var valueY = y.GetNumberValue();
            if (valueX == null || valueY == null)
                throw new Exception("无法转换为有效数字类型,该比较无效");
            return valueX < valueY;
        }
        public static bool operator >(IntString x, IntString y)
        {
            var valueX = x.GetNumberValue();
            var valueY = y.GetNumberValue();
            if (valueX == null || valueY == null)
                throw new Exception("无法转换为有效数字类型,该比较无效");
            return valueX > valueY;
        }
        public static bool operator ==(IntString x, IntString y)
        {
            var valueX = x.GetNumberValue();
            var valueY = y.GetNumberValue();
            if (valueX != null && valueY != null)
                return valueX == valueY;

            return x.StringValue == y.StringValue;
        }
        public static bool operator !=(IntString x, IntString y)
        {
            var valueX = x.GetNumberValue();
            var valueY = y.GetNumberValue();
            if (valueX != null && valueY != null)
                return valueX != valueY;

            return x.StringValue != y.StringValue;
        }
        public static implicit operator IntString?(int? value)
        {
            if (value.HasValue)
                return new IntString(value.Value);
            else
            {
                return null;
            }
        }
        public static implicit operator int?(IntString? intString)
        {
            return intString?.IntValue;
        }
        public static implicit operator IntString?(string value)
        {
            if (value != null)
                return new IntString(value);
            else
            {
                return null;
            }
        }
        public static implicit operator string(IntString? intString)
        {
            return intString?.StringValue;
        }
    }
}
