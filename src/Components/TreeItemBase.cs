using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Blazui.Component
{
    public class TreeItemBase
    {
        [TableIgnore]
        public int Id { get; set; }
        [TableIgnore]
        public virtual int ParentId { get; set; }
        [TableIgnore]
        public virtual int? Level { get; set; }
        [TableIgnore]
        public virtual string Direction { get; set; }
        [TableIgnore]
        public virtual bool IsLoading { get; set; }
        [TableIgnore]
        public virtual bool HasChildren { get; set; }
        [TableIgnore]
        public virtual bool Expanded { get; set; }
        [TableIgnore]
        public virtual string Text { get; set; }
        [TableIgnore]
        public virtual List<TreeItemBase> Children { get; set; }
        [TableIgnore]
        public virtual TreeItemBase Parent { get; set; }
        [TableIgnore]
        [JsonIgnore]
        public virtual IDictionary<int, string> TextPaths { get; set; }
        [TableIgnore]
        public string TextPath
        {
            get
            {
                if (TextPaths == null)
                {
                    return string.Empty;
                }
                return string.Join(" > ", TextPaths.Values ?? new List<string>());
            }
        }
    }
}
