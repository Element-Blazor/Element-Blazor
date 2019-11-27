using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.CheckBox
{
    public class CheckBoxOption<TModel,TValue>
    {
        public Status Status { get; set; }
        public TValue Value { get; set; }
        public string Label { get; set; }
        public bool IsDisabled { get; set; }
        public TModel Model { get; set; }
    }
}
