using Element.Model;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Element
{
    public partial class BTree : BComponentBase
    {
        private List<TreeItemBase> selectedNodes = new List<TreeItemBase>();
        private List<TreeItemBase> items;
        [Inject]
        private HttpClient httpClient { get; set; }

        /// <summary>
        /// 一次性加载所有节点
        /// </summary>
        [Parameter]
        public bool? LoadFullTree { get; set; }
        /// <summary>
        /// 选择节点后触发
        /// </summary>
        [Parameter]
        public EventCallback<List<TreeItemBase>> SelectedNodesChanged { get; set; }

        /// <summary>
        /// 服务器地址
        /// </summary>
        [Parameter]
        public string Url { get; set; }

        /// <summary>
        /// 节点类型，必须实现 <seealso cref="TreeItemBase"/> 接口
        /// </summary>
        [Parameter]
        public Type ItemType { get; set; } = typeof(TreeItemBase);

        [CascadingParameter]
        public BTreeSingleSelect Select { get; set; }

        /// <summary>
        /// 数据格式
        /// </summary>
        [Parameter]
        public DataFormat DataFormat { get; set; } = DataFormat.List;

        /// <summary>
        /// 自动展开所有层级
        /// </summary>
        [Parameter]
        public bool AutoExpandAll { get; set; } = true;

        /// <summary>
        /// 数据源
        /// </summary>
        [Parameter]
        public List<TreeItemBase> DataSource { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (DataSource != null)
            {
                items = DataSource;
                return;
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!RequireRender)
            {
                return;
            }
            await FetchNodesAsync();
            await InvokeAsync(Refresh);
        }

        private async Task FetchNodesAsync()
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                return;
            }
            EnsureUrl();
            Console.WriteLine(ItemType);
            var responseObj = (IEnumerable)await httpClient.GetFromJsonAsync(Url, typeof(List<>).MakeGenericType(ItemType));
            DataSource = items = responseObj.Cast<TreeItemBase>().ToList();
            foreach (var item in DataSource)
            {
                Console.WriteLine(item.Id);
            }
            var rootNodes = FindChildren(new TreeItemModel()
            {
                Children = items
            });
            Initilize(rootNodes, null);
        }

        private void EnsureUrl()
        {
            if (!Url.EndsWith("/"))
            {
                Url += "/";
            }
        }

        private async Task SelectNodeAsync(TreeItemBase treeNode)
        {
            selectedNodes.Add(treeNode);
            if (SelectedNodesChanged.HasDelegate)
            {
                _ = SelectedNodesChanged.InvokeAsync(selectedNodes);
            }
            if (Select != null)
            {
                await Select.OnInternalSelectAsync(new SelectResultModel<int?>()
                {
                    Text = treeNode.TextPath,
                    Key = treeNode.Id
                });
            }
        }

        private void Initilize(List<TreeItemBase> nodes, TreeItemBase parentNode)
        {
            foreach (var node in nodes)
            {
                node.Level = (parentNode?.Level ?? -1) + 1;
                node.TextPaths = parentNode?.TextPaths == null ? new Dictionary<int, string>() : new Dictionary<int, string>(parentNode.TextPaths);
                if (!node.TextPaths.ContainsKey(node.Id))
                {
                    node.TextPaths.Add(node.Id, node.Text);
                }
                Initilize(FindChildren(node), node);
            }
        }

        /// <summary>
        /// 展开某个节点
        /// </summary>
        /// <param name="treeItem"></param>
        public async Task ExpandAsync(TreeItemBase treeItem, bool autoRefresh = true)
        {
            if (treeItem.Expanded)
            {
                return;
            }
            treeItem.Expanded = true;
            if (LoadFullTree != null && LoadFullTree.Value)
            {
                var children = FindChildren(treeItem);
                await ExpandAsync(children, false);
                if (autoRefresh)
                {
                    Refresh();
                }
                return;
            }
            if (!string.IsNullOrWhiteSpace(Url))
            {
                EnsureUrl();
                var responseObj = (IEnumerable)await httpClient.GetFromJsonAsync(Url + treeItem.Id, typeof(List<>).MakeGenericType(ItemType));
                var newTreeNodes = responseObj.Cast<TreeItemBase>().ToArray();
                DataSource.RemoveAll(x => newTreeNodes.Any(y => y.Id == x.Id));
                DataSource.AddRange(newTreeNodes);
                var rootNodes = FindChildren(new TreeItemModel()
                {
                    Children = items
                });
                Initilize(rootNodes, null);
                await AutoExpandAllAsync();
            }
            Refresh();
        }

        private async Task AutoExpandAllAsync()
        {
            if (DataSource == null)
            {
                return;
            }
            if (!AutoExpandAll)
            {
                return;
            }
            var dataSource = DataSource.ToList();
            if (LoadFullTree == null && dataSource.Any(x => x.Children != null && x.Children.Any()))
            {
                LoadFullTree = true;
            }
            await ExpandAsync(dataSource);
        }

        private async Task ExpandAsync(List<TreeItemBase> dataSource, bool autoRefresh = true)
        {
            foreach (var item in dataSource)
            {
                await ExpandAsync(item, autoRefresh);
            }
        }

        private async Task ToggleAsync(TreeItemBase item)
        {
            if (item.Expanded)
            {
                item.Expanded = false;
                Refresh();
            }
            else
            {
                await ExpandAsync(item);
            }
            item.Expanded = !item.Expanded;
        }

        private List<TreeItemBase> FindChildren(TreeItemBase treeItem)
        {
            switch (DataFormat)
            {
                case DataFormat.List:
                    return items.Where(x => x.ParentId == treeItem.Id).ToList();
                case DataFormat.Children:
                    return treeItem.Children ?? new List<TreeItemBase>();
            }
            return new List<TreeItemBase>();
        }

        internal void AddChild(TreeItemModel treeItemModel)
        {
            items = items ?? new List<TreeItemBase>();
            treeItemModel.Expanded = AutoExpandAll;
            items.Add(treeItemModel);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                Refresh();
                return;
            }
            await AutoExpandAllAsync();
        }
    }
}