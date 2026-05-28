using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class ElTreeSelect : ElSelect<int?>
    {
        private bool hasExplicitChildContent;
        private bool hasExplicitLabel;
        private RenderFragment explicitChildContent;

        [Parameter]
        public bool? LoadFullTree { get; set; }

        [Parameter]
        public EventCallback<List<TreeItemBase>> SelectedNodesChanged { get; set; }

        [Parameter]
        public string Url { get; set; }

        [Parameter]
        public Type ItemType { get; set; } = typeof(TreeItemBase);

        [Parameter]
        public DataFormat DataFormat { get; set; } = DataFormat.List;

        [Parameter]
        public bool AutoExpandAll { get; set; } = true;

        [Parameter]
        public List<TreeItemBase> DataSource { get; set; }

        [Parameter]
        public List<TreeItemBase> Data
        {
            get => DataSource;
            set => DataSource = value;
        }

        [Parameter]
        public RenderFragment TreeContent { get; set; }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            isTree = true;
            hasExplicitChildContent = parameters.TryGetValue<RenderFragment>(nameof(ChildContent), out explicitChildContent);
            hasExplicitLabel = parameters.TryGetValue<string>(nameof(Label), out _);
            return base.SetParametersAsync(parameters);
        }

        protected override void OnParametersSet()
        {
            isTree = true;
            if (TreeContent != null || !hasExplicitChildContent)
            {
                ChildContent = BuildTreeContent(TreeContent);
            }
            else
            {
                ChildContent = explicitChildContent;
            }

            PrepareTreeData();
            base.OnParametersSet();
            SyncLabelFromValue();
        }

        private RenderFragment BuildTreeContent(RenderFragment childContent)
        {
            return builder =>
            {
                var seq = 0;
                builder.OpenComponent<ElTree>(seq++);
                builder.AddAttribute(seq++, nameof(ElTree.LoadFullTree), LoadFullTree);
                builder.AddAttribute(seq++, nameof(ElTree.SelectedNodesChanged), SelectedNodesChanged);
                builder.AddAttribute(seq++, nameof(ElTree.Url), Url);
                builder.AddAttribute(seq++, nameof(ElTree.ItemType), ItemType ?? typeof(TreeItemBase));
                builder.AddAttribute(seq++, nameof(ElTree.DataFormat), DataFormat);
                builder.AddAttribute(seq++, nameof(ElTree.AutoExpandAll), AutoExpandAll);
                builder.AddAttribute(seq++, nameof(ElTree.DataSource), DataSource);
                if (childContent != null)
                {
                    builder.AddAttribute(seq++, nameof(ElTree.ChildContent), childContent);
                }
                builder.CloseComponent();
            };
        }

        private void SyncLabelFromValue()
        {
            if (hasExplicitLabel || DataSource == null)
            {
                return;
            }

            if (!Value.HasValue)
            {
                Label = string.Empty;
                return;
            }

            var textPath = FindTextPath(Value.Value);
            Label = textPath ?? string.Empty;
        }

        private string FindTextPath(int id)
        {
            return DataFormat == DataFormat.Children
                ? FindTextPathInChildren(DataSource, id)
                : FindTextPathInList(id);
        }

        private string FindTextPathInList(int id)
        {
            var lookup = FlattenTreeItems(DataSource)
                .GroupBy(x => x.Id)
                .ToDictionary(x => x.Key, x => x.First());
            if (!lookup.TryGetValue(id, out var node))
            {
                return null;
            }

            var labels = new Stack<string>();
            while (node != null)
            {
                labels.Push(node.Text);
                if (node.ParentId == 0 || !lookup.TryGetValue(node.ParentId, out node))
                {
                    node = null;
                }
            }

            return string.Join(" > ", labels.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static string FindTextPathInChildren(IEnumerable<TreeItemBase> nodes, int id)
        {
            foreach (var node in nodes ?? Enumerable.Empty<TreeItemBase>())
            {
                if (node.Id == id)
                {
                    return node.Text;
                }

                var childPath = FindTextPathInChildren(node.Children, id);
                if (!string.IsNullOrWhiteSpace(childPath))
                {
                    return string.IsNullOrWhiteSpace(node.Text) ? childPath : $"{node.Text} > {childPath}";
                }
            }

            return null;
        }

        private void PrepareTreeData()
        {
            if (DataSource == null)
            {
                return;
            }

            if (DataFormat == DataFormat.Children)
            {
                foreach (var node in DataSource.Where(x => x != null))
                {
                    PrepareChildrenNode(node, null);
                }

                return;
            }

            var childrenByParent = DataSource
                .Where(x => x != null)
                .GroupBy(x => x.ParentId)
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var node in childrenByParent.TryGetValue(0, out var roots) ? roots : DataSource.Where(x => x?.ParentId == 0))
            {
                PrepareListNode(node, null, childrenByParent);
            }
        }

        private void PrepareListNode(TreeItemBase node, TreeItemBase parent, IDictionary<int, List<TreeItemBase>> childrenByParent)
        {
            PrepareNode(node, parent);
            var hasChildren = childrenByParent.TryGetValue(node.Id, out var children) && children.Any();
            node.HasChildren = node.HasChildren || hasChildren;
            if (AutoExpandAll && hasChildren)
            {
                node.Expanded = true;
            }

            foreach (var child in children ?? Enumerable.Empty<TreeItemBase>())
            {
                PrepareListNode(child, node, childrenByParent);
            }
        }

        private void PrepareChildrenNode(TreeItemBase node, TreeItemBase parent)
        {
            PrepareNode(node, parent);
            var children = node.Children?.Where(x => x != null).ToList() ?? new List<TreeItemBase>();
            node.HasChildren = node.HasChildren || children.Any();
            if (AutoExpandAll && children.Any())
            {
                node.Expanded = true;
            }

            foreach (var child in children)
            {
                if (child.ParentId == 0)
                {
                    child.ParentId = node.Id;
                }
                child.Parent = node;
                PrepareChildrenNode(child, node);
            }
        }

        private static void PrepareNode(TreeItemBase node, TreeItemBase parent)
        {
            node.Level = (parent?.Level ?? -1) + 1;
            node.TextPaths = parent?.TextPaths == null
                ? new Dictionary<int, string>()
                : new Dictionary<int, string>(parent.TextPaths);
            if (!node.TextPaths.ContainsKey(node.Id))
            {
                node.TextPaths.Add(node.Id, node.Text);
            }
        }

        private static IEnumerable<TreeItemBase> FlattenTreeItems(IEnumerable<TreeItemBase> nodes)
        {
            foreach (var node in nodes ?? Enumerable.Empty<TreeItemBase>())
            {
                if (node == null)
                {
                    continue;
                }

                yield return node;
                foreach (var child in FlattenTreeItems(node.Children))
                {
                    yield return child;
                }
            }
        }
    }
}
