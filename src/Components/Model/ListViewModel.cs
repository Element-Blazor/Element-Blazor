using System;
using System.Collections.Generic;
using System.Text;

namespace Element.Model
{
    public class ListViewModel<T>
    {
        [TableColumn(Text = "列表")]
        public T Item { get; set; }
    }
}
