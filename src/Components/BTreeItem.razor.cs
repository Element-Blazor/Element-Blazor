using Element.Model;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Element
{
    public partial class BTreeItem 
    {
        private int level;
        private List<TreeItemBase> children = new List<TreeItemBase>();
        /// <summary>
        /// 节点值
        /// </summary>
        [Parameter]
        public int Id { get; set; }

        /// <summary>
        /// 显示文字
        /// </summary>
        [Parameter]
        public string Text { get; set; }

        [CascadingParameter]
        public BTree Tree { get; set; }

        [CascadingParameter]
        public BTreeItem ParentTreeItem { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            level = (ParentTreeItem?.level + 1) ?? 0;
            var treeItemModel = new TreeItemModel()
            {
                Direction = "right",
                Expanded = false,
                ParentId = ParentTreeItem?.Id ?? 0,
                Text = Text,
                Level = level,
                Id = Id
            };
            Tree.AddChild(treeItemModel);
        }
    }
}
