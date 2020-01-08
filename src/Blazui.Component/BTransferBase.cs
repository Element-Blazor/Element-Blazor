using Blazui.Component.CheckBox;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BTransferBase : BComponentBase
    {
        private Status list1Status = Status.UnChecked;
        internal Status List1Status
        {
            get
            {
                return list1Status;
            }
            set
            {
                if (value == Status.Checked)
                {
                    List1Checked = List1.ToList();
                }
                else
                {
                    List1Checked.Clear();
                }
                list1Status = value;
                RequireRender = true;
            }
        }
        private Status list2Status = Status.UnChecked;
        internal Status List2Status
        {
            get
            {
                return list2Status;
            }
            set
            {
                if (value == Status.Checked)
                {
                    List2Checked = List2.ToList();
                }
                else
                {
                    List2Checked.Clear();
                }
                list2Status = value;
                RequireRender = true;
            }
        }

        internal void ToLeft()
        {
            List1Checked.Clear();
            list2Status = Status.UnChecked;
            list1Status = Status.UnChecked;
            List1.AddRange(List2Checked);
            List2.Clear();
            List2Checked.Clear();
            RequireRender = true;
        }
        internal void ToRight()
        {
            List2Checked.Clear();
            list1Status = Status.UnChecked;
            list2Status = Status.UnChecked;
            List2.AddRange(List1Checked);
            List1.Clear();
            List1Checked.Clear();
            RequireRender = true;
        }

        internal void Status1Changed(Status status, TransferItem transferItem)
        {
            if (status == Status.Checked)
            {
                List1Checked.Add(transferItem);
            }
            else
            {
                List1Checked.Remove(transferItem);
            }

            if (List1.All(List1Checked.Contains))
            {
                list1Status = Status.Checked;
            }
            else if (List1Checked.Count > 0)
            {
                list1Status = Status.Indeterminate;
            }
            else
            {
                list1Status = Status.UnChecked;
            }
            RequireRender = true;
        }
        internal void Status2Changed(Status status, TransferItem transferItem)
        {
            if (status == Status.Checked)
            {
                List2Checked.Add(transferItem);
            }
            else
            {
                List2Checked.Remove(transferItem);
            }

            if (List2.All(List2Checked.Contains))
            {
                list2Status = Status.Checked;
            }
            else if (List2Checked.Count > 0)
            {
                list2Status = Status.Indeterminate;
            }
            else
            {
                list2Status = Status.UnChecked;
            }
            RequireRender = true;
        }

        /// <summary>
        /// 列表1
        /// </summary>
        [Parameter]
        public List<TransferItem> List1 { get; set; } = new List<TransferItem>();
        internal List<TransferItem> List1Checked { get; set; } = new List<TransferItem>();

        /// <summary>
        /// 列表2
        /// </summary>
        [Parameter]
        public List<TransferItem> List2 { get; set; } = new List<TransferItem>();
        internal List<TransferItem> List2Checked { get; set; } = new List<TransferItem>();
    }
}
