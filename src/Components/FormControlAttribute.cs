using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class FormControlAttribute : Attribute
    {
        public int SortNo { get; set; }
        public float LabelWidth { get; set; }

        /// <summary>
        /// 表单自动生成时使用的控件类型。
        /// </summary>
        public Type Control { get; set; }
    }
}
