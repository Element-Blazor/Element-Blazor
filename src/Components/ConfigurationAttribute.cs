using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component
{
    public class ConfigurationAttribute : Attribute
    {
        /// <summary>
        /// 样式文件路径，支持 CSS 和 LESS
        /// </summary>
        public string[] Styles { get; set; } = new string[0];
    }
}
