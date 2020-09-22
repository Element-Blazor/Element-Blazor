using System;
using System.Collections.Generic;
using System.Text;

namespace Element.ControlConfigs
{
    public class BaseAttribute : Attribute
    {
        /// <summary>
        /// 背景图片（并非每个组件都生效，我的项目用到的都会生效！！）
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 鼠标放上去的背景图片（并非每个组件都生效，我的项目用到的都会生效！！）
        /// </summary>
        public string HoverImage { get; set; }
        /// <summary>
        /// 鼠标点击的背景图片（并非每个组件都生效，我的项目用到的都会生效！！）
        /// </summary>
        public string ClickImage { get; set; }
        /// <summary>
        /// CSS 样式（并非每个组件都生效，我的项目用到的都会生效！！）
        /// </summary>
        public string Style { get; set; }
    }
}
