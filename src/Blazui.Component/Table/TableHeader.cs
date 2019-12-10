using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazui.Component.Table
{
    public class TableHeader
    {
        public string Text { get; set; }
        public float? Width { get; set; }
        public PropertyInfo Property { get; set; }
        public Func<object, object> Eval { get; set; }
        public bool IsCheckBox { get; set; }
        public RenderFragment<object> Template { get; set; }
    }
}
