using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class FormControlAttribute : Attribute
    {
        /// <summary>
        /// 控件类型
        /// </summary>
        public Type ControlType { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; } = true;

        /// <summary>
        /// 必填错误消息
        /// </summary>
        public string RequiredMessage { get; set; }
        public string Label { get; set; }
        public string Image { get; set; }
        public float LabelWidth { get; set; } = 100;
        public string Placeholder { get; internal set; }
    }
}
