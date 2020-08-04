using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component
{
    public class EditorAttribute : Attribute
    {
        /// <summary>
        /// 编辑器控件
        /// </summary>
        public Type Control { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; } = true;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string RequiredMessage { get; set; }
        public string Placeholder { get; set; }
    }
}
