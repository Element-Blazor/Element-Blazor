using Element;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Admin
{
    public class RoleModel
    {
        public string Id { get; set; }

        [TableColumn(Text = "名称")]
        public string Name { get; set; }

        public List<string> Resources { get; set; } = new List<string>();
    }
}
