using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component
{
    /// <summary>
    /// 设置读取当前实体的指定属性值对象的指定属性值
    /// </summary>
    public class PropertyAttribute : Attribute
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// 当前实体的指定属性名，为可则读取当前属性的值
        /// </summary>
        public string ModelProperty { get; set; }
    }
}
