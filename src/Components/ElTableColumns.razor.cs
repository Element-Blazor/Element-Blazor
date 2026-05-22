using Element.DisplayRenders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElTableColumns
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [CascadingParameter]
        public ElTable Table { get; set; }

        public void AddColumn(ElTableColumn column)
        {
            Table.AddColumn(column);
        }
    }
}
