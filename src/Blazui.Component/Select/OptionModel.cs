using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Select
{
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
    }
}
