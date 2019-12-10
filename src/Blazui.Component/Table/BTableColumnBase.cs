using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blazui.Component.Table
{
    public class BTableColumnBase : BComponentBase
    {
        internal virtual bool IsCheckBox { get; set; }
        [Parameter]
        public virtual int? Width { get; set; }

        /// <summary>
        /// 当前列关联的属性名
        /// </summary>
        [Parameter]
        public string Property { get; set; }

        [CascadingParameter]
        public BTableColumns Columns { get; set; }

        [Parameter]
        public virtual RenderFragment<object> ChildContent { get; set; }

        [Parameter]
        public string Text { get; set; }

        protected override void OnParametersSet()
        {
            if (Columns == null)
            {
                return;
            }
            Columns.AddColumn(this);
        }
    }
}
