using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class PopupService
    {
        internal ObservableCollection<DateTimePickerOption> DateTimePickerOptions = new ObservableCollection<DateTimePickerOption>();
        internal ObservableCollection<DropDownOption> SelectDropDownOptions = new ObservableCollection<DropDownOption>();
        internal ObservableCollection<DropDownOption> DropDownMenuOptions = new ObservableCollection<DropDownOption>();
        internal ObservableCollection<SubMenuOption> SubMenuOptions = new ObservableCollection<SubMenuOption>();
        internal ObservableCollection<PopupLayerOption> PopupLayerOptions = new ObservableCollection<PopupLayerOption>();
        internal ObservableCollection<PopupLayerOption> DropDownTreeOptions = new ObservableCollection<PopupLayerOption>();
    }
}
