using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Table
{
    public class TableColumnAttribute : Attribute
    {
        /// <summary>
        /// 列头文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// 格式化参数，仅支持日期格式
        /// </summary>
        public string Format { get; set; }
    }
}
