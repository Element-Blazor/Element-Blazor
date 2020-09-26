using System;
using System.Collections.Generic;
using System.Text;

namespace Element.Admin
{
    /// <summary>
    /// 该特性将枚举识别为资源集合，如果在组件中使用了 <seealso cref="ResourceAttribute"/> 特性声明资源，则无需再通过此方法来声明
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class ResourcesAttribute : Attribute
    {
    }
}
