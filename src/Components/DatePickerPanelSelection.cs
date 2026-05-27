using System;

namespace Element
{
    public class DatePickerPanelSelection
    {
        public DatePickerPanelSelection(DateTime value, DatePickerPanelType type)
        {
            Value = value;
            Type = type;
        }

        public DateTime Value { get; }

        public DatePickerPanelType Type { get; }
    }
}
