using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component
{
    public class EditorGeneratorAttribute : Attribute
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
        /// 显示文本
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string RequiredMessage { get; set; }
        public string Placeholder { get; set; }

        /// <summary>
        /// 是否忽略该属性
        /// </summary>
        public bool Ignore { get; set; }
        public string Image { get; internal set; }
    }
}
