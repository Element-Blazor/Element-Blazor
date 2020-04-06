



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

namespace Blazui.Component
{
    public class BTableBase : BComponentBase, IContainerComponent
    {
        internal BCheckBoxBase<bool> chkAll;
        internal ElementReference headerElement;
        internal List<TableHeader> Headers { get; set; } = new List<TableHeader>();
        internal bool headerInitilized = false;
        internal bool headerRendered = false;
        private IDictionary<BCheckBoxBase<bool>, object> rowCheckBoxses = new Dictionary<BCheckBoxBase<bool>, object>();
        internal int headerHeight = 49;

        internal List<object> rows = new List<object>();
        /// <summary>
        /// 忽略的字段名称
        /// </summary>
        [Parameter]
        public string[] IgnoreProperties { get; set; } = { };

        /// <summary>
        /// 数据源
        /// </summary>
        [Parameter]
        public object DataSource { get; set; }
        internal Type DataType { get; set; }

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
        /// 当表格无数据时显示的消息
        /// </summary>
        [Parameter]
        public string EmptyMessage { get; set; }
        /// <summary>
        /// 总数据条数
        /// </summary>
        [Parameter]
        public int Total { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        [Parameter]
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 当前页数
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
            }
        }

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

        /// <summary>
        /// 当页码变化时触发
        /// </summary>
        [Parameter]
        public EventCallback<int> CurrentPageChanged { get; set; }


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
        /// 加载中状态背景颜色
        /// </summary>
        [Parameter]
        public string LoadingBackground { get; set; }

        /// <summary>
        /// 加载中状态样式类
        /// </summary>
        [Parameter]
        public string LoadingIconClass { get; set; }

        /// <summary>
        /// 加载中状态文字
        /// </summary>
        public string LoadingText { get; set; }
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

        internal Task OnRowCheckBoxRenderCompleted(object row, BCheckBoxBase<bool> chk)
        {
            lock (rowCheckBoxses)
            {
                rowCheckBoxses.Add(chk, row);
            }
            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            SelectedRows = new HashSet<object>();
            base.OnInitialized();
            if (!AutoGenerateColumns)
            {
                return;
            }
            if (DataSource == null)
            {
                return;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (headerRendered)
            {
                await (OnRenderCompleted?.Invoke(this) ?? Task.CompletedTask);
                return;
            }
            headerRendered = true;
            RequireRender = true;
            StateHasChanged();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (parameters.GetValueOrDefault<object>(nameof(DataSource)) != null)
            {
                rows = (DataSource as IEnumerable).Cast<object>().ToList();
                DataType = DataSource.GetType().GetGenericArguments()[0];
                if (AutoGenerateColumns && !headerInitilized)
                {
                    headerInitilized = true;
                    DataType.GetProperties().Where(p => !IgnoreProperties.Contains(p.Name)).Reverse().ToList().ForEach(property =>
                    {
                        if (Headers.Any(x => x.Property?.Name == property.Name))
                        {
                            return;
                        }
                        var attrs = property.GetCustomAttributes(true);
                        var columnConfig = attrs.OfType<TableColumnAttribute>().FirstOrDefault() ?? new TableColumnAttribute()
                        {
                            Text = property.Name
                        };

                        if (columnConfig.Ignore)
                        {
                            return;
                        }
                        Headers.Insert(0, new TableHeader()
                        {
                            Eval = row =>
                            {
                                var value = property.GetValue(row);
                                if (string.IsNullOrWhiteSpace(columnConfig.Format))
                                {
                                    return value;
                                }
                                if (value == null)
                                {
                                    return null;
                                }

                                try
                                {
                                    return Convert.ToDateTime(value).ToString(columnConfig.Format);
                                }
                                catch (InvalidCastException)
                                {
                                    throw new BlazuiException("仅日期列支持 Format 参数");
                                }
                            },
                            IsCheckBox = property.PropertyType == typeof(bool) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(bool),
                            Property = property,
                            Text = columnConfig.Text,
                            Width = columnConfig.Width
                        });
                    }
                     );
                }
                chkAll?.MarkAsRequireRender();
                ResetSelectAllStatus();
            }
        }

        internal async Task CurrentPageChangedAsync(int page)
        {
            CurrentPage = page;
            if (CurrentPageChanged.HasDelegate)
            {
                RequireRender = true;
                var option = new LoadingOption()
                {
                    Background = LoadingBackground,
                    Target = Container,
                    IconClass = LoadingIconClass,
                    Text = LoadingText
                };
                LoadingService.LoadingOptions.Add(option);
                SelectedRows.Clear();
                await CurrentPageChanged.InvokeAsync(page);
                LoadingService.LoadingOptions.Remove(option);
            }
        }

        void ResetSelectAllStatus()
        {
            if (rows.Count == 0 || SelectedRows.Count == 0)
            {
                selectAllStatus = Status.UnChecked;
            }
            else if (rows.Count > SelectedRows.Count)
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
            RequireRender = true;
            if (status == Status.Checked)
            {
                SelectedRows = new HashSet<object>(rows);
            }
            else
            {
                SelectedRows = new HashSet<object>();
            }

            foreach (var item in rowCheckBoxses.Keys)
            {
                item.MarkAsRequireRender();
            }
            if (SelectedRowsChanged.HasDelegate)
            {
                _ = SelectedRowsChanged.InvokeAsync(SelectedRows);
            }
            ResetSelectAllStatus();
        }

        protected void ChangeRowStatus(Status status, object row)
        {
            RequireRender = true;
            chkAll.MarkAsRequireRender();
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
            ResetSelectAllStatus();
        }
    }
}
