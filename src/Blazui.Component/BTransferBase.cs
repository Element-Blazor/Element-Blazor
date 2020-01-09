using Blazui.Component.CheckBox;
using Blazui.Component.Form;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BTransferBase : BFieldComponentBase<List<string>>
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

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            ResetList2(value);
        }

        /// <summary>
        /// 左边列表的标题
        /// </summary>
        [Parameter]
        public string LeftTitle { get; set; } = "列表1";

        /// <summary>
        /// 右边列表的标题
        /// </summary>
        [Parameter]
        public string RightTitle { get; set; } = "列表2";

        private void ResetList2(object value)
        {
            var valueList = (List<string>)value;
            List1.AddRange(List2);
            if (valueList == null)
            {
                List2.Clear();
            }
            else
            {
                List2 = List1.Where(x => valueList.Contains(x.Id)).ToList();
                List1.RemoveAll(List2.Contains);
            }
            List1Status = Status.UnChecked;
            List2Status = Status.UnChecked;
            RequireRender = true;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (FormItem == null)
            {
                return;
            }
            if (FormItem.OriginValueHasRendered)
            {
                return;
            }
            FormItem.OriginValueHasRendered = true;
            if (FormItem.Form.Values.Any())
            {
                ResetList2(FormItem.OriginValue);
            }
            SyncFieldValue(false);
        }

        internal void ToLeft()
        {
            List1Checked.Clear();
            list2Status = Status.UnChecked;
            list1Status = Status.UnChecked;
            List1.AddRange(List2Checked);
            List2.RemoveAll(List2Checked.Contains);
            List2Checked.Clear();
            RequireRender = true;
            SyncFieldValue(true);
        }

        private void SyncFieldValue(bool validate)
        {
            SetFieldValue(List2.Select(x => x.Id).ToList(), validate);
        }

        internal void ToRight()
        {
            List2Checked.Clear();
            list1Status = Status.UnChecked;
            list2Status = Status.UnChecked;
            List2.AddRange(List1Checked);
            List1.RemoveAll(List1Checked.Contains);
            List1Checked.Clear();
            RequireRender = true;
            SyncFieldValue(true);
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
