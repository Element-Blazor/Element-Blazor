using System;
using System.Collections.Generic;
using System.Text;

namespace Element.Model
{
    public class SelectResultModel<TKey>
    {
        public TKey Key { get; set; }
        public string Text { get; set; }
    }

    public class SelectResultModel : SelectResultModel<int>
    {

    }
}
