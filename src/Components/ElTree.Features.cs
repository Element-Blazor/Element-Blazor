using Element.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElTree
    {
        private TreeItemBase draggingNode;

        [Parameter]
        public RenderFragment<TreeItemBase> NodeTemplate { get; set; }

        [Parameter]
        public string FilterText { get; set; }

        [Parameter]
        public Func<TreeItemBase, string, bool> FilterNodeMethod { get; set; }

        [Parameter]
        public bool ShowCheckbox { get; set; }

        [Parameter]
        public bool CheckStrictly { get; set; }

        [Parameter]
        public EventCallback<List<TreeItemBase>> CheckedNodesChanged { get; set; }

        [Parameter]
        public bool Lazy { get; set; }

        [Parameter]
        public Func<TreeItemBase, Task<IEnumerable<TreeItemBase>>> LazyLoad { get; set; }

        [Parameter]
        public bool Draggable { get; set; }

        [Parameter]
        public EventCallback<TreeDragEventArgs> OnNodeDrop { get; set; }

        protected string TreeClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-tree", Cls)
            .AddIf(Draggable, "is-draggable")
            .ToString();

        protected IReadOnlyList<TreeItemBase> VisibleRootNodes => GetRootNodes()
            .Where(IsVisible)
            .ToList();

        private IReadOnlyList<TreeItemBase> GetRootNodes()
        {
            if (items == null)
            {
                return new List<TreeItemBase>();
            }

            return DataFormat == DataFormat.List
                ? items.Where(x => x.ParentId == 0).ToList()
                : items;
        }

        protected RenderFragment RenderTreeNode(TreeItemBase node)
        {
            return builder => BuildTreeNode(builder, node);
        }

        private void BuildTreeNode(RenderTreeBuilder builder, TreeItemBase node)
        {
            var seq = 0;
            var children = GetChildrenForRender(node);
            var hasChildren = node.HasChildren || children.Any();
            node.Direction = hasChildren && node.Expanded ? "expanded" : string.Empty;
            var nodeClass = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-tree-node", "is-focusable")
                .AddIf(node.Expanded, "is-expanded")
                .AddIf(node.Disabled, "is-disabled")
                .AddIf(selectedNodes.Contains(node), "is-current")
                .ToString();
            var iconClass = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-tree-node__expand-icon", "el-icon-caret-right", node.Direction)
                .AddIf(!hasChildren, "is-leaf")
                .ToString();

            builder.OpenElement(seq++, "div");
            builder.SetKey(node.Id);
            builder.AddAttribute(seq++, "role", "treeitem");
            builder.AddAttribute(seq++, "tabindex", node.Disabled ? "-1" : "0");
            builder.AddAttribute(seq++, "aria-disabled", node.Disabled.ToString().ToLowerInvariant());
            builder.AddAttribute(seq++, "draggable", Draggable && !node.Disabled ? "true" : "false");
            builder.AddAttribute(seq++, "class", nodeClass);
            builder.AddAttribute(seq++, "ondragstart", EventCallback.Factory.Create<DragEventArgs>(this, _ => OnFeatureDragStart(node)));
            builder.AddAttribute(seq++, "ondragover", EventCallback.Factory.Create<DragEventArgs>(this, OnFeatureDragOverAsync));
            builder.AddAttribute(seq++, "ondrop", EventCallback.Factory.Create<DragEventArgs>(this, _ => OnFeatureDropAsync(node)));

            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "el-tree-node__content");
            builder.AddAttribute(seq++, "style", $"padding-left: {18 * (node.Level ?? 0)}px;");
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, _ => SelectFeatureNodeAsync(node)));

            builder.OpenElement(seq++, "span");
            builder.AddAttribute(seq++, "class", iconClass);
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, _ => ToggleFeatureNodeAsync(node)));
            builder.AddEventStopPropagationAttribute(seq++, "onclick", true);
            builder.CloseElement();

            if (ShowCheckbox)
            {
                builder.OpenComponent<ElCheckbox<bool>>(seq++);
                builder.AddAttribute(seq++, nameof(ElCheckbox<bool>.Status), node.Checked ? Status.Checked : Status.UnChecked);
                builder.AddAttribute(seq++, nameof(ElCheckbox<bool>.IsDisabled), node.Disabled);
                builder.AddAttribute(seq++, nameof(ElCheckbox<bool>.StatusChanged), EventCallback.Factory.Create<Status>(this, status => ToggleFeatureCheckAsync(node, status)));
                builder.CloseComponent();
            }

            builder.OpenElement(seq++, "span");
            builder.AddAttribute(seq++, "class", "el-tree-node__label");
            if (NodeTemplate != null)
            {
                builder.AddContent(seq++, NodeTemplate(node));
            }
            else
            {
                builder.AddContent(seq++, node.Text);
            }
            builder.CloseElement();
            builder.CloseElement();
            builder.CloseElement();

            if (node.Expanded && children.Any())
            {
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "role", "group");
                builder.AddAttribute(seq++, "class", "el-tree-node__children");
                builder.AddAttribute(seq++, "aria-expanded", "true");
                foreach (var child in children)
                {
                    builder.AddContent(seq++, RenderTreeNode(child));
                }
                builder.CloseElement();
            }
        }

        private List<TreeItemBase> GetChildrenForRender(TreeItemBase node)
        {
            return FindFeatureChildren(node).Where(IsVisible).ToList();
        }

        private List<TreeItemBase> FindFeatureChildren(TreeItemBase treeItem)
        {
            if (items == null)
            {
                return new List<TreeItemBase>();
            }

            return DataFormat switch
            {
                DataFormat.List => items.Where(x => x.ParentId == treeItem.Id).ToList(),
                DataFormat.Children => treeItem.Children ?? new List<TreeItemBase>(),
                _ => new List<TreeItemBase>()
            };
        }

        private bool IsVisible(TreeItemBase item)
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                return true;
            }

            if (FilterNodeMethod != null && FilterNodeMethod(item, FilterText))
            {
                return true;
            }

            if (FilterNodeMethod == null && (item.Text ?? string.Empty).Contains(FilterText, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            return FindFeatureChildren(item).Any(IsVisible);
        }

        private async Task SelectFeatureNodeAsync(TreeItemBase treeNode)
        {
            if (treeNode.Disabled)
            {
                return;
            }

            if (!selectedNodes.Contains(treeNode))
            {
                selectedNodes.Add(treeNode);
            }

            if (SelectedNodesChanged.HasDelegate)
            {
                await SelectedNodesChanged.InvokeAsync(selectedNodes);
            }

            if (Select != null)
            {
                await Select.OnInternalSelectAsync(new SelectResultModel<int?>
                {
                    Text = treeNode.TextPath,
                    Key = treeNode.Id
                });
            }
        }

        private async Task ToggleFeatureNodeAsync(TreeItemBase node)
        {
            if (node.Disabled)
            {
                return;
            }

            var children = FindFeatureChildren(node);
            var shouldLoadLazyChildren = Lazy && LazyLoad != null && node.HasChildren && !children.Any();
            if (node.Expanded && !shouldLoadLazyChildren)
            {
                node.Expanded = false;
                Refresh();
                return;
            }

            node.Expanded = true;
            if (shouldLoadLazyChildren)
            {
                node.IsLoading = true;
                Refresh();
                var loaded = (await LazyLoad(node))?.ToList() ?? new List<TreeItemBase>();
                node.IsLoading = false;
                MergeFeatureChildren(node, loaded);
                InitFeatureLevels();
            }

            Refresh();
        }

        private void MergeFeatureChildren(TreeItemBase parent, IList<TreeItemBase> children)
        {
            if (children == null || children.Count == 0)
            {
                parent.HasChildren = false;
                return;
            }

            foreach (var child in children)
            {
                child.ParentId = parent.Id;
                child.Parent = parent;
            }

            if (DataFormat == DataFormat.Children)
            {
                parent.Children = children.ToList();
            }
            else
            {
                items ??= new List<TreeItemBase>();
                items.RemoveAll(x => children.Any(y => y.Id == x.Id));
                items.AddRange(children);
                DataSource = items;
            }
        }

        private void InitFeatureLevels()
        {
            foreach (var root in GetRootNodes())
            {
                InitFeatureLevel(root, null);
            }
        }

        private void InitFeatureLevel(TreeItemBase node, TreeItemBase parent)
        {
            node.Level = (parent?.Level ?? -1) + 1;
            node.TextPaths = parent?.TextPaths == null ? new Dictionary<int, string>() : new Dictionary<int, string>(parent.TextPaths);
            if (!node.TextPaths.ContainsKey(node.Id))
            {
                node.TextPaths.Add(node.Id, node.Text);
            }

            foreach (var child in FindFeatureChildren(node))
            {
                InitFeatureLevel(child, node);
            }
        }

        private async Task ToggleFeatureCheckAsync(TreeItemBase node, Status status)
        {
            var isChecked = status == Status.Checked;
            SetFeatureChecked(node, isChecked);
            if (CheckedNodesChanged.HasDelegate)
            {
                await CheckedNodesChanged.InvokeAsync(GetCheckedNodes());
            }

            Refresh();
        }

        private void SetFeatureChecked(TreeItemBase node, bool isChecked)
        {
            node.Checked = isChecked;
            if (CheckStrictly)
            {
                return;
            }

            foreach (var child in FindFeatureChildren(node))
            {
                SetFeatureChecked(child, isChecked);
            }
        }

        public List<TreeItemBase> GetCheckedNodes()
        {
            return (items ?? new List<TreeItemBase>()).Where(x => x.Checked).ToList();
        }

        private void OnFeatureDragStart(TreeItemBase node)
        {
            if (!Draggable || node.Disabled)
            {
                return;
            }

            draggingNode = node;
        }

        private Task OnFeatureDragOverAsync(DragEventArgs e)
        {
            return Task.CompletedTask;
        }

        private async Task OnFeatureDropAsync(TreeItemBase dropNode)
        {
            if (!Draggable || draggingNode == null || dropNode == null || draggingNode == dropNode)
            {
                return;
            }

            draggingNode.ParentId = dropNode.ParentId;
            if (OnNodeDrop.HasDelegate)
            {
                await OnNodeDrop.InvokeAsync(new TreeDragEventArgs
                {
                    DraggingNode = draggingNode,
                    DropNode = dropNode
                });
            }

            draggingNode = null;
            Refresh();
        }
    }
}
