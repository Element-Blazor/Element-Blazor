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
    }
}
