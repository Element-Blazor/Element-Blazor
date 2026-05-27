using System;
using System.Collections.Generic;
using System.Text;

namespace Element.ControlConfigs
{
    public class SelectAttribute : Attribute
    {
        /// <summary>
        /// 显示字段
        /// </summary>
        public string Display { get; set; }

        /// <summary>
        /// 值字段
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 数据加载器
        /// </summary>
        public Type DataSourceLoader { get; set; }

        public string Placeholder { get; set; }

        public bool IsDisabled { get; set; }

        public bool Clearable { get; set; } = true;

        public bool Filterable { get; set; }

        public bool Multiple { get; set; }

        public bool CollapseTags { get; set; }

        public int MaxCollapseTags { get; set; } = 1;

        public bool Virtualized { get; set; }

        public int ItemHeight { get; set; } = 34;

        public int Height { get; set; } = 274;
    }
}
