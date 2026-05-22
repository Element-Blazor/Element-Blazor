using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Element
{
    public partial class BTableColumn : BComponentBase
    {
        internal virtual bool IsCheckBox { get; set; }
        [Parameter]
        public virtual string Width { get; set; }

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
        [CascadingParameter]
        public BTable Table { get; set; }

        [Parameter]
        public virtual RenderFragment<object> ChildContent { get; set; }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public string Label
        {
            get => Text;
            set => Text = value;
        }

        [Parameter]
        public bool Sortable { get; set; }

        [Parameter]
        public TableSortOrder SortOrder { get; set; }

        [Parameter]
        public TableColumnFixed Fixed { get; set; }

        [Parameter]
        public IList<TableFilterOption> Filters { get; set; }

        [Parameter]
        public Func<object, object, bool> FilterMethod { get; set; }

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
            if (Columns != null)
            {
                Columns.AddColumn(this);
                return;
            }
            if (Table == null)
            {
                throw new BlazuiException($"列 {GetType().Name} 必须放在 BTable/ElTable 或 BTableColumns/ElTableColumns 内");
            }
            Table.AddColumn(this);
        }
    }
}
