using Blazui.Component.Popup;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class DateTimePickerOption : DropDownOption
    {
        public TaskCompletionSource<DateTime> TaskComplietionSource { get; set; }
        internal DateTime CurrentMonth { get; set; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        internal int ShowYear { get; set; }
        internal int ShowMonth { get; set; }
    }
}
