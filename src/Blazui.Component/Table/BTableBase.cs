using Blazui.Component.CheckBox;
using Blazui.Component.Dom;
using Blazui.Component.EventArgs;
using Blazui.Component.Pagination;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Table
{
    public class BTableBase<TRow> : ComponentBase, IContainerComponent
    {
        internal BPagination Pagination { get; set; }
        protected ElementReference headerElement;
        internal List<TableHeader<TRow>> Headers { get; set; } = new List<TableHeader<TRow>>();
        private bool requireRender = true;
        protected int headerHeight = 49;

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

        /// <summary>
        /// 总记录数
        /// </summary>
        [Parameter]
        public int Total { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        [Parameter]
        public int PageSize { get; set; } = 20;

        private int currentPage = 1;

        /// <summary>
        /// 当前页码，从1开始
        /// </summary>
        [Parameter]
        public int CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                currentPage = value;

                if (CurrentPageChanged.HasDelegate)
                {
                    _ = CurrentPageChanged.InvokeAsync(value);
                }
            }
        }

        /// <summary>
        /// 最大显示的页码数
        /// </summary>
        [Parameter]
        public int PageCount { get; set; } = 7;

        /// <summary>
        /// 当前页码变化时触发
        /// </summary>
        [Parameter]
        public EventCallback<int> CurrentPageChanged { get; set; }

        /// <summary>
        /// 当前最大显示的页码数变化时触发
        /// </summary>
        [Parameter]
        public EventCallback<int> PageCountChanged { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        [Parameter]
        public List<TRow> DataSource { get; set; } = new List<TRow>();

        /// <summary>
        /// 当只有一页时，不显示分页
        /// </summary>
        [Parameter]
        public bool NoPaginationOnSinglePage { get; set; } = true;

        /// <summary>
        /// 选中的记录
        /// </summary>
        [Parameter]
        public HashSet<TRow> SelectedRows { get; set; } = new HashSet<TRow>();

        /// <summary>
        /// 选中的记录变化时触发
        /// </summary>
        [Parameter]
        public EventCallback<HashSet<TRow>> SelectedRowsChanged { get; set; }
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
        /// 启用边框
        /// </summary>
        [Parameter]
        public bool IsBordered { get; set; }
        public ElementReference Container { get; set; }

        protected override void OnInitialized()
        {
            Total = Total <= 0 ? DataSource.Count : Total;
        }
        protected override void OnAfterRender(bool firstRender)
        {
            if (AutoGenerateColumns)
            {
                typeof(TRow).GetProperties().Reverse().ToList().ForEach(property =>
                {
                    if (Headers.Any(x => x.Property == property.Name))
                    {
                        return;
                    }
                    Func<TRow, object> getMethod = row =>
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
                    Headers.Insert(0, new TableHeader<TRow>()
                    {
                        Eval = getMethod,
                        IsCheckBox = property.PropertyType == typeof(bool) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(bool),
                        Property = property.Name,
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
                SelectedRows = new HashSet<TRow>(DataSource);
            }
            else
            {
                SelectedRows = new HashSet<TRow>();
            }

            if (SelectedRowsChanged.HasDelegate)
            {
                _ = SelectedRowsChanged.InvokeAsync(SelectedRows);
            }
            RefreshSelectAllStatus();
        }

        protected void ChangeRowStatus(Status status, TRow row)
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
