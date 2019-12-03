using Blazui.Component.CheckBox;
using Blazui.Component.Dom;
using Blazui.Component.EventArgs;
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
        protected ElementReference headerElement;
        internal List<TableHeader<TRow>> Headers { get; set; } = new List<TableHeader<TRow>>();
        private bool requireRender = true;
        protected int headerHeight = 49;

        [Parameter]
        public bool AutoGenerateColumns { get; set; } = true;
        [Parameter]
        public bool HasSelectionColumn { get; set; } = true;
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
        public int PageSize { get; set; } = 50;
        /// <summary>
        /// 当前页码，从1开始
        /// </summary>
        [Parameter]
        public int CurrentPage { get; set; } = 1;
        [Parameter]
        public List<TRow> DataSource { get; set; } = new List<TRow>();

        [Parameter]
        public HashSet<TRow> SelectedRows { get; set; } = new HashSet<TRow>();

        protected Status selectAllStatus;

        [Inject]
        private IJSRuntime jsRunTime { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

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
            Total = DataSource.Count;
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
            RefreshSelectAllStatus();
        }
    }
}
