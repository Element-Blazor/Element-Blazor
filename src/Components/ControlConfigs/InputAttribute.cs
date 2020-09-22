using System;
using System.Collections.Generic;
using System.Text;

namespace Element.ControlConfigs
{
    public class InputAttribute : BaseAttribute
    {
        /// <summary>
        /// 输入框类型
        /// </summary>
        public InputType Type { get; set; } = InputType.Text;

        /// <summary>
        /// 输入框尺寸
        /// </summary>
        public InputSize Size { get; set; } = InputSize.Normal;

        /// <summary>
        /// Placeholder
        /// </summary>
        public string Placeholder { get; set; } = "请输入内容";

        /// <summary>
        /// 是否禁用输入框
        /// </summary>
        public bool IsDisabled { get; set; } = false;

        /// <summary>
        /// 是否可清空
        /// </summary>
        public bool IsClearable { get; set; } = false;

        /// <summary>
        /// 前缀图标
        /// </summary>
        public string PrefixIcon { get; set; }

        /// <summary>
        /// 后缀图标
        /// </summary>
        public string SuffixIcon { get; set; }
    }
}
