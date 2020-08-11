using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component.Model
{
    public class KeyValueModel
    {
        [TableColumn(Text ="名称")]
        public string Key { get; set; }
        [TableColumn(Text = "值")]
        public string Value { get; set; }
    }
}
