using System;

namespace Element
{
    public class CalendarDateContext
    {
        public DateTime Date { get; set; }

        public bool IsCurrentMonth { get; set; }

        public bool IsSelected { get; set; }

        public bool IsToday { get; set; }

        public string Type { get; set; }
    }
}
