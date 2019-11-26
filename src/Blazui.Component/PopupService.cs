using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class PopupService
    {
        internal ObservableCollection<DateTimePickerOption> DateTimePickerOptions = new ObservableCollection<DateTimePickerOption>();
        internal ObservableCollection<DropDownOption> DropDownOptions = new ObservableCollection<DropDownOption>();
    }
}
