using System;
using System.Collections.Generic;
using System.Linq;

namespace Element
{
    public class DateTimePickerShortcut
    {
        public string Text { get; set; }

        public DateTime? Value { get; set; }

        public Func<DateTime?> ValueFactory { get; set; }

        public IEnumerable<DateTime?> RangeValue { get; set; }

        public Func<IEnumerable<DateTime?>> RangeValueFactory { get; set; }

        internal DateTime? ResolveValue()
        {
            return ValueFactory != null ? ValueFactory() : Value;
        }

        internal IList<DateTime?> ResolveRangeValue()
        {
            return (RangeValueFactory != null ? RangeValueFactory() : RangeValue)?.Take(2).ToList()
                ?? new List<DateTime?>();
        }
    }
}
