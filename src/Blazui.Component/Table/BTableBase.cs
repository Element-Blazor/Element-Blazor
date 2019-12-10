using Blazui.Component.CheckBox;
using Blazui.Component.Dom;
using Blazui.Component.EventArgs;
using Blazui.Component.Pagination;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Table
{
    public class BTableBase : ComponentBase, IContainerComponent
    {
        internal ElementReference headerElement;
        internal List<TableHeader> Headers { get; set; } = new List<TableHeader>();
        private bool requireRender = true;
        internal int headerHeight = 49;

        /// <summary>
        /// 要显示的实体类型
        /// </summary>
        [Parameter]
        public Type DataType { get; set; }

        /// <summary>
        /// 是否自动生成列
        /// </summary>
        [Parameter]
        public bool AutoGenerateColumns { get; set; } = true;

        /// <summary>
        /// 是否在第一列显示复选框列
        /// </summary>
        [Parameter]
        public bool HasSelectionColumn { get; set; } = true;

        /// <summary>
        /// 当表格渲染结束触发
        /// </summary>
        [Parameter]
        public EventCallback RenderCompleted { get; set; }

        internal int Total { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        [Parameter]
        public int PageSize { get; set; } = 20;

        private int currentPage = 1;

        /// <summary>
        /// 最大显示的页码数
        /// </summary>
        [Parameter]
        public int ShowPageCount { get; set; } = 7;

        /// <summary>
        /// 当前最大显示的页码数变化时触发
        /// </summary>
        [Parameter]
        public EventCallback<int> ShowPageCountChanged { get; set; }

        internal List<object> DataSource { get; set; } = new List<object>();

        /// <summary>
        /// 当只有一页时，不显示分页
        /// </summary>
        [Parameter]
        public bool NoPaginationOnSinglePage { get; set; } = true;

        /// <summary>
        /// 选中的记录
        /// </summary>
        [Parameter]
        public HashSet<object> SelectedRows { get; set; } = new HashSet<object>();

        /// <summary>
        /// 选中的记录变化时触发
        /// </summary>
        [Parameter]
        public EventCallback<HashSet<object>> SelectedRowsChanged { get; set; }
        internal Status selectAllStatus;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// 启用分页
        /// </summary>
        [Parameter]
        public bool EnablePagination { get; set; } = true;

        /// <summary>
        /// 表格高度
        /// </summary>
        [Parameter]
        public int Height { get; set; }

        /// <summary>
        /// 启用斑马纹
        /// </summary>
        [Parameter]
        public bool IsStripe { get; set; }

        /// <summary>
        /// 当加载数据源时触发，传入参数为当前页
        /// </summary>
        [Parameter]
        public Func<int, Task<PagerResult>> OnLoadDataSource { get; set; }
        /// <summary>
        /// 启用边框
        /// </summary>
        [Parameter]
        public bool IsBordered { get; set; }
        public ElementReference Container { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (OnLoadDataSource != null)
            {
                await ChangeCurrentPageAsync(currentPage);
            }

        }

        internal async Task ChangeCurrentPageAsync(int currentPage)
        {
            var pagerResult = await OnLoadDataSource(currentPage);
            Total = pagerResult.Total;
            var dataSource = pagerResult.Rows as IEnumerable;
            DataSource.Clear();
            foreach (var item in dataSource)
            {
                DataSource.Add(item);
            }
            SelectedRows.Clear();
            RefreshSelectAllStatus();
        }
        protected override void OnAfterRender(bool firstRender)
        {
            if (AutoGenerateColumns)
            {
                if (Headers == null)
                {
                    Headers = new List<TableHeader>();
                }
                DataType.GetProperties().Reverse().ToList().ForEach(property =>
                 {
                     if (Headers.Any(x => x.Property?.Name == property.Name))
                     {
                         return;
                     }
                     Func<object, object> getMethod = row =>
                       {
                           return property.GetValue(row);
                       };
                     var attrs = property.GetCustomAttributes(true);
                     var text = attrs.OfType<DisplayAttribute>().FirstOrDefault()?.Name;
                     if (string.IsNullOrWhiteSpace(text))
                     {
                         text = attrs.OfType<DescriptionAttribute>().FirstOrDefault()?.Description;
                     }
                     var width = attrs.OfType<WidthAttribute>().FirstOrDefault()?.Width;
                     Headers.Insert(0, new TableHeader()
                     {
                         Eval = getMethod,
                         IsCheckBox = property.PropertyType == typeof(bool) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(bool),
                         Property = property,
                         Text = text ?? property.Name,
                         Width = width
                     });
                 });

            }
            if (requireRender)
            {
                StateHasChanged();
                requireRender = false;
                return;
            }
            if (RenderCompleted.HasDelegate)
            {
                RenderCompleted.InvokeAsync(null);
            }
        }

        protected override void OnParametersSet()
        {
            RefreshSelectAllStatus();
        }

        void RefreshSelectAllStatus()
        {
            if (DataSource.Count == 0 || SelectedRows.Count == 0)
            {
                selectAllStatus = Status.UnChecked;
            }
            else if (DataSource.Count > SelectedRows.Count)
            {
                selectAllStatus = Status.Indeterminate;
            }
            else
            {
                selectAllStatus = Status.Checked;
            }
        }

        protected void ChangeAllStatus(Status status)
        {
            if (status == Status.Checked)
            {
                SelectedRows = new HashSet<object>(DataSource);
            }
            else
            {
                SelectedRows = new HashSet<object>();
            }

            if (SelectedRowsChanged.HasDelegate)
            {
                _ = SelectedRowsChanged.InvokeAsync(SelectedRows);
            }
            RefreshSelectAllStatus();
        }

        protected void ChangeRowStatus(Status status, object row)
        {
            if (status == Status.Checked)
            {
                SelectedRows.Add(row);
            }
            else
            {
                SelectedRows.Remove(row);
            }
            if (SelectedRowsChanged.HasDelegate)
            {
                _ = SelectedRowsChanged.InvokeAsync(SelectedRows);
            }
            RefreshSelectAllStatus();
        }
    }
}
