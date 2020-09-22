using System;
using System.Collections.Generic;
using System.Text;

namespace Element.ControlConfigs
{
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// 表格高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 表格可编辑
        /// </summary>
        public bool IsEditable { get; set; }
    }
}
