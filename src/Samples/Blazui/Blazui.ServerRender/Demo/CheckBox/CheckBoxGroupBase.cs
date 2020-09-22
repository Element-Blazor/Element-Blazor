

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Element;
namespace Element.ServerRender.Demo.CheckBox
{
    public class CheckBoxGroupBase : ComponentBase
    {
        public Status Status { get; set; }
        public List<string> Values { get; set; }
        public ObservableCollection<string> SelectedValues { get; set; }

        protected override void OnInitialized()
        {
            Values = new List<string>()
{
                    "列表选项1",
                    "列表选项2",
                    "列表选项3"
                };
            SelectedValues = new ObservableCollection<string>()
{
                "列表选项1",
                "列表选项3"
            };
            Status = Status.Indeterminate;
        }
        public void SelectAll(Status status)
        {
            Status = status;
            if (Status == Status.Checked)
            {
                SelectedValues = new ObservableCollection<string>(Values);
            }
            else if (Status == Status.UnChecked)
            {
                SelectedValues = new ObservableCollection<string>();
            }
        }

        public void ChangeStatus(Status status, string item)
        {
            if (status == Status.UnChecked)
            {
                SelectedValues.Remove(item);
            }
            else
            {
                SelectedValues.Add(item);
            }

            if (Values.All(SelectedValues.Contains))
            {
                Status = Status.Checked;
            }
            else if (SelectedValues.Any())
            {
                Status = Status.Indeterminate;
            }
            else
            {
                Status = Status.UnChecked;
            }
        }
    }
}
