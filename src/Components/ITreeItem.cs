using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component
{
    public interface ITreeItem
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int? Level { get; set; }
        public string Direction { get; set; }
        public bool IsLoading { get; set; }
        public bool HasChildren { get; set; }
    }
}
