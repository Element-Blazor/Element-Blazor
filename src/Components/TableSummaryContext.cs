using System.Collections.Generic;

namespace Element
{
    public class TableSummaryContext
    {
        public IReadOnlyList<object> Rows { get; set; }

        public IReadOnlyList<TableHeader> Columns { get; set; }

        public TableHeader Column { get; set; }

        public int ColumnIndex { get; set; }
    }
}
