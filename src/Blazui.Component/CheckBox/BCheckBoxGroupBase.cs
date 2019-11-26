using Blazui.Component.Form;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.CheckBox
{
    public class BCheckBoxGroupBase<TValue> : BFieldComponentBase<IEnumerable<TValue>>
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        internal ObservableCollection<TValue> SelectedItems { get; set; } = new ObservableCollection<TValue>();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
        }

        private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetFieldValue(SelectedItems.ToList());
        }

        protected override void FormItem_OnReset()
        {
            SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
            SelectedItems = new ObservableCollection<TValue>();
            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
        }
    }
}
