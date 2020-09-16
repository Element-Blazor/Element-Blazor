using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public partial class BTableColumn
    {
        internal virtual bool IsCheckBox { get; set; }
        [Parameter]
        public virtual string? Width { get; set; }

        internal virtual bool IsTree { get; set; }
        /// <summary>
        /// 排序编号
        /// </summary>
        [Parameter]
        public int SortNo { get; set; }
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

        /// <summary>
        /// 是否可编辑
        /// </summary>
        [Parameter]
        public bool IsEditable { get; set; } = true;

        /// <summary>
        /// 格式化
        /// </summary>
        [Parameter]
        public string Format { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Columns.AddColumn(this);
        }
    }
}
