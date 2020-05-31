using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
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

        /// <summary>
        /// 渲染是否忽略该属性
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// 排序编号
        /// </summary>
        public int SortNo { get; set; }
    }
}
