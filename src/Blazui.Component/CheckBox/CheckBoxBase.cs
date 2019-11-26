using Blazui.Component.EventArgs;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.CheckBox
{
    public class CheckBoxBase<TModel, TValue> : ComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public Func<TModel, bool> IsChecked { get; set; }

        [Parameter]
        public Func<TModel, bool> IsIndeterminate { get; set; }

        [Parameter]
        public ObservableCollection<TModel> SelectedItems { get; set; }

        [Parameter]
        public IList<TModel> DataSource { get; set; }

        [Parameter]
        public Func<TModel, TValue> Value { get; set; }

        [Parameter]
        public Func<TModel, string> Label { get; set; }

        protected IList<CheckBoxOption<TModel, TValue>> models { get; set; }

        protected void OnInnerStatusChanged(Status status, TModel model)
        {
            if (SelectedItems == null)
            {
                return;
            }
            if (status == Status.Checked)
            {
                lock (SelectedItems)
                {
                    if (!SelectedItems.Contains(model))
                    {
                        SelectedItems.Add(model);
                    }
                }
            }
            else if (status == Status.UnChecked)
            {
                lock (SelectedItems)
                {
                    SelectedItems.Remove(model);
                }
            }
        }

        protected bool ModelItemIsSimpleType { get; set; }

        protected override void OnParametersSet()
        {
            if (DataSource == null)
            {
                return;
            }
            var type = typeof(TModel);
            ModelItemIsSimpleType = type.IsValueType || type.IsPrimitive || type == typeof(string);
            if (ModelItemIsSimpleType)
            {
                models = new List<CheckBoxOption<TModel,TValue>>();
                foreach (var item in DataSource)
                {
                    models.Add(ConvertModelItem(item));
                }
            }
        }

        private CheckBoxOption<TModel,TValue> ConvertModelItem(TModel modelItem)
        {
            if (!ModelItemIsSimpleType)
            {
                return null;
            }
            Status status = CheckBox.Status.Checked;
            if (IsChecked != null)
            {
                status = IsChecked(modelItem) ? Status.Checked : Status.UnChecked;
            }

            if (IsIndeterminate != null)
            {
                status = IsIndeterminate(modelItem) ? Status.Indeterminate : Status.UnChecked;
            }
            if (SelectedItems != null)
            {
                status = SelectedItems.Contains(modelItem) ? Status.Checked : Status.UnChecked;
            }

            var label = string.Empty;
            if (Label != null)
            {
                label = Label(modelItem);
            }
            else if (ModelItemIsSimpleType)
            {
                label = Convert.ToString(modelItem);
            }
            var option = new CheckBoxOption<TModel,TValue>()
            {
                Status = status,
                Value = Value == null ? (TValue)TypeHelper.ChangeType(label,typeof(TValue)) : Value(modelItem),
                IsDisabled = false,
                Label = label,
                Model = modelItem
            };
            return option;

        }

        public void Refresh()
        {
            this.StateHasChanged();
        }
    }
}
