

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElTransfer : ElementFieldComponentBase<List<string>>
    {
        protected HtmlPropertyBuilder CheckBoxGroupCssBuilder;
        protected string list1KeyWords = string.Empty;
        protected string list2KeyWords = string.Empty;
        private bool hasAppliedValueParameter;
        private List<string> lastAppliedValueParameter;
        private string lastAppliedItemsSignature;
        private Status list1Status = Status.UnChecked;
        internal Status List1Status
        {
            get
            {
                return list1Status;
            }
            set
            {
                var visibleEnabledItems = VisibleList1.Where(x => !x.IsDisabled).ToList();
                List1Checked.RemoveAll(visibleEnabledItems.Contains);
                if (value == Status.Checked)
                {
                    List1Checked.AddRange(visibleEnabledItems);
                }
                list1Status = ResolvePanelStatus(List1Checked, VisibleList1);
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
                var visibleEnabledItems = VisibleList2.Where(x => !x.IsDisabled).ToList();
                List2Checked.RemoveAll(visibleEnabledItems.Contains);
                if (value == Status.Checked)
                {
                    List2Checked.AddRange(visibleEnabledItems);
                }
                list2Status = ResolvePanelStatus(List2Checked, VisibleList2);
                RequireRender = true;
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            ResetList2(value);
        }

        [Parameter]
        public List<string> Value { get; set; }

        [Parameter]
        public List<string> ModelValue
        {
            get => Value;
            set => Value = value;
        }

        [Parameter]
        public EventCallback<List<string>> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<List<string>> ModelValueChanged { get; set; }

        [Parameter]
        public EventCallback<List<string>> OnChange { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [Parameter]
        public bool EnableSearch { get; set; }

        [Parameter]
        public bool Filterable
        {
            get => EnableSearch;
            set => EnableSearch = value;
        }

        [Parameter]
        public Func<string, TransferItem, bool> FilterMethod { get; set; }

        [Parameter]
        public RenderFragment<TransferItem> ItemContent { get; set; }

        [Parameter]
        public RenderFragment<TransferItem> LeftItemContent { get; set; }

        [Parameter]
        public RenderFragment<TransferItem> RightItemContent { get; set; }

        [Parameter]
        public RenderFragment<TransferPanelContext> LeftHeaderContent { get; set; }

        [Parameter]
        public RenderFragment<TransferPanelContext> RightHeaderContent { get; set; }

        [Parameter]
        public RenderFragment<TransferPanelContext> LeftFooterContent { get; set; }

        [Parameter]
        public RenderFragment<TransferPanelContext> RightFooterContent { get; set; }

        [Parameter]
        public string[] ButtonTexts { get; set; }

        [Parameter]
        public string ToLeftText { get; set; }

        [Parameter]
        public string ToRightText { get; set; }

        [Parameter]
        public string EmptyText { get; set; } = "No data";

        [Parameter]
        public string NoMatchText { get; set; } = "No matches";

        /// <summary>
        /// ���б�1����ʱ����
        /// </summary>
        [Parameter]
        public EventCallback<string> OnList1Search { get; set; }

        /// <summary>
        /// �б�1������ PlaceHolder
        /// </summary>
        [Parameter]
        public string List1SearchPlaceHolder { get; set; }

        /// <summary>
        /// �б�2������ PlaceHolder
        /// </summary>
        [Parameter]
        public string List2SearchPlaceHolder { get; set; }

        /// <summary>
        /// ���б�2����ʱ����
        /// </summary>
        [Parameter]
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
        /// ����б��ı���
        /// </summary>
        [Parameter]
        public string LeftTitle { get; set; } = "List 1";

        /// <summary>
        /// �ұ��б��ı���
        /// </summary>
        [Parameter]
        public string RightTitle { get; set; } = "List 2";

        private void ResetList2(object value)
        {
            var valueList = NormalizeValueList(value);
            List1 ??= new List<TransferItem>();
            List2 ??= new List<TransferItem>();
            foreach (var item in List2.Where(x => x != null && !List1.Contains(x)).ToList())
            {
                List1.Add(item);
            }
            if (valueList == null)
            {
                List2.Clear();
            }
            else
            {
                List2 = List1.Where(x => valueList.Contains(x.Id)).ToList();
                List1.RemoveAll(List2.Contains);
            }
            List1Checked.Clear();
            List2Checked.Clear();
            list1Status = Status.UnChecked;
            list2Status = Status.UnChecked;
            RequireRender = true;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            CheckBoxGroupCssBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-transfer-panel__list")
                .AddIf(IsFilterable, "is-filterable");
            List1 ??= new List<TransferItem>();
            List2 ??= new List<TransferItem>();
            if (FormItem == null)
            {
                ApplyValueParameterIfNeeded();
                RemoveInvalidCheckedItems();
                RefreshPanelStatuses();
                return;
            }
            RemoveInvalidCheckedItems();
            RefreshPanelStatuses();
            if (FormItem.OriginValueHasRendered)
            {
                return;
            }
            FormItem.OriginValueHasRendered = true;
            if (FormItem.Form.Values.Any())
            {
                ResetList2(FormItem.OriginValue);
            }
            else if (Value != null)
            {
                ResetList2(Value);
            }
            SyncFieldValue(false);
        }

        internal void ToLeft()
        {
            var movingItems = List2Checked.Where(x => x != null && !x.IsDisabled).ToList();
            if (!movingItems.Any())
            {
                return;
            }
            list2Status = Status.UnChecked;
            list1Status = Status.UnChecked;
            List1.AddRange(movingItems);
            List2.RemoveAll(movingItems.Contains);
            List1Checked.Clear();
            List2Checked.Clear();
            RefreshPanelStatuses();
            RequireRender = true;
            SyncFieldValue(true);
        }

        private void SyncFieldValue(bool validate)
        {
            if (List2 == null)
            {
                return;
            }
            var value = List2.Select(x => x.Id).ToList();
            Value = value;
            RememberAppliedValue(value);
            SetFieldValue(value, validate);
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(value);
            }
            if (ModelValueChanged.HasDelegate)
            {
                _ = ModelValueChanged.InvokeAsync(value);
            }
            if (validate && OnChange.HasDelegate)
            {
                _ = OnChange.InvokeAsync(value);
            }
        }

        internal void ToRight()
        {
            var movingItems = List1Checked.Where(x => x != null && !x.IsDisabled).ToList();
            if (!movingItems.Any())
            {
                return;
            }
            list1Status = Status.UnChecked;
            list2Status = Status.UnChecked;
            if (List2 == null)
            {
                List2 = new List<TransferItem>();
            }
            List2.AddRange(movingItems);
            List1.RemoveAll(movingItems.Contains);
            List1Checked.Clear();
            List2Checked.Clear();
            RefreshPanelStatuses();
            RequireRender = true;
            SyncFieldValue(true);
        }

        internal void Status1Changed(Status status, TransferItem transferItem)
        {
            if (transferItem == null || transferItem.IsDisabled)
            {
                return;
            }
            if (status == Status.Checked)
            {
                if (!List1Checked.Contains(transferItem))
                {
                    List1Checked.Add(transferItem);
                }
            }
            else
            {
                List1Checked.Remove(transferItem);
            }

            list1Status = ResolvePanelStatus(List1Checked, VisibleList1);
            RequireRender = true;
        }
        internal void Status2Changed(Status status, TransferItem transferItem)
        {
            if (transferItem == null || transferItem.IsDisabled)
            {
                return;
            }
            if (status == Status.Checked)
            {
                if (!List2Checked.Contains(transferItem))
                {
                    List2Checked.Add(transferItem);
                }
            }
            else
            {
                List2Checked.Remove(transferItem);
            }

            list2Status = ResolvePanelStatus(List2Checked, VisibleList2);
            RequireRender = true;
        }

        private IReadOnlyList<TransferItem> VisibleList1 => GetVisibleItems(List1, list1KeyWords).ToList();

        private IReadOnlyList<TransferItem> VisibleList2 => GetVisibleItems(List2, list2KeyWords).ToList();

        private bool IsFilterable => EnableSearch;

        private string LeftFilterPlaceholder => List1SearchPlaceHolder ?? "Enter keyword";

        private string RightFilterPlaceholder => List2SearchPlaceHolder ?? "Enter keyword";

        private string ToLeftButtonText => ResolveButtonText(0, ToLeftText);

        private string ToRightButtonText => ResolveButtonText(1, ToRightText);

        private TransferPanelContext LeftPanelContext => CreatePanelContext("left", LeftTitle, List1Checked, List1, VisibleList1);

        private TransferPanelContext RightPanelContext => CreatePanelContext("right", RightTitle, List2Checked, List2, VisibleList2);

        private RenderFragment<TransferItem> ResolveItemContent(bool left)
        {
            return left ? LeftItemContent ?? ItemContent : RightItemContent ?? ItemContent;
        }

        private IEnumerable<TransferItem> GetVisibleItems(IEnumerable<TransferItem> items, string keyword)
        {
            items ??= Enumerable.Empty<TransferItem>();
            if (!IsFilterable || string.IsNullOrWhiteSpace(keyword))
            {
                return items.Where(x => x != null);
            }
            return items.Where(x => x != null && MatchesFilter(keyword, x));
        }

        private bool MatchesFilter(string keyword, TransferItem item)
        {
            if (FilterMethod != null)
            {
                return FilterMethod(keyword, item);
            }
            return (item.Label ?? string.Empty).IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0
                || (item.Id ?? string.Empty).IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private string GetEmptyText(IEnumerable<TransferItem> source, string keyword)
        {
            return IsFilterable && !string.IsNullOrWhiteSpace(keyword) && source?.Any() == true
                ? NoMatchText
                : EmptyText;
        }

        private string ResolveButtonText(int index, string fallback)
        {
            if (!string.IsNullOrWhiteSpace(fallback))
            {
                return fallback;
            }
            return ButtonTexts != null && ButtonTexts.Length > index ? ButtonTexts[index] : null;
        }

        private static TransferPanelContext CreatePanelContext(string direction, string title, ICollection<TransferItem> checkedItems, ICollection<TransferItem> allItems, IReadOnlyCollection<TransferItem> visibleItems)
        {
            return new TransferPanelContext
            {
                Direction = direction,
                Title = title,
                CheckedCount = checkedItems?.Count ?? 0,
                TotalCount = allItems?.Count ?? 0,
                VisibleCount = visibleItems?.Count ?? 0
            };
        }

        private static Status ResolvePanelStatus(ICollection<TransferItem> checkedItems, IEnumerable<TransferItem> visibleItems)
        {
            var enabledItems = visibleItems?.Where(x => x != null && !x.IsDisabled).ToList() ?? new List<TransferItem>();
            if (!enabledItems.Any())
            {
                return Status.UnChecked;
            }
            var checkedCount = enabledItems.Count(x => checkedItems?.Contains(x) == true);
            if (checkedCount == enabledItems.Count)
            {
                return Status.Checked;
            }
            if (checkedCount > 0)
            {
                return Status.Indeterminate;
            }
            return Status.UnChecked;
        }

        private void RefreshPanelStatuses()
        {
            list1Status = ResolvePanelStatus(List1Checked, VisibleList1);
            list2Status = ResolvePanelStatus(List2Checked, VisibleList2);
        }

        private void RemoveInvalidCheckedItems()
        {
            List1Checked.RemoveAll(x => x == null || x.IsDisabled || !List1.Contains(x));
            List2Checked.RemoveAll(x => x == null || x.IsDisabled || !List2.Contains(x));
        }

        private void ApplyValueParameterIfNeeded()
        {
            var itemsSignature = CreateItemsSignature();
            if (!ShouldApplyValueParameter(Value, itemsSignature))
            {
                return;
            }

            ResetList2(Value);
            RememberAppliedValue(Value);
        }

        private bool ShouldApplyValueParameter(List<string> value, string itemsSignature)
        {
            if (!hasAppliedValueParameter)
            {
                return value != null;
            }

            return !StringListsEqual(lastAppliedValueParameter, value)
                || !string.Equals(lastAppliedItemsSignature, itemsSignature, StringComparison.Ordinal);
        }

        private void RememberAppliedValue(List<string> value)
        {
            hasAppliedValueParameter = true;
            lastAppliedValueParameter = value?.ToList();
            lastAppliedItemsSignature = CreateItemsSignature();
        }

        private string CreateItemsSignature()
        {
            return string.Join("|", (List1 ?? Enumerable.Empty<TransferItem>())
                .Concat(List2 ?? Enumerable.Empty<TransferItem>())
                .Where(x => x != null)
                .Select(x => x.Id ?? string.Empty));
        }

        private static bool StringListsEqual(IReadOnlyCollection<string> left, IReadOnlyCollection<string> right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.SequenceEqual(right);
        }

        private static List<string> NormalizeValueList(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is List<string> list)
            {
                return list;
            }
            if (value is IEnumerable<string> enumerable)
            {
                return enumerable.ToList();
            }
            return new List<string> { Convert.ToString(value) };
        }

        /// <summary>
        /// �б�1
        /// </summary>
        [Parameter]
        public List<TransferItem> List1 { get; set; } = new List<TransferItem>();
        internal List<TransferItem> List1Checked { get; set; } = new List<TransferItem>();

        /// <summary>
        /// �б�2
        /// </summary>
        [Parameter]
        public List<TransferItem> List2 { get; set; } = new List<TransferItem>();
        internal List<TransferItem> List2Checked { get; set; } = new List<TransferItem>();
    }
}
