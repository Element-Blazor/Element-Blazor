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

        public void Refresh()
        {
            StateHasChanged();
        }

        private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetFieldValue(SelectedItems.ToList(), true);
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            //if (OnSetValue == null)
            //{
            //    return;
            //}
            SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
            //OnSetValue((List<TValue>)value);
            if (value != null)
            {
                SelectedItems = new ObservableCollection<TValue>(((List<TValue>)value));
            }
            else
            {
                SelectedItems = new ObservableCollection<TValue>();
            }
            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
            SetFieldValue(SelectedItems.ToList(), false);
        }
    }
}
