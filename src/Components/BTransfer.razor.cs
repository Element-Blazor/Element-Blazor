

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public partial class BTransfer
    {
        protected HtmlPropertyBuilder CheckBoxGroupCssBuilder;
        protected string list1KeyWords = string.Empty;
        protected string list2KeyWords = string.Empty;
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
                    if (EnableSearch && !OnList1Search.HasDelegate)
                    {
                        List1Checked = List1.Where(x => x.Label.Contains(list1KeyWords, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    }
                    else
                    {
                        List1Checked = List1.ToList();
                    }
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
                    if (EnableSearch && !OnList2Search.HasDelegate)
                    {
                        List2Checked = List2.Where(x => x.Label.Contains(list2KeyWords, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    }
                    else
                    {
                        List2Checked = List2.ToList();
                    }
                }
                else
                {
                    List2Checked.Clear();
                }
                list2Status = value;
                RequireRender = true;
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRender)
        {
            ResetList2(value);
        }

        /// <summary>
        /// 启用搜索
        /// </summary>
        [Parameter]
        public bool EnableSearch { get; set; }

        /// <summary>
        /// 当列表1搜索时触发
        /// </summary>
        [Parameter]
        public EventCallback<string> OnList1Search { get; set; }

        /// <summary>
        /// 列表1搜索框 PlaceHolder
        /// </summary>
        [Parameter]
        public string List1SearchPlaceHolder { get; set; }

        /// <summary>
        /// 列表2搜索框 PlaceHolder
        /// </summary>
        [Parameter]
        public string List2SearchPlaceHolder { get; set; }

        /// <summary>
        /// 当列表2搜索时触发
        /// </summary>
        public EventCallback<string> OnList2Search { get; set; }

        protected async Task List1SearchChanged(string keywords)
        {
            list1KeyWords = keywords;
            if (OnList1Search.HasDelegate)
            {
                await OnList1Search.InvokeAsync(keywords);
                return;
            }
        }
        protected async Task List2SearchChanged(string keywords)
        {
            list2KeyWords = keywords;
            if (OnList2Search.HasDelegate)
            {
                await OnList2Search.InvokeAsync(keywords);
                return;
            }
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
            CheckBoxGroupCssBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-transfer-panel__list")
                .AddIf(EnableSearch, "is-filterable");
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
            if (List2 == null)
            {
                return;
            }
            SetFieldValue(List2.Select(x => x.Id).ToList(), validate);
        }

        internal void ToRight()
        {
            List2Checked.Clear();
            list1Status = Status.UnChecked;
            list2Status = Status.UnChecked;
            if (List2 == null)
            {
                List2 = new List<TransferItem>();
            }
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
