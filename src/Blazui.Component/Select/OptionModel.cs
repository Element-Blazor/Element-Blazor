using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Select
{
    /// <summary>
    /// 下拉组件专用 Model 类，使用该类的属性将强制渲染成下拉框
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class OptionModel<TValue>
    {
        public OptionModel(string text, TValue value)
        {
            Text = text;
            Value = value;
        }

        public string Text { get; set; }
        public TValue Value { get; set; }

        public override string ToString()
        {
            return $"{Value}:{Text}";
        }

        public static implicit operator TValue(OptionModel<TValue> value)
        {
            return value.Value;
        }
    }
}
