using Microsoft.AspNetCore.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Element
{
    public partial class ElTableV2 : ElementComponentBase
    {
        [Parameter]
        public object DataSource { get; set; }

        [Parameter]
        public IList<TableV2Column> Columns { get; set; }

        [Parameter]
        public bool AutoGenerateColumns { get; set; } = true;

        [Parameter]
        public int StartIndex { get; set; }

        [Parameter]
        public int ItemCount { get; set; } = 50;

        [Parameter]
        public string Height { get; set; }

        [Parameter]
        public RenderFragment<object> ExpandContent { get; set; }

        [Parameter]
        public HashSet<object> ExpandedRows { get; set; } = new HashSet<object>();

        [Parameter]
        public bool ShowSummary { get; set; }

        [Parameter]
        public string SumText { get; set; } = "合计";

        [Parameter]
        public Func<TableSummaryContext, object> SummaryMethod { get; set; }

        protected IReadOnlyList<object> Rows => DataSource is IEnumerable enumerable
            ? enumerable.Cast<object>().ToList()
            : new List<object>();

        protected IReadOnlyList<object> VisibleRows => Rows
            .Skip(Math.Max(0, StartIndex))
            .Take(Math.Max(1, ItemCount))
            .ToList();

        protected string TableClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-table-v2", "el-table", Cls)
            .ToString();

        protected string TableStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!string.IsNullOrWhiteSpace(Height), $"height:{ElementCssUtility.NormalizeCssSize(Height)}")
            .Add(Style)
            .ToString();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if ((Columns == null || Columns.Count == 0) && AutoGenerateColumns)
            {
                Columns = CreateColumns().ToList();
            }
        }

        protected string GetColumnStyle(TableV2Column column)
        {
            return HtmlPropertyBuilder.CreateCssStyleBuilder()
                .AddIf(!string.IsNullOrWhiteSpace(column?.Width), $"width:{ElementCssUtility.NormalizeCssSize(column.Width)}")
                .ToString();
        }

        protected void ToggleExpand(object row)
        {
            if (ExpandedRows.Contains(row))
            {
                ExpandedRows.Remove(row);
            }
            else
            {
                ExpandedRows.Add(row);
            }
        }

        protected object GetSummaryCell(TableV2Column column, int index)
        {
            if (index == 0)
            {
                return SumText;
            }

            if (SummaryMethod != null)
            {
                return SummaryMethod(new TableSummaryContext
                {
                    Rows = Rows,
                    Columns = new List<TableHeader>(),
                    ColumnIndex = index
                });
            }

            decimal total = 0;
            var hasValue = false;
            foreach (var row in Rows)
            {
                var value = column.GetValue(row);
                if (decimal.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
                {
                    total += number;
                    hasValue = true;
                }
            }

            return hasValue ? total.ToString(CultureInfo.CurrentCulture) : string.Empty;
        }

        private IEnumerable<TableV2Column> CreateColumns()
        {
            var row = Rows.FirstOrDefault();
            var type = row?.GetType();
            if (type == null)
            {
                return Enumerable.Empty<TableV2Column>();
            }

            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => !x.GetCustomAttributes(true).OfType<TableIgnoreAttribute>().Any())
                .Select(x => new TableV2Column
                {
                    Title = x.Name,
                    Property = x.Name
                });
        }
    }
}
